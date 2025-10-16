using GameSpace.Models;
using GameSpace.Areas.MiniGame.Models.Settings;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 寵物換色點數設定服務實作
    /// </summary>
    public class PetColorChangeSettingsService : IPetColorChangeSettingsService
    {
        private readonly GameSpacedatabaseContext _context;

        public PetColorChangeSettingsService(GameSpacedatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得所有寵物換色點數設定
        /// </summary>
        /// <returns>寵物換色點數設定清單</returns>
        public async Task<IEnumerable<PetColorChangeSettings>> GetAllSettingsAsync()
        {
            try
            {
                return await _context.PetColorChangeSettings
                    .AsNoTracking()
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("取得所有寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 取得所有寵物換色點數設定 (Alias for GetAllSettingsAsync)
        /// </summary>
        /// <returns>寵物換色點數設定清單</returns>
        public async Task<IEnumerable<PetColorChangeSettings>> GetAllAsync()
        {
            try
            {
                return await _context.PetColorChangeSettings
                    .AsNoTracking()
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("取得所有寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 根據ID取得寵物換色點數設定
        /// </summary>
        /// <param name="id">設定ID</param>
        /// <returns>寵物換色點數設定</returns>
        public async Task<PetColorChangeSettings?> GetSettingByIdAsync(int id)
        {
            try
            {
                return await _context.PetColorChangeSettings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SettingId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"根據ID {id} 取得寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 新增寵物換色點數設定
        /// </summary>
        /// <param name="setting">寵物換色點數設定</param>
        /// <returns>新增的設定</returns>
        public async Task<PetColorChangeSettings> CreateSettingAsync(PetColorChangeSettings setting)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                setting.CreatedAt = DateTime.UtcNow;
                _context.PetColorChangeSettings.Add(setting);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return setting;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("新增寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 更新寵物換色點數設定
        /// </summary>
        /// <param name="setting">寵物換色點數設定</param>
        /// <returns>更新後的設定</returns>
        public async Task<PetColorChangeSettings> UpdateSettingAsync(PetColorChangeSettings setting)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                setting.UpdatedAt = DateTime.UtcNow;
                _context.PetColorChangeSettings.Update(setting);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return setting;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("更新寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 刪除寵物換色點數設定
        /// </summary>
        /// <param name="id">設定ID</param>
        /// <returns>是否刪除成功</returns>
        public async Task<bool> DeleteSettingAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var setting = await _context.PetColorChangeSettings
                    .FirstOrDefaultAsync(s => s.SettingId == id);

                if (setting == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                _context.PetColorChangeSettings.Remove(setting);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"刪除ID {id} 的寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 取得目前啟用的換色點數設定
        /// </summary>
        /// <returns>啟用的換色點數設定</returns>
        public async Task<PetColorChangeSettings?> GetActiveSettingAsync()
        {
            try
            {
                return await _context.PetColorChangeSettings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception("取得目前啟用的換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 切換設定的啟用狀態
        /// </summary>
        /// <param name="id">設定ID</param>
        /// <returns>是否切換成功</returns>
        public async Task<bool> ToggleActiveAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var setting = await _context.PetColorChangeSettings
                    .FirstOrDefaultAsync(s => s.SettingId == id);

                if (setting == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                setting.IsActive = !setting.IsActive;
                setting.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"切換ID {id} 的設定啟用狀態時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 根據ID取得寵物換色點數設定 (Alias for GetSettingByIdAsync)
        /// </summary>
        /// <param name="id">設定ID</param>
        /// <returns>寵物換色點數設定</returns>
        public async Task<PetColorChangeSettings?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.PetColorChangeSettings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SettingId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"根據ID {id} 取得寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 新增寵物換色點數設定 (Alias for CreateSettingAsync)
        /// </summary>
        /// <param name="setting">寵物換色點數設定</param>
        /// <returns>新增的設定</returns>
        public async Task<PetColorChangeSettings> CreateAsync(PetColorChangeSettings setting)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                setting.CreatedAt = DateTime.UtcNow;
                _context.PetColorChangeSettings.Add(setting);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return setting;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("新增寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 更新寵物換色點數設定 (Overload with ID parameter)
        /// </summary>
        /// <param name="id">設定ID</param>
        /// <param name="setting">寵物換色點數設定</param>
        /// <returns>更新後的設定</returns>
        public async Task<PetColorChangeSettings> UpdateAsync(int id, PetColorChangeSettings setting)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 檢查設定是否存在
                var existingSetting = await _context.PetColorChangeSettings
                    .FirstOrDefaultAsync(s => s.SettingId == id);

                if (existingSetting == null)
                {
                    await transaction.RollbackAsync();
                    throw new ArgumentException($"找不到ID為 {id} 的設定");
                }

                // 更新字段
                existingSetting.SettingName = setting.SettingName;
                existingSetting.ColorCode = setting.ColorCode;
                existingSetting.PointsRequired = setting.PointsRequired;
                existingSetting.IsActive = setting.IsActive;
                existingSetting.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(setting.Remarks))
                {
                    existingSetting.Remarks = setting.Remarks;
                }

                _context.PetColorChangeSettings.Update(existingSetting);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingSetting;
            }
            catch (ArgumentException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"更新ID為 {id} 的寵物換色點數設定時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 刪除寵物換色點數設定 (Alias for DeleteSettingAsync)
        /// </summary>
        /// <param name="id">設定ID</param>
        /// <returns>是否刪除成功</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var setting = await _context.PetColorChangeSettings
                    .FirstOrDefaultAsync(s => s.SettingId == id);

                if (setting == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                _context.PetColorChangeSettings.Remove(setting);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"刪除ID {id} 的寵物換色點數設定時發生錯誤", ex);
            }
        }
    }
}

