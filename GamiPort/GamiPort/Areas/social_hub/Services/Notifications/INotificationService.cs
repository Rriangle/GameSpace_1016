namespace GamiPort.Areas.social_hub.Services.Notifications
{
	/// <summary>
	/// 【極簡通知服務介面】
	/// 單一方法：給 SourceId/ActionId + 收件人（User/Manager），其餘選填。
	/// 注意：介面內不能有與介面同名的成員；也不要放任何類別或靜態方法。
	/// </summary>
	public interface INotificationService
	{
		/// <summary>
		/// 送出一則通知（單筆、單/雙收件者）。
		/// 規則：
		///  - 驗證 sourceId/actionId 是否存在；不存在 → 失敗
		///  - 驗證收件人是否存在：兩者皆無效 → 失敗
		///  - sender_user_id / sender_manager_id：
		///       兩者同為 null → 系統寄件；
		///       僅一者有值 → 必須存在；
		///       同時有值 → 失敗（避免歧義）
		///  - 成功時建立 Notifications（created_at 由 DB 預設 UTC），再建立 Recipients
		/// </summary>
		Task<NotificationSendResult> SendAsync(
			int sourceId,
			int actionId,
			int? toUserId = null,
			int? toManagerId = null,
			int? senderUserId = null,
			int? senderManagerId = null,
			string? title = null,
			string? message = null,
			int? groupId = null,
			CancellationToken ct = default);
	}
}
