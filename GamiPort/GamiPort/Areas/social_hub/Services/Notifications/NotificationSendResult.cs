// 若你的專案沒有啟用「Implicit Usings」，需要這個 using 才能使用 List<T>
using System.Collections.Generic;

namespace GamiPort.Areas.social_hub.Services.Notifications
{
	/// <summary>
	/// 寄送結果 DTO（純資料物件）
	/// - 用來回報「有沒有寄成功、寫入了哪些資料、有哪些錯誤/警告」。
	/// - 設計為 sealed（不可被繼承），避免被誤用擴充造成語意不一致。
	/// - 以工廠方法 Ok/Fail 建立實例，外部不直接 new；可確保狀態（Success/Ids/Counts）一致。
	/// </summary>
	public sealed class NotificationSendResult
	{
		/// <summary>
		/// 寄送是否成功：
		/// - true：代表主檔已建立，且至少有 1 筆收件明細成功寫入。
		/// - false：代表整體作業未完成（通常沒有任何資料寫入）。
		/// </summary>
		public bool Success { get; private set; }

		/// <summary>
		/// 成功時的 notification_id；失敗時為 null。
		/// - 方便呼叫端日後追蹤或跳轉到「通知詳情」。
		/// </summary>
		public int? NotificationId { get; private set; }

		/// <summary>
		/// 實際寫入的收件人數（0/1/2）
		/// - 0：理論上只會出現在 Fail 的情況（無有效收件者）。
		/// - 1：只寄給使用者或只寄給管理員。
		/// - 2：同時各寫一筆（使用者＋管理員）。
		/// </summary>
		public int RecipientsAdded { get; private set; }

		/// <summary>
		/// 錯誤代碼（阻擋寄送）：
		/// 常見值：
		/// - "InvalidSourceOrAction"、"UnknownSourceId"、"UnknownActionId"
		/// - "NoRecipient"、"NoValidRecipient"
		/// - "AmbiguousSender"、"InvalidSenderUser"、"InvalidSenderManager"
		/// - "DbFailure"
		/// </summary>
		public List<string> Errors { get; } = new(); // 若你的 C# 版本不支援 target-typed new，改成 new List<string>()。

		/// <summary>
		/// 警告代碼（不阻擋寄送）：
		/// 常見值：
		/// - "TitleTruncated"（標題超過 100 被截斷）
		/// - "MessageTruncated"（內文超過 255 被截斷）
		/// - 其他非致命提示亦可記錄在此。
		/// </summary>
		public List<string> Warnings { get; } = new();

		// 私有建構子：外界不能直接 new；改用 Ok/Fail 工廠方法，確保狀態一貫性。
		private NotificationSendResult() { }

		/// <summary>
		/// 建立「失敗」結果：
		/// - Success=false、NotificationId=null、RecipientsAdded=0。
		/// - 可帶入 0..n 個錯誤碼，方便呼叫端顯示或記錄。
		/// </summary>
		public static NotificationSendResult Fail(params string[] errors)
		{
			var r = new NotificationSendResult
			{
				Success = false,
				NotificationId = null,
				RecipientsAdded = 0
			};
			if (errors != null) r.Errors.AddRange(errors);
			return r;
		}

		/// <summary>
		/// 建立「成功」結果：
		/// - Success=true，帶回資料庫產生的 notificationId 與實際寫入的收件筆數。
		/// - 可選擇帶入 0..n 個警告碼（例如：字數截斷）。
		/// </summary>
		public static NotificationSendResult Ok(int notificationId, int recipientsAdded, IEnumerable<string>? warnings = null)
		{
			var r = new NotificationSendResult
			{
				Success = true,
				NotificationId = notificationId,
				RecipientsAdded = recipientsAdded
			};
			if (warnings != null) r.Warnings.AddRange(warnings);
			return r;
		}
	}
}
