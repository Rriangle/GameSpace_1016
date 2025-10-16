using Microsoft.AspNetCore.Mvc;
// ★ 引用你在 social_hub 區域底下的通知服務介面
using GamiPort.Areas.social_hub.Services.Notifications;

namespace GamiPort.Areas.social_hub.Controllers
{
	// ★ 這是 MVC 的 Area 標記：會把本控制器的路由前綴定在 /social_hub/...
	//    例如本檔案的 Index 動作路徑會是：/social_hub/Home/Index
	[Area("social_hub")]
	public class HomeController : Controller
	{
		// 透過 DI 注入極簡通知服務
		private readonly INotificationService _notify;

		// ★ 建構式注入：DI 容器會把對應的實作（NotificationService）塞進來
		public HomeController(INotificationService notify)
		{
			_notify = notify;
		}

		// GET: /social_hub/Home/Index
		// ★ 回傳 View（你的按鈕就放在這個頁面）
		public IActionResult Index()
		{
			return View();
		}

		// POST: /social_hub/Home/SendDemo
		// ★ 這個動作供 Index 頁面上的表單（或 AJAX）呼叫，用來示範「按一下就寄通知」
		//   - [HttpPost]：限制只能用 POST
		//   - [ValidateAntiForgeryToken]：需要在 View 的 <form> 內加入 @Html.AntiForgeryToken()
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendDemo()
		{
			// 你可以把這些值改成實際業務邏輯或表單參數傳入
			// ---------------------------------------------------------
			// sourceId / actionId：對應 Notification_Sources / Notification_Actions 的主鍵
			// （服務內會檢查是否存在；不存在會失敗）
			int sourceId = 2; // 範例：2 = 論壇
			int actionId = 4; // 範例：4 = 新留言（依你的資料表對照）

			// 收件人：擇一或兩者都給（服務會分別驗證是否存在）
			// toUserId: null 代表這次不寄給使用者
			// toManagerId: 30000100 代表寄給管理員 #30000100（需在 ManagerData 表存在）
			int? toUserId = null;
			int? toManagerId = 30000100;

			// 寄件者規則（很重要）：
			// - 兩者同時為 null ⇒ 「系統寄件」
			// - 只有一者有值 ⇒ 代表使用者寄件或管理員寄件，且該寄件者必須存在於相對應表
			// - 兩者同時有值 ⇒ 服務會回傳 AmbiguousSender（失敗）
			//
			// 目前這裡是「管理員寄件」（因為 senderManagerId 有值；senderUserId 為 null）
			int? senderUserId = null;
			int? senderManagerId = 30000001; // 這個管理員 ID 必須存在於 ManagerData 表

			// 顯示文字（可為 null；超過長度會自動截斷且回傳警告碼）
			string? title = "你有一則新通知（Demo）";       // Title ≤ 100（依你的 schema）
			string? message = "這是一則來自 Index 按鈕的測試通知"; // Message ≤ 255（依你的 schema）

			// 若有群組概念可傳 groupId；沒有就留 null
			int? groupId = null;

			// ★ 呼叫通知服務：所有驗證（來源/動作存在、寄件者/收件者存在、長度截斷）都在服務層完成
			//   - 若想支援「用戶取消請求」可把 ct 改成 HttpContext.RequestAborted
			var result = await _notify.SendAsync(
				sourceId: sourceId,
				actionId: actionId,
				toUserId: toUserId,
				toManagerId: toManagerId,
				senderUserId: senderUserId,
				senderManagerId: senderManagerId,
				title: title,
				message: message,
				groupId: groupId
			// ct: HttpContext.RequestAborted
			);

			// ★ 用 TempData 把結果帶回 Index 顯示（TempData 只活一個 Redirect）
			if (!result.Success)
			{
				// 常見錯誤碼：
				// - UnknownSourceId / UnknownActionId（來源或動作不存在）
				// - NoRecipient / NoValidRecipient（沒給收件人或收件人不存在）
				// - AmbiguousSender（兩種 sender 同時有值）
				// - InvalidSenderUser / InvalidSenderManager（寄件者不存在）
				// - DbFailure（資料庫例外，已回滾）
				TempData["toast"] = "寄送失敗：" + string.Join("、", result.Errors);
			}
			else
			{
				// 成功：會有 NotificationId 與 RecipientsAdded（0/1/2）
				// 警告碼（Warnings）可能包含：
				// - TitleTruncated / MessageTruncated（字數截斷）
				var warn = result.Warnings.Count > 0
					? $"（警告：{string.Join("、", result.Warnings)}）"
					: "";
				TempData["toast"] =
					$"寄送成功：通知 #{result.NotificationId}，收件人數：{result.RecipientsAdded}{warn}";
			}

			// ★ Post/Redirect/Get：避免使用者重新整理造成重複送出
			return RedirectToAction(nameof(Index));
		}

		// --- 進階版本（若你要從表單帶參數就改用這個；上面那個註解掉） ---
		// [HttpPost]
		// [ValidateAntiForgeryToken]
		// public async Task<IActionResult> SendDemo(
		//     int sourceId, int actionId,
		//     int? toUserId, int? toManagerId,
		//     int? senderUserId, int? senderManagerId,
		//     string? title, string? message)
		// {
		//     var result = await _notify.SendAsync(
		//         sourceId, actionId, toUserId, toManagerId,
		//         senderUserId, senderManagerId,
		//         title, message, groupId: null);
		//
		//     TempData["toast"] = result.Success
		//         ? $"寄送成功：通知 #{result.NotificationId}，收件人數：{result.RecipientsAdded}"
		//         : "寄送失敗：" + string.Join("、", result.Errors);
		//
		//     return RedirectToAction(nameof(Index));
		// }
	}
}
