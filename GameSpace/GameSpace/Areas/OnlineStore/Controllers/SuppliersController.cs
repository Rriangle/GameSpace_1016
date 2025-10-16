using GameSpace.Areas.OnlineStore.ViewModels;
//using GameSpace.Areas.OnlineStore.ViewModels.GameSpace.Areas.OnlineStore.ViewModels;
using GameSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.OnlineStore.Controllers
{
    [Area("OnlineStore")]
    public class SuppliersController : Controller
    {
        private readonly GameSpacedatabaseContext _context;

        public SuppliersController(GameSpacedatabaseContext context)
        {
            _context = context;
        }

        // ========== Index ==========
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await _context.Suppliers
                .Select(s => new SupplierVM
                {
                    SupplierId = s.SupplierId,
                    SupplierName = s.SupplierName,
                    //IsActive = s.IsActive   // DB 有這欄位的話
                })
                .ToListAsync();

            return View(list);
        }

        // ========== Create ==========
        [HttpGet]
        public IActionResult CreateModal()
        {
            return PartialView("_SupplierForm", new SupplierVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierVM vm)
        {
            if (!ModelState.IsValid)
                return Json(new { ok = false, message = "驗證失敗" });

            var entity = new Supplier
            {
                SupplierName = vm.SupplierName,
                //IsActive = true
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return Json(new { ok = true });
        }

        

        // ========= Modal: Edit =========
        [HttpGet]
        public async Task<IActionResult> EditModal(int id)
        {
            var s = await _context.Suppliers.FindAsync(id);
            if (s == null) return NotFound();

            var vm = new SupplierVM
            {
                SupplierId = s.SupplierId,
                SupplierName = s.SupplierName,
                //IsActive = s.IsActive
            };

            return PartialView("_SupplierForm", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierVM vm)
        {
            if (!ModelState.IsValid)
                return Json(new { ok = false, message = "驗證失敗" });

            var s = await _context.Suppliers.FindAsync(vm.SupplierId);
            if (s == null) return Json(new { ok = false, message = "資料不存在" });

            s.SupplierName = vm.SupplierName;
            //s.IsActive = vm.IsActive;

            await _context.SaveChangesAsync();
            return Json(new { ok = true, message = "修改成功" });
        }


        // ========== 刪除 ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _context.Suppliers.FindAsync(id);
            if (s == null) return Json(new { ok = false, message = "不存在" });

            _context.Suppliers.Remove(s);
            await _context.SaveChangesAsync();
            return Json(new { ok = true });
        }

        //// ========== 不合作 ==========
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ToggleActive(int id)
        //{
        //    var s = await _context.Suppliers.FindAsync(id);
        //    if (s == null) return Json(new { ok = false, message = "不存在" });

        //    s.IsActive = !s.IsActive;
        //    await _context.SaveChangesAsync();
        //    return Json(new { ok = true, active = s.IsActive });
        //}

        //// ========== 查詢所屬商品 ==========
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ProductsBySuppliers([FromBody] int[] supplierIds)
        //{
        //    if (supplierIds == null || supplierIds.Length == 0)
        //        return Content("<div class='text-muted'>未選取任何供應商。</div>", "text/html; charset=utf-8");

        //    var query =
        //        from p in _context.Suppliers

        //        join s in _context.GameProductDetails on p.SupplierId equals s.SupplierId
        //        join h in _context.OtherProductDetails on p.SupplierId equals h.SupplierId into ps



        //        where supplierIds.Contains(p.SupplierId)

        //        select new SupplierProductVM
        //        {
        //            SupplierId = s.SupplierId,
        //            SupplierName = p.SupplierName,
        //            ProductId = s.ProductId,
        //            ProductName = s.ProductName
        //        };
        //    var query2 =
        //        from o in _context.ProductInfos
        //        join g in _context.Suppliers on p.ProducutName equals g.ProductName
        //        join s in _context.GameProductDetails on p.SupplierId equals s.SupplierId into ps


        //    var list = await query.ToListAsync();
        //    return PartialView("_SupplierProducts", list);
        //}
        // ========== 查詢所屬商品（多選供應商） ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductsBySuppliers([FromBody] int[] supplierIds)
        {
            if (supplierIds == null || supplierIds.Length == 0)
                return Content("<div class='text-muted'>未選取任何供應商。</div>", "text/html; charset=utf-8");

            // 遊戲商品：Supplier -> GameProductDetail -> ProductInfo
            var gameQuery =
                from sup in _context.Suppliers
                join det in _context.GameProductDetails on sup.SupplierId equals det.SupplierId
                join info in _context.ProductInfos on det.ProductId equals info.ProductId
                where supplierIds.Contains(sup.SupplierId)
                select new SupplierProductVM
                {
                    SupplierId = sup.SupplierId,
                    SupplierName = sup.SupplierName,
                    ProductId = info.ProductId,
                    ProductName = info.ProductName,
                    // ✅ 用相關子查詢挑一個 ProductCode（這裡取字典序最小的一筆）
                    ProductCode = _context.ProductCodes
                                    .Where(pc => pc.ProductId == info.ProductId)
                                    .OrderBy(pc => pc.ProductCode1)       // 如果欄位叫 Code，就改成 pc.Code
                                    .Select(pc => pc.ProductCode1)
                                    .FirstOrDefault(),
                    Category = "遊戲商品"
                };

            // 周邊商品：Supplier -> OtherProductDetail -> ProductInfo
            var otherQuery =
                from sup in _context.Suppliers
                join det in _context.OtherProductDetails on sup.SupplierId equals det.SupplierId
                join info in _context.ProductInfos on det.ProductId equals info.ProductId
                where supplierIds.Contains(sup.SupplierId)
                select new SupplierProductVM
                {
                    SupplierId = sup.SupplierId,
                    SupplierName = sup.SupplierName,
                    ProductId = info.ProductId,
                    ProductName = info.ProductName,
                    ProductCode = _context.ProductCodes
                                    .Where(pc => pc.ProductId == info.ProductId)
                                    .OrderBy(pc => pc.ProductCode1)
                                    .Select(pc => pc.ProductCode1)
                                    .FirstOrDefault(),
                    Category = "周邊商品"
                };

            var list = await gameQuery
                .Concat(otherQuery)
                .OrderBy(x => x.SupplierId).ThenBy(x => x.Category).ThenBy(x => x.ProductId)
                .ToListAsync();

            return PartialView("_SupplierProducts", list);
        }


    }



}
