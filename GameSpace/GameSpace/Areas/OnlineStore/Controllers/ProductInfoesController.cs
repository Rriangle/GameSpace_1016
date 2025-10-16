using GameSpace.Areas.OnlineStore.ViewModels;
using GameSpace.Models;
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.RegularExpressions;
// 其他 using 依原檔保留

namespace GameSpace.Areas.OnlineStore.Controllers
{
    [Area("OnlineStore")]
    public class ProductInfoesController : Controller
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostEnvironment; // ★ 新增：拿 webroot 實體路徑


        public ProductInfoesController(GameSpacedatabaseContext context, IConfiguration config, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _config = config;
            _hostEnvironment = hostEnvironment;
        }
        // ============================================================
        // Index（清單＋篩選）→ 表格模式 (Info only)
        // ============================================================
        [HttpGet]
		public async Task<IActionResult> Index(
			string? keyword, string? type,
			int? qtyMin, int? qtyMax,
			string status = "active",
			DateTime? createdFrom = null, DateTime? createdTo = null,
			string? hasLog = null
		)
		{
			var q = _context.ProductInfos.AsNoTracking().AsQueryable();

			// 關鍵字 / 類別
			if (!string.IsNullOrWhiteSpace(keyword))
				q = q.Where(p => p.ProductName.Contains(keyword) || p.ProductType.Contains(keyword));
			if (!string.IsNullOrWhiteSpace(type))
				q = q.Where(p => p.ProductType == type);

			// 存量
			if (qtyMin.HasValue) q = q.Where(p => p.ShipmentQuantity >= qtyMin);
			if (qtyMax.HasValue) q = q.Where(p => p.ShipmentQuantity <= qtyMax);

			// 狀態
			if (!string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
			{
				bool isActive = status.Equals("active", StringComparison.OrdinalIgnoreCase);
				q = q.Where(p => p.IsActive == isActive);
			}

			// 建立日期
			if (createdFrom.HasValue) q = q.Where(p => p.ProductCreatedAt >= createdFrom.Value);
			if (createdTo.HasValue) q = q.Where(p => p.ProductCreatedAt < createdTo.Value.AddDays(1));

			// 異動紀錄
			if (hasLog == "yes")
				q = q.Where(p => _context.ProductInfoAuditLogs.Any(a => a.ProductId == p.ProductId));
			else if (hasLog == "no")
				q = q.Where(p => !_context.ProductInfoAuditLogs.Any(a => a.ProductId == p.ProductId));

            // 投影成表格用 VM (ProductIndexRowVM)
            var rows = await q
				.OrderByDescending(p => p.ProductId)
				.Select(p => new ProductIndexRowVM
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					ProductType = p.ProductType,
					Price = p.Price,
					CurrencyCode = p.CurrencyCode,
					ShipmentQuantity = p.ShipmentQuantity,
					IsActive = p.IsActive,
					ProductCreatedAt = p.ProductCreatedAt,
					CreatedByManagerId = p.ProductCreatedBy,
					ProductCode = _context.ProductCodes.Where(c => c.ProductId == p.ProductId)
						.Select(c => c.ProductCode1).FirstOrDefault(),
					LastLog = _context.ProductInfoAuditLogs.Where(a => a.ProductId == p.ProductId)
						.OrderByDescending(a => a.ChangedAt)
						.Select(a => new LastLogDto { LogId = a.LogId, ManagerId = a.ManagerId, ChangedAt = a.ChangedAt })
						.FirstOrDefault()
				})
				.ToListAsync();

			// 排序碼（DataTables 用）
			foreach (var r in rows)
			{
				if (!string.IsNullOrWhiteSpace(r.ProductCode))
				{
					var digits = new string(r.ProductCode.SkipWhile(ch => !char.IsDigit(ch)).TakeWhile(char.IsDigit).ToArray());
					if (int.TryParse(digits, out var n)) r.ProductCodeSort = n;
				}
			}

			// 篩選下拉
			ViewBag.TypeList = await _context.ProductInfos.AsNoTracking()
				.Select(p => p.ProductType).Distinct().OrderBy(s => s).ToListAsync();

			// 回填用
			ViewBag.Keyword = keyword;
			ViewBag.Type = type;
			ViewBag.QtyMin = qtyMin;
			ViewBag.QtyMax = qtyMax;
			ViewBag.Status = status;
			ViewBag.CreatedFrom = createdFrom?.ToString("yyyy-MM-dd");
			ViewBag.CreatedTo = createdTo?.ToString("yyyy-MM-dd");
			ViewBag.HasLog = hasLog;

			return View(rows);
		}

//		// ============================================================
//		// Strips（寬條清單，支援完整篩選＋每頁筆數）→ 以商品代碼數字排序 
//		// ============================================================
//		// 寬條清單（穩定版：避免無法翻譯的巢狀 Select）
//		[HttpGet]
//		public async Task<IActionResult> Strips(
//			string? keyword, string? type,
//			string status = "all",
//			int page = 1, int pageSize = 12,
//			int? supplierId = null, int? platformId = null, int? merchTypeId = null,
//			string? gameType = null)
//		{
//			// 1) 基底查詢（只碰 ProductInfo，可順利翻譯）
//			var q = _context.ProductInfos.AsNoTracking().AsQueryable();

//			if (!string.IsNullOrWhiteSpace(keyword))
//				q = q.Where(p => p.ProductName.Contains(keyword) || p.ProductType.Contains(keyword));
//			if (!string.IsNullOrWhiteSpace(type))
//				q = q.Where(p => p.ProductType == type);
//			if (!string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
//			{
//				bool isActive = string.Equals(status, "active", StringComparison.OrdinalIgnoreCase);
//				q = q.Where(p => p.IsActive == isActive);
//			}

//			// 進階條件：用子表 Exists 過濾（仍在單一 Where 層，不在 Select 裡巢）
//			if (supplierId.HasValue)
//				q = q.Where(p =>
//					_context.GameProductDetails.Any(g => g.ProductId == p.ProductId && g.SupplierId == supplierId) ||
//					_context.OtherProductDetails.Any(o => o.ProductId == p.ProductId && o.SupplierId == supplierId));
//			if (platformId.HasValue)
//				q = q.Where(p => _context.GameProductDetails.Any(g => g.ProductId == p.ProductId && g.PlatformId == platformId));
//			if (merchTypeId.HasValue)
//				q = q.Where(p => _context.OtherProductDetails.Any(o => o.ProductId == p.ProductId && o.MerchTypeId == merchTypeId));
//			if (!string.IsNullOrWhiteSpace(gameType))
//				q = q.Where(p => _context.GameProductDetails.Any(g => g.ProductId == p.ProductId && g.GameType.Contains(gameType)));

//			// 2) 先抓「基本欄位」
//			var basics = await q
//				.Select(p => new
//				{
//					p.ProductId,
//					p.ProductName,
//					p.ProductType,
//					p.Price,
//					p.IsActive,
//					p.ShipmentQuantity,
//					p.ProductCreatedAt
//				})
//				.ToListAsync();

//			var ids = basics.Select(b => b.ProductId).ToList();
//			if (ids.Count == 0)
//			{
//				ViewBag.TypeList = await _context.ProductInfos.AsNoTracking()
//					.Select(p => p.ProductType).Distinct().OrderBy(s => s).ToListAsync();
//				ViewBag.SupplierList = await _context.Suppliers.AsNoTracking()
//					.OrderBy(s => s.SupplierName).Select(s => new { s.SupplierId, s.SupplierName }).ToListAsync();
//				ViewBag.PlatformList = await _context.GameProductDetails.AsNoTracking()
//					.Where(g => g.PlatformId != null)
//					.Select(g => new { g.PlatformId, g.PlatformName }).Distinct()
//					.OrderBy(g => g.PlatformName).ToListAsync();
//				ViewBag.MerchTypeList = await _context.MerchTypes.AsNoTracking()
//					.OrderBy(m => m.MerchTypeName).Select(m => new { m.MerchTypeId, m.MerchTypeName }).ToListAsync();

//				ViewBag.Keyword = keyword; ViewBag.Type = type; ViewBag.Status = status;
//				ViewBag.SupplierId = supplierId; ViewBag.PlatformId = platformId;
//				ViewBag.MerchTypeId = merchTypeId; ViewBag.GameType = gameType;
//				ViewBag.Page = page; ViewBag.PageSize = pageSize; ViewBag.Total = 0;

//				return PartialView("_StripsList", Array.Empty<object>());
//			}

//			// 3) 各子表一次性查回來 → 字典
//			var codeMap = await _context.ProductCodes.AsNoTracking()
//				.Where(c => ids.Contains(c.ProductId))
//				.GroupBy(c => c.ProductId)
//				.Select(g => new { ProductId = g.Key, Code = g.Select(x => x.ProductCode1).FirstOrDefault() })
//				.ToDictionaryAsync(x => x.ProductId, x => x.Code);

//			var lastChangedMap = await _context.ProductInfoAuditLogs.AsNoTracking()
//				.Where(a => ids.Contains(a.ProductId))
//				.GroupBy(a => a.ProductId)
//				.Select(g => new { ProductId = g.Key, ChangedAt = g.Max(a => a.ChangedAt) })
//				.ToDictionaryAsync(x => x.ProductId, x => (DateTime?)x.ChangedAt);

//			// 最新一張圖（用 GroupBy + FirstOrDefault 可翻譯）
//			var imgMap = await _context.ProductImages.AsNoTracking()
//				.Where(i => ids.Contains(i.ProductId))
//				.GroupBy(i => i.ProductId)
//				.Select(g => new
//				{
//					ProductId = g.Key,
//					Url = g.OrderByDescending(i => i.ProductimgId).Select(i => i.ProductimgUrl).FirstOrDefault()
//				})
//				.ToDictionaryAsync(x => x.ProductId, x => x.Url);

//			// 供應商（Game + Other 合併）
//			var gameSup = await (from d in _context.GameProductDetails.AsNoTracking()
//								 join s in _context.Suppliers.AsNoTracking()
//								   on d.SupplierId equals s.SupplierId
//								 where ids.Contains(d.ProductId)
//								 select new { d.ProductId, s.SupplierName }).ToListAsync();
//			var otherSup = await (from d in _context.OtherProductDetails.AsNoTracking()
//								  join s in _context.Suppliers.AsNoTracking()
//									on d.SupplierId equals s.SupplierId
//								  where ids.Contains(d.ProductId)
//								  select new { d.ProductId, s.SupplierName }).ToListAsync();
//			var supMap = gameSup.Concat(otherSup)
//				.GroupBy(x => x.ProductId)
//				.ToDictionary(g => g.Key, g => g.Select(x => x.SupplierName).FirstOrDefault());

//			// 平台 / 遊戲類型（只針對 game）
//			var gameMeta = await _context.GameProductDetails.AsNoTracking()
//				.Where(g => ids.Contains(g.ProductId))
//				.GroupBy(g => g.ProductId)
//				.Select(g => new
//				{
//					ProductId = g.Key,
//					PlatformName = g.Select(x => x.PlatformName).FirstOrDefault(),
//					GameType = g.Select(x => x.GameType).FirstOrDefault()
//				})
//				.ToListAsync();
//			var platMap = gameMeta.ToDictionary(x => x.ProductId, x => x.PlatformName);
//			var gameTypeMap = gameMeta.ToDictionary(x => x.ProductId, x => x.GameType);

//			// 周邊分類名稱（只針對 notgame）
//			var otherMeta = await (from d in _context.OtherProductDetails.AsNoTracking()
//								   join m in _context.MerchTypes.AsNoTracking()
//									 on d.MerchTypeId equals m.MerchTypeId
//								   where ids.Contains(d.ProductId)
//								   select new { d.ProductId, m.MerchTypeName }).ToListAsync();
//			var merchMap = otherMeta.GroupBy(x => x.ProductId)
//				.ToDictionary(g => g.Key, g => g.Select(x => x.MerchTypeName).FirstOrDefault());

//			// 4) 合併到 ViewModel（避免在 Select 裡做子查詢）
//			var list = basics.Select(b => new
//			{
//				b.ProductId,
//				b.ProductName,
//				b.ProductType,
//				b.Price,
//				b.IsActive,
//				b.ShipmentQuantity,
//				b.ProductCreatedAt,
//				ProductCode = codeMap.GetValueOrDefault(b.ProductId),
//				LastChangedAt = lastChangedMap.GetValueOrDefault(b.ProductId),
//				ImageUrl = imgMap.GetValueOrDefault(b.ProductId),
//				SupplierName = supMap.GetValueOrDefault(b.ProductId),
//				PlatformName = platMap.GetValueOrDefault(b.ProductId),
//				GameType = gameTypeMap.GetValueOrDefault(b.ProductId),
//				MerchTypeName = merchMap.GetValueOrDefault(b.ProductId)
//			}).ToList();

//			// 5) 商品代碼數字排序（無代碼者最後）
//			list = list.OrderBy(x =>
//			{
//				if (string.IsNullOrWhiteSpace(x.ProductCode)) return int.MaxValue;
//				var s = new string(x.ProductCode.Trim().SkipWhile(ch => !char.IsDigit(ch)).TakeWhile(char.IsDigit).ToArray());
//				return int.TryParse(s, out var n) ? n : int.MaxValue;
//			}).ToList();

//			// 6) 分頁
//			var total = list.Count;
//			page = Math.Max(1, page);
//			pageSize = Math.Max(1, pageSize);
//			var pageList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

//			// 下拉來源 + 查詢字串回填
//			ViewBag.TypeList = await _context.ProductInfos.AsNoTracking()
//				.Select(p => p.ProductType).Distinct().OrderBy(s => s).ToListAsync();
//			ViewBag.SupplierList = await _context.Suppliers.AsNoTracking()
//				.OrderBy(s => s.SupplierName).Select(s => new { s.SupplierId, s.SupplierName }).ToListAsync();
//			ViewBag.PlatformList = await _context.GameProductDetails.AsNoTracking()
//				.Where(g => g.PlatformId != null)
//				.Select(g => new { g.PlatformId, g.PlatformName }).Distinct()
//				.OrderBy(g => g.PlatformName).ToListAsync();
//			ViewBag.MerchTypeList = await _context.MerchTypes.AsNoTracking()
//				.OrderBy(m => m.MerchTypeName).Select(m => new { m.MerchTypeId, m.MerchTypeName }).ToListAsync();

//			ViewBag.Keyword = keyword; ViewBag.Type = type; ViewBag.Status = status;
//			ViewBag.SupplierId = supplierId; ViewBag.PlatformId = platformId;
//			ViewBag.MerchTypeId = merchTypeId; ViewBag.GameType = gameType;
//			ViewBag.Page = page; ViewBag.PageSize = pageSize; ViewBag.Total = total;

//			return PartialView("_StripsList", pageList);
//		}

//        // 儲存目錄（相對於 wwwroot）
//        private const string ProductImagesRelFolder = "/images/products";

//        // 目錄（實體路徑）
//        private string ProductImagesAbsFolder =>
//            Path.Combine(_hostEnvironment.WebRootPath, "images", "products");

//        // 允許的副檔名（可依需求增減）
//        private static readonly HashSet<string> _allowedExts = new(StringComparer.OrdinalIgnoreCase)
//{ ".jpg", ".jpeg", ".png", ".gif", ".webp" };

//        // 產生安全檔名（避免原始檔名注入/碰撞）
//        private static string MakeSafeFileName(string? prefer, string ext)
//        {
//            var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
//            var guid = Guid.NewGuid().ToString("N");
//            var baseName = string.IsNullOrWhiteSpace(prefer)
//                ? "img"
//                : Regex.Replace(prefer, @"[^a-zA-Z0-9\-_]+", "-"); // 簡單 slug
//            baseName = baseName.Trim('-');
//            if (baseName.Length > 40) baseName = baseName.Substring(0, 40);
//            return $"{baseName}_{stamp}_{guid}{ext}";
//        }

//        // 實際存檔：回傳【相對路徑】清單（/images/products/xxx.ext）
//        private async Task<List<string>> SaveUploadedFilesAsync(IEnumerable<IFormFile> files, string? preferNameForSlug)
//        {
//            Directory.CreateDirectory(ProductImagesAbsFolder);
//            var relPaths = new List<string>();

//            foreach (var f in files)
//            {
//                if (f == null || f.Length == 0) continue;

//                var ext = Path.GetExtension(f.FileName);
//                if (string.IsNullOrWhiteSpace(ext) || !_allowedExts.Contains(ext))
//                    continue; // 直接忽略不合規檔案

//                var fname = MakeSafeFileName(preferNameForSlug, ext);
//                var abs = Path.Combine(ProductImagesAbsFolder, fname);
//                using (var fs = System.IO.File.Create(abs))
//                {
//                    await f.CopyToAsync(fs);
//                }
//                relPaths.Add($"{ProductImagesRelFolder}/{fname}".Replace('\\', '/'));
//            }
//            return relPaths;
//        }

//        // 刪除實體檔（只有在無其他紀錄引用同一路徑時才刪）
//        private async Task TryDeletePhysicalFileIfNoReferenceAsync(string relPath)
//        {
//            if (string.IsNullOrWhiteSpace(relPath)) return;

//            var stillRef = await _context.ProductImages
//                .AsNoTracking()
//                .AnyAsync(x => x.ProductimgUrl == relPath);
//            if (stillRef) return; // 還有人用就不刪

//            // 限制只能刪 wwwroot/images/products 下的檔
//            if (!relPath.StartsWith(ProductImagesRelFolder, StringComparison.OrdinalIgnoreCase)) return;

//            var abs = Path.Combine(_hostEnvironment.WebRootPath, relPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
//            if (System.IO.File.Exists(abs))
//            {
//                try { System.IO.File.Delete(abs); } catch { /* ignore */ }
//            }
        //}

        // ============================================================
        // Detail（Modal / Panel）— 共用 Builder
        // ============================================================
        [HttpGet]
		public async Task<IActionResult> DetailModal(int id)
		{
			var vm = await BuildDetailVM(id);
			if (vm == null) return NotFound();
			return PartialView("_DetailModal", vm);
		}

		[HttpGet]
		public async Task<IActionResult> DetailPanel(int id)
		{
			var vm = await BuildDetailVM(id);
			if (vm == null) return NotFound();
			return PartialView("_DetailPanel", vm);
		}

		private async Task<ProductDetailModalVM?> BuildDetailVM(int id)
		{
			var p = await _context.ProductInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);
			if (p == null) return null;

			var vm = new ProductDetailModalVM
			{
				ProductId = p.ProductId,
				ProductName = p.ProductName,
				ProductType = p.ProductType,
				IsActive = p.IsActive,
				Price = p.Price,
				CurrencyCode = p.CurrencyCode ?? "TWD",
				ShipmentQuantity = p.ShipmentQuantity,
				ProductCreatedAt = p.ProductCreatedAt,
				ProductCreatedBy = p.ProductCreatedBy,
				ProductUpdatedAt = p.ProductUpdatedAt,
				ProductUpdatedBy = p.ProductUpdatedBy,
				ProductCode = await _context.ProductCodes.Where(c => c.ProductId == id).Select(c => c.ProductCode1).FirstOrDefaultAsync(),
				Images = await _context.ProductImages.AsNoTracking()
					.Where(i => i.ProductId == id)
					.OrderByDescending(i => i.ProductimgId)
					.Select(i => new ProductDetailModalVM.ImageVM { Id = i.ProductimgId, Url = i.ProductimgUrl, Alt = i.ProductimgAltText })
					.ToListAsync()
			};

			if (string.Equals(p.ProductType, "game", StringComparison.OrdinalIgnoreCase))
			{
				var d = await _context.GameProductDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);
				if (d != null)
				{
					vm.SupplierId = d.SupplierId;
					vm.SupplierName = await _context.Suppliers.Where(s => s.SupplierId == d.SupplierId).Select(s => s.SupplierName).FirstOrDefaultAsync();
					vm.PlatformId = d.PlatformId;
					vm.PlatformName = d.PlatformName;
					vm.GameType = d.GameType;
					vm.DownloadLink = d.DownloadLink;
					vm.GameDescription = d.ProductDescription;
				}
			}
			else
			{
				var d = await _context.OtherProductDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);
				if (d != null)
				{
					vm.SupplierId = d.SupplierId;
					vm.SupplierName = await _context.Suppliers.Where(s => s.SupplierId == d.SupplierId).Select(s => s.SupplierName).FirstOrDefaultAsync();
					vm.MerchTypeId = d.MerchTypeId;
					vm.MerchTypeName = await _context.MerchTypes.Where(m => m.MerchTypeId == d.MerchTypeId).Select(m => m.MerchTypeName).FirstOrDefaultAsync();
					vm.DigitalCode = d.DigitalCode;
					vm.Size = d.Size;
					vm.Color = d.Color;
					vm.Weight = d.Weight;
					vm.Dimensions = d.Dimensions;
					vm.Material = d.Material;
					vm.StockQuantity = d.StockQuantity;
					vm.OtherDescription = d.ProductDescription;
				}
			}

			vm.LastLogs = await _context.ProductInfoAuditLogs.AsNoTracking()
				.Where(a => a.ProductId == id)
				.OrderByDescending(a => a.ChangedAt)
				.Select(a => new ProductDetailModalVM.LastLogVM
				{
					LogId = (long)a.LogId,
					ActionType = a.ActionType,
					FieldName = a.FieldName,
					OldValue = a.OldValue,
					NewValue = a.NewValue,
					ManagerId = a.ManagerId,
					ChangedAt = a.ChangedAt
				})
				.ToListAsync();

			return vm;
		}

		// 小工具：將 productType 正規化
		private static string NormalizeType(string? type)
		{
			if (string.IsNullOrWhiteSpace(type)) return "";
			var t = type.Trim().ToLowerInvariant();
			if (t is "game") return "game";
			if (t is  "notgame") return "notgame";   // 「notgame」作為唯一標準
			return "";
		}
		private static bool IsGameType(string? t) => NormalizeType(t) == "game";
		private static bool IsOtherType(string? t) => NormalizeType(t) == "notgame";

		// 把不屬於該類型的欄位清空（避免跨區欄位被送進來）
		private void GateFieldsByType(ProductInfoFormVM vm)
		{
			if (IsGameType(vm.ProductType))
			{
				// Non-Game 欄位一律忽略
				vm.MerchTypeId = null;
				vm.DigitalCode = null;
				vm.Size = vm.Color = vm.Weight = vm.Dimensions = vm.Material = null;
				// 庫存（Detail）交由 Info 的 ShipmentQuantity 管
				// vm.StockQuantity = null; // 若你希望完全由 Info 管控就清掉
			}
			else if (IsOtherType(vm.ProductType))
			{
				// Game 欄位一律忽略
				vm.PlatformId = null;
				vm.PlatformName = null;
				vm.GameType = null;
				vm.DownloadLink = null;
				vm.GameProductDescription = null;
			}
		}

		// ============================================================
		// Create（Modal）
		// ============================================================
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			await FillDropdownsAsync();
			ViewBag.Mode = "create";
			// 預設未選類別 → 兩區都上鎖
			ViewBag.EnableGame = false;
			ViewBag.EnableOther = false;

			var vm = new ProductInfoFormVM
			{
				CurrencyCode = "TWD",
				IsActive = true,
				ProductType = null // 讓畫面顯示「請先選擇類別」
			};
			return PartialView("_CreateEditModal", vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductInfoFormVM vm)
		{
			var type = NormalizeType(vm.ProductType); // "game" / "notgame" / ""
			ViewBag.Mode = "create";
			ViewBag.EnableGame = type == "game";
			ViewBag.EnableOther = type == "notgame";
			await FillDropdownsAsync();

			// --- 條件式驗證 ---
			if (string.IsNullOrWhiteSpace(type))
				ModelState.AddModelError(nameof(vm.ProductType), "請先選擇類別");

			if (type == "game")
			{
				// 移除原本的：平台為必填
				// if (string.IsNullOrWhiteSpace(vm.PlatformName) && vm.PlatformId == null)
				//     ModelState.AddModelError(nameof(vm.PlatformName), "平台為必填");

				if (string.IsNullOrWhiteSpace(vm.GameType))
					ModelState.AddModelError(nameof(vm.GameType), "遊戲類型為必填");
			}
			else if (type == "notgame")
			{
				if (!vm.MerchTypeId.HasValue)
					ModelState.AddModelError(nameof(vm.MerchTypeId), "商品種類為必選");
				if (!vm.StockQuantity.HasValue)
					ModelState.AddModelError(nameof(vm.StockQuantity), "庫存數量為必填");
				else if (vm.StockQuantity.Value < 0)
					ModelState.AddModelError(nameof(vm.StockQuantity), "庫存數量不可小於 0");
			}

			// 收集錯誤給 View
			var allErrors = ModelState
				.Where(ms => ms.Value.Errors.Any())
				.Select(ms => new
				{
					Key = ms.Key,
					Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
				}).ToList();
			ViewBag.ModelErrors = allErrors;

			if (!ModelState.IsValid)
				return PartialView("_CreateEditModal", vm);

			try
			{
				// === 1) 建 Info ===
				var entity = new ProductInfo();
				ApplyFromVM(entity, vm);
				entity.ProductType = type;
				entity.ProductCreatedBy = GetCurrentManagerId();
				entity.ProductCreatedAt = DateTime.Now;

				_context.ProductInfos.Add(entity);
				await _context.SaveChangesAsync(); // 取 ProductId

				// === 2) 建 Detail ===
				if (type == "game")
				{
					_context.GameProductDetails.Add(new GameProductDetail
					{
						ProductId = entity.ProductId,
						SupplierId = vm.SupplierId!.Value,
						PlatformId = vm.PlatformId,       // 可以是 null
						PlatformName = vm.PlatformName,
						GameType = vm.GameType,
						DownloadLink = vm.DownloadLink,
						ProductDescription = vm.GameProductDescription,
						IsActive = vm.IsActive
					});
				}
				else if (type == "notgame")
				{
					_context.OtherProductDetails.Add(new OtherProductDetail
					{
						ProductId = entity.ProductId,
						SupplierId = vm.SupplierId!.Value,
						MerchTypeId = vm.MerchTypeId,
						DigitalCode = vm.DigitalCode,
						Size = vm.Size,
						Color = vm.Color,
						Weight = vm.Weight,
						Dimensions = vm.Dimensions,
						Material = vm.Material,
						StockQuantity = vm.StockQuantity ?? 0,
						ProductDescription = vm.OtherProductDescription,
						IsActive = vm.IsActive
					});
				}

				// === 3) 產生 ProductCode ===
				var code = await GenerateProductCodeAsync(type);
				_context.ProductCodes.Add(new ProductCode
				{
					ProductId = entity.ProductId,
					ProductCode1 = code
				});

				// 圖片上傳 & Audit Log & 存檔等照原本邏輯...

				await _context.SaveChangesAsync();

				return Json(new { ok = true, msg = $"「{entity.ProductName}」已新增！" });
			}
			catch (SqlException ex)
			{
				return Json(new { ok = false, msg = $"新增失敗：{ex.Message}" });
			}
		}



		// ============================================================
		// Edit（Modal）
		// ============================================================
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var p = await _context.ProductInfos.FindAsync(id);
			if (p == null) return NotFound();

			await FillDropdownsAsync();
			ViewBag.Mode = "edit";
			return PartialView("_CreateEditModal", MapToFormVM(p));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ProductInfoFormVM vm)
		{
			// 先決定類別與灰階旗標（回填畫面要用）
			var type = NormalizeType(vm.ProductType); // "game" / "notgame" / ""
			vm.ProductType = type;                    // 標準化回 VM，避免 ApplyFromVM 帶入不一致
			ViewBag.Mode = "edit";
			ViewBag.EnableGame = type == "game";
			ViewBag.EnableOther = type == "notgame";

			await FillDropdownsAsync(); // 若需回傳部分檢核頁面

			if (!ModelState.IsValid)
				return PartialView("_CreateEditModal", vm);

			var p = await _context.ProductInfos.FindAsync(vm.ProductId);
			if (p == null) return NotFound();

			if (!vm.SupplierId.HasValue)
				return Json(new { ok = false, msg = "請選擇供應商。" });

			// 依類別必填檢核
			if (type == "game")
			{
				if (!vm.PlatformId.HasValue)
					return Json(new { ok = false, msg = "請選擇平台（PlatformId）。" });

				if (string.IsNullOrWhiteSpace(vm.PlatformName))
					vm.PlatformName = await LookupPlatformNameById(vm.PlatformId.Value);
			}
			else if (type == "notgame")
			{
				if (!vm.MerchTypeId.HasValue)
					return Json(new { ok = false, msg = "非遊戲類需選擇周邊分類。" });
			}
			else
			{
				return Json(new { ok = false, msg = "請先選擇類別（game / notgame）。" });
			}

			// 變更前快照（log 用）
			var old = new { p.ProductName, p.Price, p.ShipmentQuantity, p.IsActive, p.ProductType };

			// 套用 Info
			ApplyFromVM(p, vm);            // 會寫入名稱/類別/價格/幣別/存量/IsActive
			p.ProductType = type;      // 再次保險：類別用標準化後
			p.ProductUpdatedBy = GetCurrentManagerId();
			p.ProductUpdatedAt = DateTime.Now;

			// 類別切換：刪舊建新
			if (!string.Equals(old.ProductType, type, StringComparison.OrdinalIgnoreCase))
			{
				if (string.Equals(old.ProductType, "game", StringComparison.OrdinalIgnoreCase))
				{
					var g = await _context.GameProductDetails.FirstOrDefaultAsync(x => x.ProductId == p.ProductId);
					if (g != null) _context.GameProductDetails.Remove(g);
				}
				else
				{
					var o = await _context.OtherProductDetails.FirstOrDefaultAsync(x => x.ProductId == p.ProductId);
					if (o != null) _context.OtherProductDetails.Remove(o);
				}

				if (type == "game")
				{
					_context.GameProductDetails.Add(new GameProductDetail
					{
						ProductId = p.ProductId,
						SupplierId = vm.SupplierId!.Value,
						PlatformId = vm.PlatformId,
						PlatformName = vm.PlatformName,
						GameType = vm.GameType,
						DownloadLink = vm.DownloadLink,
						ProductDescription = vm.GameProductDescription,
						IsActive = vm.IsActive
					});
				}
				else // notgame
				{
					_context.OtherProductDetails.Add(new OtherProductDetail
					{
						ProductId = p.ProductId,
						SupplierId = vm.SupplierId!.Value,
						MerchTypeId = vm.MerchTypeId,
						DigitalCode = vm.DigitalCode,
						Size = vm.Size,
						Color = vm.Color,
						Weight = vm.Weight,
						Dimensions = vm.Dimensions,
						Material = vm.Material,
						StockQuantity = vm.StockQuantity ?? 0,
						ProductDescription = vm.OtherProductDescription,
						IsActive = vm.IsActive
					});
				}
			}
			else
			{
				// 類別未變：更新 Detail
				if (type == "game")
				{
					var d = await _context.GameProductDetails.FirstOrDefaultAsync(x => x.ProductId == p.ProductId);
					if (d == null) { d = new GameProductDetail { ProductId = p.ProductId }; _context.GameProductDetails.Add(d); }
					d.SupplierId = vm.SupplierId!.Value;
					d.PlatformId = vm.PlatformId;
					d.PlatformName = vm.PlatformName;
					d.GameType = vm.GameType;
					d.DownloadLink = vm.DownloadLink;
					d.ProductDescription = vm.GameProductDescription;
					d.IsActive = vm.IsActive;
				}
				else // notgame
				{
					var d = await _context.OtherProductDetails.FirstOrDefaultAsync(x => x.ProductId == p.ProductId);
					if (d == null) { d = new OtherProductDetail { ProductId = p.ProductId }; _context.OtherProductDetails.Add(d); }
					d.SupplierId = vm.SupplierId!.Value;
					d.MerchTypeId = vm.MerchTypeId;
					d.DigitalCode = vm.DigitalCode;
					d.Size = vm.Size;
					d.Color = vm.Color;
					d.Weight = vm.Weight;
					d.Dimensions = vm.Dimensions;
					d.Material = vm.Material;
					d.StockQuantity = vm.StockQuantity ?? d.StockQuantity;
					d.ProductDescription = vm.OtherProductDescription;
					d.IsActive = vm.IsActive;
				}
			}

			//// 刪除舊圖（勾選 Remove 的）
			//if (vm.ExistingImages is { Count: > 0 })
			//{
			//	var idsToRemove = vm.ExistingImages.Where(x => x.Remove).Select(x => x.ImageId).ToList();
			//	if (idsToRemove.Count > 0)
			//	{
			//		var imgs = await _context.ProductImages
			//			.Where(i => i.ProductId == p.ProductId && idsToRemove.Contains(i.ProductimgId))
			//			.ToListAsync();

			//		foreach (var img in imgs)
			//		{
			//			_context.ProductImages.Remove(img);
			//			_context.ProductInfoAuditLogs.Add(new ProductInfoAuditLog
			//			{
			//				ProductId = p.ProductId,
			//				ActionType = "UPDATE",
			//				FieldName = "image:remove",
			//				OldValue = img.ProductimgUrl,
			//				NewValue = null,
			//				ManagerId = p.ProductUpdatedBy,
			//				ChangedAt = DateTime.Now
			//			});
			//		}
			//	}
			//}

			// 新圖：
			// 1) 前端傳來的外部連結（NewImageUrls）
			// 2) 若也有本地檔案，直接上傳
			// IIamge
			

			// 若此商品尚未有 ProductCode，補一筆（不重發新代碼）
			var hasCode = await _context.ProductCodes.AsNoTracking().AnyAsync(c => c.ProductId == p.ProductId);
			if (!hasCode)
			{
				var code = await GenerateProductCodeAsync(type);
				_context.ProductCodes.Add(new ProductCode { ProductId = p.ProductId, ProductCode1 = code });
			}

			// 一次存檔
			await _context.SaveChangesAsync();


			// Log（概述）
			var log = new ProductInfoAuditLog
			{
				ProductId = p.ProductId,
				ActionType = "UPDATE",
				FieldName = "(mixed)",
				OldValue = $"Name={old.ProductName}, Price={old.Price}, Qty={old.ShipmentQuantity}, Active={old.IsActive}, Type={old.ProductType}",
				NewValue = $"Name={p.ProductName}, Price={p.Price}, Qty={p.ShipmentQuantity}, Active={p.IsActive}, Type={p.ProductType}",
				ManagerId = p.ProductUpdatedBy,
				ChangedAt = DateTime.Now
			};
			_context.ProductInfoAuditLogs.Add(log);
			await _context.SaveChangesAsync();

			return Json(new
			{
				ok = true,
				msg = $"「{p.ProductName}」的商品資訊已修改完成！",
				updated = new
				{
					id = p.ProductId,
					name = p.ProductName,
					type = p.ProductType,
					priceN0 = p.Price.ToString("N0"),
					qty = p.ShipmentQuantity,
					active = p.IsActive,
					lastChangedText = log.ChangedAt.ToString("yyyy/MM/dd tt hh:mm"),
					lastChangedRaw = log.ChangedAt.ToString("yyyy/MM/dd HH:mm:ss"),
					lastManagerId = log.ManagerId
				}
			});
		}

		//=======================
		// Delete（軟刪）+ Deactivate（寬版 AJAX 用）
		// ============================================================
		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			var p = await _context.ProductInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);
			if (p == null) return NotFound();

			var vm = new ProductInfoFormVM { ProductId = p.ProductId, ProductName = p.ProductName, ProductType = p.ProductType, Price = p.Price, IsActive = p.IsActive };
			return PartialView("_DeleteModal", vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed([FromForm(Name = "productId")] int id)
		{
			return await Deactivate(id); // 共用
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Deactivate(int id)
		{
			var p = await _context.ProductInfos.FindAsync(id);
			if (p == null) return NotFound();

			var oldActive = p.IsActive;
			p.IsActive = false;
			p.ProductUpdatedBy = GetCurrentManagerId();
			p.ProductUpdatedAt = DateTime.Now;

			// 連動 Detail
			if (p.ProductType == "game")
			{
				var g = await _context.GameProductDetails.FirstOrDefaultAsync(x => x.ProductId == id);
				if (g != null) g.IsActive = false;
			}
			else
			{
				var o = await _context.OtherProductDetails.FirstOrDefaultAsync(x => x.ProductId == id);
				if (o != null) o.IsActive = false;
			}

			await _context.SaveChangesAsync();

			_context.ProductInfoAuditLogs.Add(new ProductInfoAuditLog
			{
				ProductId = p.ProductId,
				ActionType = "UPDATE",
				FieldName = "is_active",
				OldValue = oldActive ? "1" : "0",
				NewValue = "0",
				ManagerId = p.ProductUpdatedBy,
				ChangedAt = DateTime.Now
			});
			await _context.SaveChangesAsync();

			return Json(new { ok = true, msg = $"「{p.ProductName}」已下架（軟刪除）。", id = p.ProductId, active = false, softDeleted = true });
		}

		// ============================================================
		// AuditLog（Modal）
		// ============================================================
		[HttpGet]
		public async Task<IActionResult> AuditLog(int id)
		{
			var logs = await _context.ProductInfoAuditLogs
				.Where(a => a.ProductId == id)
				.OrderByDescending(a => a.ChangedAt)
				.Select(a => new ProductInfoAuditLogRowVM
				{
					LogId = a.LogId,
					ActionType = a.ActionType,
					FieldName = a.FieldName,
					OldValue = a.OldValue,
					NewValue = a.NewValue,
					ManagerId = a.ManagerId,
					ChangedAt = a.ChangedAt
				})
				.ToListAsync();

			ViewBag.ProductId = id;
			var p = await _context.ProductInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);
			ViewBag.ProductName = p?.ProductName ?? $"#{id}";
			return PartialView("_AuditLogModal", logs);
		}
		[HttpGet]
		public async Task<IActionResult> GameTypeHints(string? q = null, int take = 20)
		{
			q = (q ?? "").Trim();
			var data = await _context.GameProductDetails.AsNoTracking()
				.Where(g => !string.IsNullOrEmpty(g.GameType) && (q == "" || g.GameType.Contains(q)))
				.GroupBy(g => g.GameType!)
				.OrderByDescending(g => g.Count())
				.Take(take)
				.Select(g => g.Key)
				.ToListAsync();

			return Json(new { ok = true, items = data });
		}
		private async Task<string> GenerateProductCodeAsync(string? productType)
{
    var isGame   = string.Equals(productType, "game",   StringComparison.OrdinalIgnoreCase);
    var isNotGame =  string.Equals(productType, "notgame", StringComparison.OrdinalIgnoreCase);

    var prefix = isGame ? "GM" : (isNotGame ? "OT" : "XX");

    // 直接用字串倒序抓最大碼：GMxxxxxxxxxx / OTxxxxxxxxxx
    // 因為尾碼固定長度 D10，所以字串排序 == 數值排序
    var last = await _context.ProductCodes
        .AsNoTracking()
        .Where(c => c.ProductCode1 != null && c.ProductCode1.StartsWith(prefix))
        .OrderByDescending(c => c.ProductCode1)   // 例如 GM0000000123 會排在 GM0000000010 前面
        .Select(c => c.ProductCode1)
        .FirstOrDefaultAsync();

    long next = 1;
    if (!string.IsNullOrEmpty(last) && last.Length >= 12)
    {
        var tail = last.Substring(2); // 取後 10 碼
        if (long.TryParse(tail, out var n))
            next = n + 1;
    }

    // 併發保險：最多重試 5 次，每次+1
    for (int i = 0; i < 5; i++)
    {
        var candidate = prefix + next.ToString("D10"); // 固定長度 10 碼
        var exists = await _context.ProductCodes
            .AsNoTracking()
            .AnyAsync(x => x.ProductCode1 == candidate);

        if (!exists)
            return candidate;

        next++;
    }

    // 極端 fallback（基本用不到）
    return prefix + DateTime.UtcNow.Ticks.ToString().PadLeft(10, '0').Substring(0, 10);
}



		// ============================================================
		// ToggleActive（Info/Detail 連動）
		// ============================================================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ToggleActive(int id)
		{
			var p = await _context.ProductInfos.FindAsync(id);
			if (p == null) return NotFound();

			var oldActive = p.IsActive;
			p.IsActive = !p.IsActive;
			p.ProductUpdatedBy = GetCurrentManagerId();
			p.ProductUpdatedAt = DateTime.Now;

			if (p.ProductType == "game")
			{
				var g = await _context.GameProductDetails.FirstOrDefaultAsync(x => x.ProductId == id);
				if (g != null) g.IsActive = p.IsActive;
			}
			else
			{
				var o = await _context.OtherProductDetails.FirstOrDefaultAsync(x => x.ProductId == id);
				if (o != null) o.IsActive = p.IsActive;
			}

			await _context.SaveChangesAsync();

			_context.ProductInfoAuditLogs.Add(new ProductInfoAuditLog
			{
				ProductId = p.ProductId,
				ActionType = "UPDATE",
				FieldName = "is_active",
				OldValue = oldActive ? "1" : "0",
				NewValue = p.IsActive ? "1" : "0",
				ManagerId = p.ProductUpdatedBy,
				ChangedAt = DateTime.Now
			});
			await _context.SaveChangesAsync();

			var msg = $"「{p.ProductName}」已{(p.IsActive ? "上架" : "下架")}。";
			return Json(new { ok = true, msg, active = p.IsActive });
		}


		// ============================================================
		// Helpers
		// ============================================================
		private int GetCurrentManagerId() => 30000060; // ★ 請確定 DB 真的有這個 ManagerData

		private void ApplyFromVM(ProductInfo e, ProductInfoFormVM vm)
		{
			e.ProductName = vm.ProductName.Trim();
			e.ProductType = vm.ProductType;   // "game"/"notgame"
			e.Price = vm.Price;
			e.CurrencyCode = vm.CurrencyCode;
			e.ShipmentQuantity = vm.ShipmentQuantity;
			e.IsActive = vm.IsActive;
		}

		private ProductInfoFormVM MapToFormVM(ProductInfo e)
		{
			var vm = new ProductInfoFormVM
			{
				ProductId = e.ProductId,
				ProductName = e.ProductName,
				ProductType = e.ProductType,
				Price = e.Price,
				CurrencyCode = e.CurrencyCode,
				ShipmentQuantity = e.ShipmentQuantity,
				IsActive = e.IsActive,
				ProductCreatedAt = e.ProductCreatedAt,
				ProductCreatedBy = e.ProductCreatedBy,
				ProductUpdatedAt = e.ProductUpdatedAt,
				ProductUpdatedBy = e.ProductUpdatedBy,
				

			};

			// 第一張圖片（若有）
			var img = _context.ProductImages
				.AsNoTracking()
				.Where(i => i.ProductId == e.ProductId)
				.OrderBy(i => i.ProductimgId)
				.FirstOrDefault();

			if (img != null)
			{
				vm.ProductImageUrl = img.ProductimgUrl; // Partial 會顯示縮圖
			}

			// 詳細資料（依類別）
			if (vm.ProductType == "game")
			{
				var d = _context.GameProductDetails.AsNoTracking()
						 .FirstOrDefault(x => x.ProductId == e.ProductId);
				if (d != null)
				{
					vm.PlatformId = d.PlatformId;
					vm.PlatformName = d.PlatformName;
					vm.GameType = d.GameType;
					vm.DownloadLink = d.DownloadLink;
					vm.GameProductDescription = d.ProductDescription;
				}
			}
			else if (vm.ProductType == "notgame")
			{
				var d = _context.OtherProductDetails.AsNoTracking()
						 .FirstOrDefault(x => x.ProductId == e.ProductId);
				if (d != null)
				{
					vm.MerchTypeId = d.MerchTypeId;
					vm.DigitalCode = d.DigitalCode;
					vm.Size = d.Size;
					vm.Color = d.Color;
					vm.Weight = d.Weight;
					vm.Dimensions = d.Dimensions;
					vm.Material = d.Material;
					vm.StockQuantity = d.StockQuantity;
					vm.OtherProductDescription = d.ProductDescription;
				}
			}

			return vm;
		}

		// 依 PlatformId 補平台名稱（從歷史資料找一筆即可）
		private async Task<string?> LookupPlatformNameById(int platformId)
		{
			return await _context.GameProductDetails.AsNoTracking()
				.Where(g => g.PlatformId == platformId && g.PlatformName != null && g.PlatformName != "")
				.OrderByDescending(g => g.ProductId)
				.Select(g => g.PlatformName)
				.FirstOrDefaultAsync();
		}

		

		// Create/Edit 用下拉/參考清單
		// 單一版本：請保留這個，刪除你檔案中其它重複命名的 FillDropdownsAsync()
		private async Task FillDropdownsAsync()
		{
			// 供應商
			ViewBag.SupplierList = await _context.Suppliers.AsNoTracking()
				.OrderBy(s => s.SupplierName)
				.Select(s => new { s.SupplierId, s.SupplierName })
				.ToListAsync();

			// 平台（目前先從歷史 GameDetail 取得）
			ViewBag.PlatformList = await _context.GameProductDetails.AsNoTracking()
				.Where(g => g.PlatformId != null)
				.GroupBy(g => new { g.PlatformId, g.PlatformName })
				.Select(g => new { g.Key.PlatformId, g.Key.PlatformName })
				.OrderBy(g => g.PlatformName)
				.ToListAsync();

			// Non-Game 周邊分類
			ViewBag.MerchTypeList = await _context.MerchTypes.AsNoTracking()
				.OrderBy(m => m.MerchTypeName)
				.Select(m => new { m.MerchTypeId, m.MerchTypeName })
				.ToListAsync();

			// ★ Non-Game 歷史參考（Top 20，常用優先）
			ViewBag.SizeList = await _context.OtherProductDetails.AsNoTracking()
				.Where(x => x.Size != null && x.Size != "")
				.GroupBy(x => x.Size).OrderByDescending(g => g.Count())
				.Select(g => g.Key).Take(20).ToListAsync();

			ViewBag.ColorList = await _context.OtherProductDetails.AsNoTracking()
				.Where(x => x.Color != null && x.Color != "")
				.GroupBy(x => x.Color).OrderByDescending(g => g.Count())
				.Select(g => g.Key).Take(20).ToListAsync();

			ViewBag.WeightList = await _context.OtherProductDetails.AsNoTracking()
				.Where(x => x.Weight != null && x.Weight != "")
				.GroupBy(x => x.Weight).OrderByDescending(g => g.Count())
				.Select(g => g.Key).Take(20).ToListAsync();

			ViewBag.DimensionsList = await _context.OtherProductDetails.AsNoTracking()
				.Where(x => x.Dimensions != null && x.Dimensions != "")
				.GroupBy(x => x.Dimensions).OrderByDescending(g => g.Count())
				.Select(g => g.Key).Take(20).ToListAsync();

			
			
			ViewBag.MaterialList = await _context.OtherProductDetails.AsNoTracking()
				.Where(x => x.Material != null && x.Material != "")
				.GroupBy(x => x.Material).OrderByDescending(g => g.Count())
				.Select(g => g.Key).Take(20).ToListAsync();

			// ★ 遊戲類型（歷史紀錄 Top 30）
			ViewBag.GameTypeList = await _context.GameProductDetails.AsNoTracking()
				.Where(g => g.GameType != null && g.GameType != "")
				.GroupBy(g => g.GameType)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key!).Take(30).ToListAsync();

			// ★ 幣別（常用清單）
			ViewBag.CurrencyList = new[] { "TWD", "USD", "JPY", "EUR", "CNY", "HKD", "KRW" };
		}

		// =============== 新增：全部圖片 Modal（寬版「顯示全部圖片」） ===============
		[HttpGet]
		public async Task<IActionResult> ImagesModal(int id)
		{
			var imgs = await _context.ProductImages.AsNoTracking()
				.Where(i => i.ProductId == id)
				.OrderByDescending(i => i.ProductimgId)
				.Select(i => new { i.ProductimgId, i.ProductimgUrl, i.ProductimgAltText, i.ProductimgUpdatedAt })
				.ToListAsync();

			ViewBag.ProductId = id;
			return PartialView("_ImagesModal", imgs);
		}

		//// =============== 快速新增供應商（Create/Edit 內 + 按鈕） ===============
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> QuickCreateSupplier(string name)
		//{
		//	if (string.IsNullOrWhiteSpace(name))
		//		return Json(new { ok = false, msg = "請輸入供應商名稱。" });

		//	name = name.Trim();

		//	var existed = await _context.Suppliers
		//		.AsNoTracking()
		//		.Where(s => s.SupplierName == name)
		//		.Select(s => new { s.SupplierId, s.SupplierName })
		//		.FirstOrDefaultAsync();

		//	if (existed != null)
		//		return Json(new { ok = true, id = existed.SupplierId, name = existed.SupplierName, existed = true });

		//	var entity = new Supplier { SupplierName = name };
		//	_context.Suppliers.Add(entity);
		//	await _context.SaveChangesAsync();

		//	return Json(new { ok = true, id = entity.SupplierId, name = entity.SupplierName });
		//}
	}
}
