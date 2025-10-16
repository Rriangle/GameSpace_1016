// 依賴：你的 EF 實體（Notification / NotificationRecipient / Users / ManagerData / NotificationSources / NotificationActions）
using GamiPort.Models;
using Microsoft.EntityFrameworkCore;

namespace GamiPort.Areas.social_hub.Services.Notifications
{
	/// <summary>
	/// 【極簡通知服務實作（單一方法）】
	/// 規格重點：
	///  1) 必填：sourceId、actionId、收件人（toUserId 或 toManagerId 至少一個）
	///  2) 寄件者（sender_user_id / sender_manager_id）：
	///     - 兩者同為 null ⇒ 視為「系統寄件」（不檢查寄件者存在性）
	///     - 有且只有一者有值 ⇒ 必須驗證該寄件者存在（Users / ManagerData）
	///     - 兩者同時有值 ⇒ 避免歧義，直接失敗
	///  3) title / message 可為 null；若有值：
	///     - Title 長度 ≤ 100、Message 長度 ≤ 255（對齊你的 ModelBuilder）
	///     - 超過會截斷並回傳警告（不阻擋寄送）
	///  4) 僅在「所有必要驗證通過且至少一位有效收件者」時才會寫入：
	///     - 建立 Notifications 主檔（created_at 交由 DB 預設 sysutcdatetime() 產生）
	///     - 建立 Notification_Recipients 明細（最多兩筆：User/Manager 各一）
	///     - 以上以單一交易提交，任何例外回滾，避免孤兒資料
	///
	/// 索引利用（依你 ModelBuilder）：
	///  - IX_Notifications_Source_Action_Created：後續查詢最新某來源×動作通知會比較快
	///  - IX_Recipients_User_Read / IX_Recipients_Manager_Read：未讀匣查詢會吃到索引
	/// </summary>
	public sealed class NotificationService : INotificationService
	{
		// DbContext（per-request Scoped，由 DI 注入）
		private readonly GameSpacedatabaseContext _db;

		// 對齊你的 ModelBuilder：Title=100、Message=255
		private const int TITLE_MAX = 100;
		private const int MESSAGE_MAX = 255;

		/// <summary>
		/// 透過 DI 注入 DbContext。
		/// 注意：建構子名稱需與類別同名且「不可」有回傳型別（不能寫成 void）。
		/// </summary>
		public NotificationService(GameSpacedatabaseContext db)
		{
			_db = db;
		}

		/// <summary>
		/// 寄出一則通知（單/雙收件者）。
		/// </summary>
		/// <param name="sourceId">來源（例如：交友/論壇/商城...）— 必須存在於 Notification_Sources</param>
		/// <param name="actionId">動作（例如：公告/交易完成/新留言...）— 必須存在於 Notification_Actions</param>
		/// <param name="toUserId">收件使用者（可空；與 toManagerId 至少擇一有效）</param>
		/// <param name="toManagerId">收件管理員（可空；與 toUserId 至少擇一有效）</param>
		/// <param name="senderUserId">寄件者（使用者；可空）</param>
		/// <param name="senderManagerId">寄件者（管理員；可空）</param>
		/// <param name="title">標題（可空；若有值長度 ≤ 100，超過會截斷並回傳警告）</param>
		/// <param name="message">內容（可空；若有值長度 ≤ 255，超過會截斷並回傳警告）</param>
		/// <param name="groupId">群組 ID（可空；對齊你的 schema，用於群發或歸檔）</param>
		/// <param name="ct">取消權杖：建議從 Controller 帶入 HttpContext.RequestAborted</param>
		/// <returns>NotificationSendResult：Success / NotificationId / RecipientsAdded / Errors / Warnings</returns>
		public async Task<NotificationSendResult> SendAsync(
			int sourceId,
			int actionId,
			int? toUserId = null,
			int? toManagerId = null,
			int? senderUserId = null,
			int? senderManagerId = null,
			string? title = null,
			string? message = null,
			int? groupId = null,
			CancellationToken ct = default)
		{
			// ---------- 0) Fail-Fast：基本輸入健檢 ----------
			// 基本必填：sourceId、actionId
			if (sourceId <= 0 || actionId <= 0)
				return NotificationSendResult.Fail("InvalidSourceOrAction");

			// 至少要有一個收件人（toUserId 或 toManagerId）
			var needUser = toUserId.HasValue && toUserId.Value > 0;
			var needMgr = toManagerId.HasValue && toManagerId.Value > 0;
			if (!needUser && !needMgr)
				return NotificationSendResult.Fail("NoRecipient");

			// Sender 規則：
			//  - 兩者同為 null ⇒ 系統寄件（不驗證寄件者存在性）
			//  - 只有一者有值 ⇒ 必須驗證該寄件者存在
			//  - 兩者同時有值 ⇒ 視為歧義，直接失敗
			var senderIsSystem = (!senderUserId.HasValue && !senderManagerId.HasValue);
			if (!senderIsSystem && senderUserId.HasValue && senderManagerId.HasValue)
				return NotificationSendResult.Fail("AmbiguousSender");

			// 內容長度預檢：僅在有值時截斷（避免丟進 DB 觸發長度例外）
			var warnings = new List<string>(2);
			if (!string.IsNullOrWhiteSpace(title) && title!.Length > TITLE_MAX)
			{
				title = title.Substring(0, TITLE_MAX);
				warnings.Add("TitleTruncated");
			}
			if (!string.IsNullOrWhiteSpace(message) && message!.Length > MESSAGE_MAX)
			{
				message = message.Substring(0, MESSAGE_MAX);
				warnings.Add("MessageTruncated");
			}

			// ---------- 1) 參照存在性驗證（以 AsNoTracking 降低追蹤成本） ----------
			// 檢查 Source 是否存在
			var srcExists = await _db.NotificationSources
				.AsNoTracking()
				.AnyAsync(s => s.SourceId == sourceId, ct);
			if (!srcExists)
				return NotificationSendResult.Fail("UnknownSourceId");

			// 檢查 Action 是否存在
			var actExists = await _db.NotificationActions
				.AsNoTracking()
				.AnyAsync(a => a.ActionId == actionId, ct);
			if (!actExists)
				return NotificationSendResult.Fail("UnknownActionId");

			// 檢查收件者存在性：至少要有一個有效收件者
			bool userValid = false, mgrValid = false;

			if (needUser)
			{
				userValid = await _db.Users
					.AsNoTracking()
					.AnyAsync(u => u.UserId == toUserId!.Value, ct);
				// 若不存在，不直接失敗，改成「不寄給使用者」並在最後檢查是否兩邊都無效
			}

			if (needMgr)
			{
				mgrValid = await _db.ManagerData
					.AsNoTracking()
					.AnyAsync(m => m.ManagerId == toManagerId!.Value, ct);
				// 同上：不存在則不寄給管理員
			}

			if (!userValid && !mgrValid)
				return NotificationSendResult.Fail("NoValidRecipient");

			// 檢查寄件者存在性（非系統寄件才檢查）
			if (!senderIsSystem && senderUserId.HasValue)
			{
				var ok = await _db.Users
					.AsNoTracking()
					.AnyAsync(u => u.UserId == senderUserId.Value, ct);
				if (!ok)
					return NotificationSendResult.Fail("InvalidSenderUser");
			}

			if (!senderIsSystem && senderManagerId.HasValue)
			{
				var ok = await _db.ManagerData
					.AsNoTracking()
					.AnyAsync(m => m.ManagerId == senderManagerId.Value, ct);
				if (!ok)
					return NotificationSendResult.Fail("InvalidSenderManager");
			}

			// ---------- 2) 寫入（交易保護，避免孤兒資料） ----------
			// 說明：
			//  - 先寫主檔 Notifications（created_at 不手動指定，交由 DB DEFAULT sysutcdatetime()）
			//  - 取回 n.NotificationId 後，依「有效收件者」新增對應的 Recipients
			//  - 全部成功才 Commit；任何錯誤都 Rollback
			await using var tx = await _db.Database.BeginTransactionAsync(ct);
			try
			{
				// (a) 新增主檔
				var n = new Notification
				{
					SourceId = sourceId,
					ActionId = actionId,
					GroupId = groupId,
					SenderUserId = senderUserId,       // 可 null（系統寄件或管理員寄件時，使用者為 null）
					SenderManagerId = senderManagerId,    // 可 null（系統寄件或使用者寄件時，管理員為 null）
														  // title/message 若是空白字串，轉為 null；否則 Trim 後寫入
					Title = string.IsNullOrWhiteSpace(title) ? null : title!.Trim(),
					Message = string.IsNullOrWhiteSpace(message) ? null : message!.Trim()
					// CreatedAt 不指定，DB 會用 sysutcdatetime() 自動填
				};

				_db.Notifications.Add(n);
				await _db.SaveChangesAsync(ct); // 取得自動產生的 notification_id

				// (b) 依有效收件者新增明細（最多兩筆：User / Manager 各一）
				var added = 0;

				if (userValid)
				{
					_db.NotificationRecipients.Add(new NotificationRecipient
					{
						NotificationId = n.NotificationId,
						UserId = toUserId,   // 使用者收件
						ManagerId = null,
						ReadAt = null        // 初始未讀
					});
					added++;
				}

				if (mgrValid)
				{
					_db.NotificationRecipients.Add(new NotificationRecipient
					{
						NotificationId = n.NotificationId,
						UserId = null,
						ManagerId = toManagerId, // 管理員收件
						ReadAt = null
					});
					added++;
				}

				await _db.SaveChangesAsync(ct);   // 實際寫入明細
				await tx.CommitAsync(ct);         // 交易提交

				// 成功：回傳通知編號與實際寫入的收件筆數，並帶上任何非阻擋型警告（例如截斷）
				return NotificationSendResult.Ok(n.NotificationId, added, warnings);
			}
			catch
			{
				// 任一階段拋出例外 → 回滾，避免只寫了主檔/只寫了部分明細
				await tx.RollbackAsync(ct);
				// （可選）在這裡記錄 Log：logger.LogError(ex, "Notification send failed");
				return NotificationSendResult.Fail("DbFailure");
			}
		}
	}
}
