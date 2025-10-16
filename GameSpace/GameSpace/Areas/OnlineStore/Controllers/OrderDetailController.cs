using GameSpace.Areas.OnlineStore.ViewModels;
using GameSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static GameSpace.Areas.OnlineStore.ViewModels.OrderDetailViewModels;

namespace GameSpace.Areas.OnlineStore.Controllers
{
	[Area("OnlineStore")]
	public class OrderDetailController : Controller
	{
		private readonly GameSpacedatabaseContext _dbContext;// ① 這是你的 EF Core DbContext

		// ② 透過相依性注入拿到 DbContext 實例
		public OrderDetailController(GameSpacedatabaseContext dbContext)
		{
			_dbContext = dbContext;
		}
		#region 取得訂單明細

		// GET: /OnlineStore/Orders/Detail/123
		[HttpGet]
		public async Task<IActionResult> Detail(int id) // ③ id = OrderInfo.order_id
		{
			// ④ 從 OrderInfo 主表查一筆，投影到 ViewModel
			var vm = await _dbContext.OrderInfos
				.AsNoTracking()
				.Where(o => o.OrderId == id) // ⑤ o 代表 OrderInfo 的一列資料
				.Select(o => new OrderDetailViewModels
				{
					// ====== 一行一行來，左邊=ViewModel 欄位，右邊=資料來源 ======
					OrderId = o.OrderId,
					OrderCode = o.OrderCode,     // 來源：OrderInfo.OrderCode（o 的屬性）
					UserId = o.UserId,        // 來源：OrderInfo.UserId
					OrderTotal = o.OrderTotal,    // 來源：OrderInfo.OrderTotal
					OrderStatus = o.OrderStatus,   // 來源：OrderInfo.OrderStatus
					OrderDate = o.OrderDate,     // 來源：OrderInfo.OrderDate

					//付款
					PaymentCode = _dbContext.PaymentTransactions
						.Where(p => p.OrderId == o.OrderId
									&& p.TxnType == "PAY"
									&& p.Status == "SUCCESS")
						.OrderByDescending(p => p.CreatedAt)     // 或改成 p.PaymentAt（看你欄位）
						.Select(p => p.PaymentCode)              // 若沒有 PaymentCode，先用 p.PaymentId 或 p.ProviderTxn
						.FirstOrDefault(),

					PaymentAt = _dbContext.PaymentTransactions
						.Where(p => p.OrderId == o.OrderId
									&& p.TxnType == "PAY"
									&& p.Status == "SUCCESS")
						.OrderByDescending(p => p.CreatedAt)
						.Select(p => (DateTime?)p.CreatedAt)     // 沒資料會回 null → 建議 ViewModel 用 DateTime?
						.FirstOrDefault(),

					PaymentStatus = _dbContext.PaymentTransactions
						.Where(p => p.OrderId == o.OrderId)
						.OrderByDescending(p => p.CreatedAt)
						.Select(p => p.Status)
						.FirstOrDefault() ?? o.PaymentStatus,     // 完全沒交易 → 退回主表欄位

					//出貨
					// ✅ 出貨單號（沒有欄位就先用 shipment_id 或 provider 的單號代打）
					ShipmentCode = _dbContext.Shipments
						.Where(s => s.OrderId == o.OrderId)
						.OrderByDescending(s => s.ShippedAt)      // 或 .OrderByDescending(s => s.ShippedAt)
						.Select(s => s.ShipmentCode)              // 若沒有 ShipmentCode 欄位，改成 s.ShipmentId 或 s.ProviderShipmentNo
						.FirstOrDefault(),                        // 建議 ViewModel 用 long?，避免沒資料顯示 0

					// ✅ 出貨狀態（你 ViewModel 取名是 Status＝出貨狀態）
					Status = _dbContext.Shipments
						.Where(s => s.OrderId == o.OrderId)
						.OrderByDescending(s => s.ShippedAt)
						.Select(s => s.Status)                    // 來源：Shipments.Status
						.FirstOrDefault() ?? "未出貨",             // 沒任何出貨紀錄 → 顯示「未出貨」

					// ✅ 實際出貨時間（如果你 ViewModel 有 ShippedAt，建議新增）
					ShippedAt = _dbContext.Shipments
						.Where(s => s.OrderId == o.OrderId && s.ShippedAt != null)
						.OrderByDescending(s => s.ShippedAt)
						.Select(s => (DateTime?)s.ShippedAt)      // 轉成可空，沒有就回 null
						.FirstOrDefault(),


					//地址
					// === 收件地址：1:1，所以不需要任何排序，直接取這筆訂單的地址 ===
					Recipient = _dbContext.OrderAddresses
						.Where(a => a.OrderId == o.OrderId)      // 來源表：OrderAddresses，條件：a.OrderId = 這筆訂單 o.OrderId
						.Select(a => a.Recipient)                // 來源欄位：OrderAddresses.Recipient
						.FirstOrDefault() ?? "",                 // VM 是非可空字串，沒資料給空字串

					Phone = _dbContext.OrderAddresses
						.Where(a => a.OrderId == o.OrderId)
						.Select(a => a.Phone)                    // 來源欄位：OrderAddresses.Phone
						.FirstOrDefault() ?? "",

					Zipcode = _dbContext.OrderAddresses
						.Where(a => a.OrderId == o.OrderId)
						.Select(a => a.Zipcode)                  // 來源欄位：OrderAddresses.Zipcode
						.FirstOrDefault() ?? "",

					Address1 = _dbContext.OrderAddresses
						.Where(a => a.OrderId == o.OrderId)
						.Select(a => a.Address1)                 // 來源欄位：OrderAddresses.Address1
						.FirstOrDefault() ?? "",

					Address2 = _dbContext.OrderAddresses
						.Where(a => a.OrderId == o.OrderId)
						.Select(a => a.Address2)                 // 來源欄位：OrderAddresses.Address2（可為 null）
						.FirstOrDefault(),                       // VM 是 string?，可直接拿 null

					City = _dbContext.OrderAddresses
						.Where(a => a.OrderId == o.OrderId)
						.Select(a => a.City)                     // 來源欄位：OrderAddresses.City
						.FirstOrDefault() ?? "",

					Country = _dbContext.OrderAddresses
						.Where(a => a.OrderId == o.OrderId)
						.Select(a => a.Country)                  // 來源欄位：OrderAddresses.Country
						.FirstOrDefault() ?? "",
				})
				.FirstOrDefaultAsync();

			if (vm == null) return NotFound(); // ⑥ 查不到就回 404

			// 取出這筆訂單的所有明細
			var items = await _dbContext.OrderItems
				.AsNoTracking()
				.Where(oi => oi.OrderId == id)                         // 關聯條件：OrderItems.OrderId = 這筆訂單
				.Select(oi => new OrderItemRowVM
				{
					ProductId = oi.ProductId,                         // 來源：OrderItems.ProductId
					ProductName = _dbContext.ProductInfos              // 來源：ProductInfos.ProductName（用子查詢取名稱）
						.Where(p => p.ProductId == oi.ProductId)
						.Select(p => p.ProductName)
						.FirstOrDefault() ?? $"#{oi.ProductId}",       // 若找不到名稱就顯示 #ProductId
					ProductCode = _dbContext.ProductCodes
						.Where(pc => pc.ProductId == oi.ProductId)
						.Select(pc => pc.ProductCode1                   /* scaffold 若生 ProductCode1 就改這個 */)
						.FirstOrDefault(),
					Quantity = oi.Quantity,                          // 來源：OrderItems.Quantity
					UnitPrice = oi.UnitPrice,                         // 來源：OrderItems.UnitPrice
					Subtotal = oi.UnitPrice * oi.Quantity            // 來源：計算（或改用 oi.Subtotal）
				})
				.ToListAsync();

			vm.Items = items;

			return View("OrderDetail", vm); // ⑦ 把 ViewModel 丟給你現成的 OrderDetail.cshtml
		}
		#endregion

		#region 搜尋明細
		[HttpGet]
		public IActionResult Search(string? q, string scope = "all")
		{
			if (string.IsNullOrWhiteSpace(q))
			{
				ViewBag.Q = "";
				return View(new List<OrderInfo>()); // 先回傳空清單
			}

			// 只示範：當數字 → 當 order_id / order_code 查
			// 之後再教你擴充模糊查
			IQueryable<OrderInfo> query = _dbContext.OrderInfos.AsNoTracking();
			switch (scope)
			{
				case "order_id":
					if (int.TryParse(q, out var id))
						query = query.Where(o => o.OrderId == id);
					else
						query = query.Where(o => false);
					break;
				case "order_code":
					if (long.TryParse(q, out var code))
						query = query.Where(o => o.OrderCode == code);
					else
						query = query.Where(o => false);
					break;
				default: // "all"：先嘗試 code，再嘗試 id（誰符合就出現誰）
					bool hasCode = long.TryParse(q, out var codeAll);
					bool hasId = int.TryParse(q, out var idAll);

					if (hasCode || hasId)
					{
						query = query.Where(o =>
							(hasCode && o.OrderCode == codeAll) ||
							(hasId && o.OrderId == idAll));
					}
					else
					{
						query = query.Where(o => false);
					}
					break;
			}

			var results = query
				.OrderByDescending(o => o.OrderDate)
				.Take(50) // 先限 50 筆
				.ToList();

			ViewBag.Q = q;
			return View(results);
		}
		#endregion

		#region 地址修改

		// GET: /OnlineStore/Orders/EditAddress/123
		[HttpGet]
		public async Task<IActionResult> EditAddress(int orderId)
		{
			// 讀訂單狀態
			var order = await _dbContext.OrderInfos
				.AsNoTracking()
				.Where(o => o.OrderId == orderId)
				.Select(o => new { o.OrderId, o.OrderStatus })
				.FirstOrDefaultAsync();

			if (order == null) return NotFound();

			// 出貨後直接導回 Detail，並顯示不可編輯
			if (await IsLockedForAddressChangeAsync(orderId))
			{
				TempData["Info"] = "已出貨後不可修改收件地址。";
				return RedirectToAction(nameof(Detail), new { id = orderId });
			}

			// 讀地址（可能 1:1 已存在；若不存在就給空白）
			var addr = await _dbContext.OrderAddresses
				.AsNoTracking()
				.FirstOrDefaultAsync(a => a.OrderId == orderId);

			var vm = new ShippingAddressEditViewModels
			{
				OrderId = orderId,
				Recipient = addr?.Recipient ?? "",
				Phone = addr?.Phone ?? "",
				Zipcode = addr?.Zipcode ?? "",
				Address1 = addr?.Address1 ?? "",
				Address2 = addr?.Address2,
				City = addr?.City ?? "",
				Country = addr?.Country ?? "台灣"
			};
			ViewBag.Countries = GetCountrySelectList(vm.Country ?? "台灣");
			ViewBag.TaiwanCities = GetTaiwanCitySelectList(vm.City);

			return View(vm); // 導到編輯頁（非 modal，整頁）
		}

		// Areas/OnlineStore/Controllers/OrderDetailController.cs

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditAddress(ShippingAddressEditViewModels vm)
		{
			// ★ 防呆：確保有拿到 VM
			if (vm == null)
			{
				TempData["Error"] = "沒有收到表單資料 (vm == null)。";
				return RedirectToAction(nameof(Detail), new { area = "OnlineStore", id = 0 });
			}

			var orderId = vm.OrderId;
			ModelState.Remove(nameof(ShippingAddressEditViewModels.OrderStatus));

			// ★ 先確認訂單存在
			var order = await _dbContext.OrderInfos
				.Where(o => o.OrderId == orderId)
				.Select(o => new { o.OrderId, o.OrderStatus })
				.FirstOrDefaultAsync();

			if (order == null)
			{
				TempData["Error"] = $"找不到訂單（OrderId={orderId}）。";
				return RedirectToAction(nameof(Detail), new { area = "OnlineStore", id = orderId });
			}

			// ★ 後端狀態鎖：若不可改，直接導回 Detail 並告知原因
			if (await IsLockedForAddressChangeAsync(orderId))
			{
				TempData["Info"] = "已出貨後不可修改收件地址。";
				return RedirectToAction(nameof(Detail), new { area = "OnlineStore", id = orderId });
			}

			// ★ 若模型驗證失敗，把錯誤整理進 TempData 並回到 View
			if (!ModelState.IsValid)
			{
				var errs = string.Join("; ",
					ModelState.Where(kv => kv.Value?.Errors?.Count > 0)
							  .Select(kv => $"{kv.Key}: {string.Join(" | ", kv.Value!.Errors.Select(e => e.ErrorMessage))}"));
				TempData["Error"] = string.IsNullOrWhiteSpace(errs) ? "ModelState 驗證失敗（未知原因）。" : $"驗證失敗：{errs}";
				ViewBag.Countries = GetCountrySelectList(vm.Country ?? "台灣");
				ViewBag.TaiwanCities = GetTaiwanCitySelectList(vm.City);
				return View(vm); // 留在同頁，讓欄位錯誤顯示
			}

			// ★ 實際寫入 OrderAddresses
			var addr = await _dbContext.OrderAddresses
				.FirstOrDefaultAsync(a => a.OrderId == orderId);

			if (addr == null)
			{
				addr = new OrderAddress
				{
					OrderId = orderId,
					Recipient = vm.Recipient?.Trim(),
					Phone = vm.Phone?.Trim(),
					Zipcode = vm.Zipcode?.Trim(),
					Address1 = vm.Address1?.Trim(),
					Address2 = vm.Address2?.Trim(),
					City = vm.City?.Trim(),
					Country = vm.Country?.Trim(),
					// 如果有 CreatedAt/UpdatedAt 欄位可在此補上
				};
				_dbContext.OrderAddresses.Add(addr);
			}
			else
			{
				addr.Recipient = vm.Recipient?.Trim();
				addr.Phone = vm.Phone?.Trim();
				addr.Zipcode = vm.Zipcode?.Trim();
				addr.Address1 = vm.Address1?.Trim();
				addr.Address2 = vm.Address2?.Trim();
				addr.City = vm.City?.Trim();
				addr.Country = vm.Country?.Trim();
				// addr.UpdatedAt = DateTime.UtcNow; // 若有此欄位
			}

			// ★ 一定要存！
			var affected = await _dbContext.SaveChangesAsync();

			// ★ 存檔結果提示（方便你確認到底有沒有寫進 DB）
			if (affected > 0)
			{
				TempData["Success"] = $"收件地址已更新（影響筆數：{affected}）。";
			}
			else
			{
				TempData["Warn"] = "呼叫 SaveChangesAsync() 但沒有更新任何筆數（可能值相同或追蹤狀態有問題）。";
			}

			// ★ 一定導回 Detail（使用 area + id）
			return RedirectToAction(nameof(Detail), new { area = "OnlineStore", id = orderId });
		}

		private async Task<bool> IsLockedForAddressChangeAsync(int orderId)
		{
			// 1) 狀態字串（中英文都擋）
			var status = await _dbContext.OrderInfos
				.Where(o => o.OrderId == orderId)
				.Select(o => o.OrderStatus)
				.FirstOrDefaultAsync() ?? "";

			var blockedStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"SHIPPED","DELIVERED","RETURNED","CANCELLED",
				"已出貨","已送達","已退貨","已取消"
			};
			if (blockedStatuses.Contains(status)) return true;

			// 2) 只要已有實際出貨紀錄也視為鎖定
			var hasShipment = await _dbContext.Shipments
				.AnyAsync(s => s.OrderId == orderId && s.ShippedAt != null);
			return hasShipment;
		}

		private static IEnumerable<SelectListItem> GetCountrySelectList(string? selected = "台灣")
		{
			var countries = new[]
			{
				"台灣","日本","韓國","中國","香港","新加坡","馬來西亞",
				"美國","加拿大","英國","德國","法國","西班牙","義大利",
				"澳洲","紐西蘭","荷蘭","瑞典","瑞士","挪威","丹麥","芬蘭",
				"泰國","越南","印尼","菲律賓","印度"};
			// TODO: 需要 ISO 3166 全清單時，我可以幫你生一份 JSON 放 wwwroot/data/countries.json 再載入

			return countries.Select(c => new SelectListItem { Text = c, Value = c, Selected = c == (selected ?? "台灣") });
		}

		// 台灣縣市完整清單
		private static readonly string[] TaiwanCities = new[]
		{
			"臺北市","新北市","桃園市","臺中市","臺南市","高雄市",
			"基隆市","新竹市","嘉義市",
			"新竹縣","苗栗縣","彰化縣","南投縣","雲林縣","嘉義縣",
			"屏東縣","宜蘭縣","花蓮縣","臺東縣",
			"澎湖縣","金門縣","連江縣"};

		private static IEnumerable<SelectListItem> GetTaiwanCitySelectList(string? selected = null)
		{
			return TaiwanCities.Select(c => new SelectListItem { Text = c, Value = c, Selected = c == selected });
		}



		#endregion
		// 請將檔案最後的多餘的 "}" 移除，或確認 class/namespace 的結尾括號數量正確。
		// 你的檔案結尾目前是：
		//	}
		// 應該是 class 的結尾，如果已經有 class 的結尾，請不要再加多餘的 "}"。
		// 如果你有額外的 namespace 或 class 沒有正確關閉，也會導致 CS1513。
		// 請檢查所有的 "{" 和 "}" 是否配對。

		// 根據你提供的內容，建議如下：
		// 如果這是檔案的最後一行，且 class 已經正確結束，則不需修改。
		// 如果你有多餘的 "}"，請刪除它。
		// 如果 class/namespace 沒有正確結束，請補上 "}"。

		// 範例修正（假設 class 結尾）：



		//	#region 作廢
		//	[HttpPost]
		//	[ValidateAntiForgeryToken]
		//	public async Task<IActionResult> VoidOrder([FromBody] VoidOrderDto dto)
		//	{
		//		if (dto == null || dto.OrderId <= 0 || string.IsNullOrWhiteSpace(dto.Reason))
		//			return BadRequest("參數不完整");

		//		var order = await _dbContext.OrderInfos.FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);
		//		if (order == null) return NotFound("找不到訂單");

		//		// 依你的商規決定是否允許作廢（示例：已出貨/已完成不能作廢）
		//		if (order.OrderStatus is "已出貨" or "已完成" or "已作廢")
		//			return BadRequest("目前狀態不可作廢");

		//		var history = new OrderStatusHistory
		//		{
		//			OrderId = order.OrderId,
		//			FromStatus = order.OrderStatus,
		//			ToStatus = "已作廢",
		//			ChangedBy = /* TODO: 目前登入管理員ID */ null,
		//			ChangedAt = DateTime.UtcNow,
		//			Note = dto.Reason
		//		};

		//		order.OrderStatus = "已作廢";

		//		_dbContext.OrderStatusHistories.Add(history);
		//		await _dbContext.SaveChangesAsync();
		//		return Ok();
		//	}
		//}

		//public class VoidOrderDto
		//{
		//	public int OrderId { get; set; }
		//	public string Reason { get; set; } = string.Empty;
		//}


		////#endregion
	}
}




