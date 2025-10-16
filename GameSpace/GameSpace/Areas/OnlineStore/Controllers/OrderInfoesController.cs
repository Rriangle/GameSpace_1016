using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameSpace.Models;

namespace GameSpace.Areas.OnlineStore.Controllers
{
    [Area("OnlineStore")]
    public class OrderInfoesController : Controller
    {
        private readonly GameSpacedatabaseContext _dbContext;

        public OrderInfoesController(GameSpacedatabaseContext dbContext)
        {
			_dbContext = dbContext;
		}

		// GET: OnlineStore/OrderInfoes
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		// DataTables 伺服器端資料（含日期/狀態/ID/金額篩選；修正 DateTime? 型別問題）
		[HttpGet]
		public async Task<IActionResult> List()
		{
			// === DataTables 參數 ===
			var draw = Request.Query["draw"].FirstOrDefault();
			int start = int.TryParse(Request.Query["start"], out var s) ? s : 0;
			int length = int.TryParse(Request.Query["length"], out var l) ? l : 10;
			string searchValue = Request.Query["search[value]"].FirstOrDefault() ?? string.Empty;

			int sortColIndex = int.TryParse(Request.Query["order[0][column]"], out var c) ? c : 0;
			string sortDir = Request.Query["order[0][dir]"].FirstOrDefault() ?? "asc";
			string sortColKey = Request.Query[$"columns[{sortColIndex}][data]"].FirstOrDefault() ?? "orderDate";

			var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["orderCode"] = nameof(OrderInfo.OrderCode),
				["userId"] = nameof(OrderInfo.UserId),
				["orderDate"] = nameof(OrderInfo.OrderDate),
				["orderStatus"] = nameof(OrderInfo.OrderStatus),
				["paymentStatus"] = nameof(OrderInfo.PaymentStatus),
				["orderTotal"] = nameof(OrderInfo.OrderTotal),
				["paymentAt"] = nameof(OrderInfo.PaymentAt),
				["shippedAt"] = nameof(OrderInfo.ShippedAt),
				["completedAt"] = nameof(OrderInfo.CompletedAt),
			};
			columnMap.TryGetValue(sortColKey, out var efSortCol);
			efSortCol ??= nameof(OrderInfo.OrderDate);

			// === 自訂篩選參數 ===
			// 日期欄位：all | order | payment | shipped | completed
			var dateField = (Request.Query["dateField"].FirstOrDefault() ?? "all").Trim().ToLowerInvariant();
			DateTime? dateStart = DateTime.TryParse(Request.Query["dateStart"], out var ds) ? ds.Date : (DateTime?)null;
			DateTime? dateEnd = DateTime.TryParse(Request.Query["dateEnd"], out var de) ? de.Date : (DateTime?)null;
			DateTime? endExclusive = dateEnd?.AddDays(1); // 半開區間 [start, end)

			// 金額區間
			decimal? minTotal = decimal.TryParse(Request.Query["minTotal"], out var minT) ? minT : (decimal?)null;
			decimal? maxTotal = decimal.TryParse(Request.Query["maxTotal"], out var maxT) ? maxT : (decimal?)null;

			// 狀態
			string orderStatus = Request.Query["orderStatus"].FirstOrDefault() ?? "";
			string paymentStatus = Request.Query["paymentStatus"].FirstOrDefault() ?? "";
			string shipmentStatus = Request.Query["shipmentStatus"].FirstOrDefault() ?? ""; // 以 ShippedAt/CompletedAt 推導

			// 精準 ID 類型/值：order_id | order_code（預留 shipment_code/payment_code）
			string idType = (Request.Query["idType"].FirstOrDefault() ?? "").Trim().ToLowerInvariant();
			string idValue = (Request.Query["idValue"].FirstOrDefault() ?? "").Trim();

			// === 查詢基礎 ===
			var query = _dbContext.OrderInfos.AsNoTracking();
			int recordsTotal = await query.CountAsync();

			// DataTables 全欄位快速搜尋（保留）
			if (!string.IsNullOrWhiteSpace(searchValue))
			{
				query = query.Where(o =>
					o.OrderCode.ToString().Contains(searchValue) ||
					o.UserId.ToString().Contains(searchValue) ||
					(o.OrderStatus ?? "").Contains(searchValue) ||
					(o.PaymentStatus ?? "").Contains(searchValue)
				);
			}

			// 精準 ID
			if (!string.IsNullOrEmpty(idType) && !string.IsNullOrEmpty(idValue))
			{
				switch (idType)
				{
					case "order_id":
						if (int.TryParse(idValue, out var oid))
							query = query.Where(o => o.OrderId == oid);
						else
							query = query.Where(o => false);
						break;

					case "order_code":
						if (long.TryParse(idValue, out var ocode))
							query = query.Where(o => o.OrderCode == ocode);
						else
							query = query.Where(o => false);
						break;
				}
			}

			// 日期區間（支援同日：start==end）
			if (dateField != "all" && (dateStart.HasValue || endExclusive.HasValue))
			{
				query = dateField switch
				{
					"order" => ApplyBetweenNullableDate(query, nameof(OrderInfo.OrderDate), dateStart, endExclusive),
					"payment" => ApplyBetweenNullableDate(query, nameof(OrderInfo.PaymentAt), dateStart, endExclusive),
					"shipped" => ApplyBetweenNullableDate(query, nameof(OrderInfo.ShippedAt), dateStart, endExclusive),
					"completed" => ApplyBetweenNullableDate(query, nameof(OrderInfo.CompletedAt), dateStart, endExclusive),
					_ => query
				};
			}

			// 狀態
			if (!string.IsNullOrWhiteSpace(orderStatus))
				query = query.Where(o => (o.OrderStatus ?? "") == orderStatus);

			if (!string.IsNullOrWhiteSpace(paymentStatus))
				query = query.Where(o => (o.PaymentStatus ?? "") == paymentStatus);

			// 出貨狀態（推導）
			if (!string.IsNullOrWhiteSpace(shipmentStatus))
			{
				switch (shipmentStatus)
				{
					case "未出貨":
						query = query.Where(o => o.ShippedAt == null && o.CompletedAt == null);
						break;
					case "已出貨":
						query = query.Where(o => o.ShippedAt != null && o.CompletedAt == null);
						break;
					case "已完成":
						query = query.Where(o => o.CompletedAt != null);
						break;
				}
			}

			// 金額
			if (minTotal.HasValue) query = query.Where(o => o.OrderTotal >= minTotal.Value);
			if (maxTotal.HasValue) query = query.Where(o => o.OrderTotal <= maxTotal.Value);

			// === 計數（過濾後） ===
			int recordsFiltered = await query.CountAsync();

			// === 排序 ===
			query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase)
				? query.OrderByDescending(e => EF.Property<object>(e, efSortCol))
				: query.OrderBy(e => EF.Property<object>(e, efSortCol));

			// === 分頁 + 投影 ===
			var data = await query
				.Skip(start)
				.Take(length)
				.Select(o => new
				{
					o.OrderId,
					o.OrderCode,
					o.UserId,
					o.OrderDate,
					o.OrderStatus,
					o.PaymentStatus,
					o.OrderTotal,
					o.PaymentAt,
					o.ShippedAt,
					o.CompletedAt
				})
				.ToListAsync();

			// === 回傳 ===
			return Json(new { draw, recordsTotal, recordsFiltered, data });
		}

		[HttpGet]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();
			var orderInfo = await _dbContext.OrderInfos.FirstOrDefaultAsync(m => m.OrderId == id);
			if (orderInfo == null) return NotFound();
			return View(orderInfo);
		}


		// GET: OnlineStore/OrderInfoes/Create
		public IActionResult Create()
        {
            return View();
        }

        // POST: OnlineStore/OrderInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderCode,UserId,OrderDate,OrderStatus,PaymentStatus,OrderTotal,PaymentAt,ShippedAt,CompletedAt")] OrderInfo orderInfo)
        {
            if (ModelState.IsValid)
            {
				_dbContext.Add(orderInfo);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderInfo);
        }

        // GET: OnlineStore/OrderInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderInfo = await _dbContext.OrderInfos.FindAsync(id);
            if (orderInfo == null)
            {
                return NotFound();
            }
            return View(orderInfo);
        }

        // POST: OnlineStore/OrderInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderCode,UserId,OrderDate,OrderStatus,PaymentStatus,OrderTotal,PaymentAt,ShippedAt,CompletedAt")] OrderInfo orderInfo)
        {
            if (id != orderInfo.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					_dbContext.Update(orderInfo);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderInfoExists(orderInfo.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orderInfo);
        }

        // GET: OnlineStore/OrderInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderInfo = await _dbContext.OrderInfos
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderInfo == null)
            {
                return NotFound();
            }

            return View(orderInfo);
        }

        // POST: OnlineStore/OrderInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderInfo = await _dbContext.OrderInfos.FindAsync(id);
            if (orderInfo != null)
            {
				_dbContext.OrderInfos.Remove(orderInfo);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderInfoExists(int id)
        {
            return _dbContext.OrderInfos.Any(e => e.OrderId == id);
        }
		// 統一用於可空時間欄位的半開區間查詢：[start, end)
		private static IQueryable<OrderInfo> ApplyBetweenNullableDate(
			IQueryable<OrderInfo> q,
			string propertyName,
			DateTime? startInclusive,
			DateTime? endExclusive)
		{
			if (startInclusive.HasValue)
				q = q.Where(o => EF.Property<DateTime?>(o, propertyName) >= startInclusive.Value);
			if (endExclusive.HasValue)
				q = q.Where(o => EF.Property<DateTime?>(o, propertyName) < endExclusive.Value);
			return q;
		}

	}

}
