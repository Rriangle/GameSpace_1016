using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameSpace.Areas.MiniGame.Services;
using GameSpace.Areas.MiniGame.Models;
using Microsoft.EntityFrameworkCore;
using GameSpace.Models;

namespace GameSpace.Areas.MiniGame.Controllers
{
    [Area("MiniGame")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Policy = "AdminOnly")]
    public class AdminWalletTypesController : MiniGameBaseController
    {
        public AdminWalletTypesController(GameSpacedatabaseContext context, IMiniGameAdminService adminService) : base(context, adminService)
        {
        }

        // 錢包類型管理首頁
        public async Task<IActionResult> Index()
        {
            try
            {
                var walletTypes = await GetAllWalletTypesAsync();
                var viewModel = new AdminWalletTypesViewModel
                {
                    WalletTypes = walletTypes
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"載入錢包類型管理頁面時發生錯誤：{ex.Message}";
                return View(new AdminWalletTypesViewModel());
            }
        }

        // 新增錢包類型
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                var viewModel = new CreateWalletTypeViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"載入新增錢包類型頁面時發生錯誤：{ex.Message}";
                return View(new CreateWalletTypeViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWalletTypeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await CreateWalletTypeAsync(model);
                TempData["SuccessMessage"] = "錢包類型創建成功！";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"創建失敗：{ex.Message}");
                return View(model);
            }
        }

        // 編輯錢包類型
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var walletType = await GetWalletTypeByIdAsync(id);
                if (walletType == null)
                {
                    TempData["ErrorMessage"] = "找不到指定的錢包類型";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new EditWalletTypeViewModel
                {
                    WalletTypeId = walletType.WalletTypeId,
                    TypeName = walletType.TypeName,
                    Description = walletType.Description,
                    IsActive = walletType.IsActive
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"載入編輯錢包類型頁面時發生錯誤：{ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditWalletTypeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await UpdateWalletTypeAsync(model);
                TempData["SuccessMessage"] = "錢包類型更新成功！";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"更新失敗：{ex.Message}");
                return View(model);
            }
        }

        // 刪除錢包類型
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await DeleteWalletTypeAsync(id);
                return Json(new { success = true, message = "錢包類型刪除成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 切換錢包類型狀態
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await ToggleWalletTypeStatusAsync(id);
                return Json(new { success = true, message = "錢包類型狀態切換成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 獲取錢包類型詳情
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var walletType = await GetWalletTypeByIdAsync(id);
                if (walletType == null)
                    return Json(new { success = false, message = "找不到指定的錢包類型" });

                var details = new
                {
                    walletTypeId = walletType.WalletTypeId,
                    typeName = walletType.TypeName,
                    description = walletType.Description,
                    isActive = walletType.IsActive,
                    createdTime = walletType.CreatedTime
                };

                return Json(new { success = true, data = details });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 私有方法
        private async Task<List<WalletTypeModel>> GetAllWalletTypesAsync()
        {
            var walletTypes = await _context.WalletTypes
                .OrderBy(w => w.TypeName)
                .Select(w => new WalletTypeModel
                {
                    WalletTypeId = w.WalletTypeId,
                    TypeName = w.TypeName,
                    Description = w.Description,
                    IsActive = w.IsActive,
                    CreatedTime = w.CreatedTime
                })
                .ToListAsync();

            return walletTypes;
        }

        private async Task<WalletTypeModel> GetWalletTypeByIdAsync(int id)
        {
            var walletType = await _context.WalletTypes.FindAsync(id);
            if (walletType == null)
                return null;

            return new WalletTypeModel
            {
                WalletTypeId = walletType.WalletTypeId,
                TypeName = walletType.TypeName,
                Description = walletType.Description,
                IsActive = walletType.IsActive,
                CreatedTime = walletType.CreatedTime
            };
        }

        private async Task CreateWalletTypeAsync(CreateWalletTypeViewModel model)
        {
            var walletType = new GameSpace.Models.WalletType
            {
                TypeName = model.TypeName,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedTime = DateTime.Now
            };

            _context.WalletTypes.Add(walletType);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateWalletTypeAsync(EditWalletTypeViewModel model)
        {
            var walletType = await _context.WalletTypes.FindAsync(model.WalletTypeId);
            if (walletType == null)
                throw new Exception("找不到指定的錢包類型");

            walletType.TypeName = model.TypeName;
            walletType.Description = model.Description;
            walletType.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
        }

        private async Task DeleteWalletTypeAsync(int id)
        {
            var walletType = await _context.WalletTypes.FindAsync(id);
            if (walletType == null)
                throw new Exception("找不到指定的錢包類型");

            _context.WalletTypes.Remove(walletType);
            await _context.SaveChangesAsync();
        }

        private async Task ToggleWalletTypeStatusAsync(int id)
        {
            var walletType = await _context.WalletTypes.FindAsync(id);
            if (walletType == null)
                throw new Exception("找不到指定的錢包類型");

            walletType.IsActive = !walletType.IsActive;
            await _context.SaveChangesAsync();
        }
    }

    // ViewModels
    public class AdminWalletTypesViewModel
    {
        public List<WalletTypeModel> WalletTypes { get; set; } = new();
    }

    public class CreateWalletTypeViewModel
    {
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class EditWalletTypeViewModel
    {
        public int WalletTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class WalletTypeModel
    {
        public int WalletTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}



