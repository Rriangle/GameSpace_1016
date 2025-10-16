using GameSpace.Areas.MiniGame.Models;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Services
{
    public class EVoucherTypeService : IEVoucherTypeService
    {
        private readonly GameSpacedatabaseContext _context;

        public EVoucherTypeService(GameSpacedatabaseContext context)
        {
            _context = context;
        }

        // EvoucherType 基本 CRUD
        public async Task<IEnumerable<EvoucherType>> GetAllEVoucherTypesAsync()
        {
            return await _context.EvoucherTypes
                .OrderByDescending(evt => evt.ValidFrom)
                .ToListAsync();
        }

        public async Task<EvoucherType?> GetEVoucherTypeByIdAsync(int evoucherTypeId)
        {
            return await _context.EvoucherTypes
                .FirstOrDefaultAsync(evt => evt.EvoucherTypeId == evoucherTypeId);
        }

        public async Task<bool> CreateEVoucherTypeAsync(EvoucherType evoucherType)
        {
            try
            {
                evoucherType.ValidFrom = DateTime.UtcNow;
                // IsActive property does not exist in EvoucherType
                _context.EvoucherTypes.Add(evoucherType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEVoucherTypeAsync(EvoucherType evoucherType)
        {
            try
            {
                // UpdatedAt property does not exist in EvoucherType
                _context.EvoucherTypes.Update(evoucherType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEVoucherTypeAsync(int evoucherTypeId)
        {
            try
            {
                var evoucherType = await GetEVoucherTypeByIdAsync(evoucherTypeId);
                if (evoucherType == null) return false;

                _context.EvoucherTypes.Remove(evoucherType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // EvoucherType 狀態管理
        public async Task<bool> ActivateEVoucherTypeAsync(int evoucherTypeId)
        {
            try
            {
                var evoucherType = await GetEVoucherTypeByIdAsync(evoucherTypeId);
                if (evoucherType == null) return false;

                // IsActive property does not exist in EvoucherType
                // UpdatedAt property does not exist in EvoucherType
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeactivateEVoucherTypeAsync(int evoucherTypeId)
        {
            try
            {
                var evoucherType = await GetEVoucherTypeByIdAsync(evoucherTypeId);
                if (evoucherType == null) return false;

                // Set ValidTo to current time to deactivate
                evoucherType.ValidTo = DateTime.UtcNow;
                // UpdatedAt property does not exist in EvoucherType
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // EvoucherType 查詢
        public async Task<IEnumerable<EvoucherType>> GetActiveEVoucherTypesAsync()
        {
            return await _context.EvoucherTypes
                .Where(evt => evt.ValidFrom <= DateTime.UtcNow && evt.ValidTo >= DateTime.UtcNow)
                .OrderBy(evt => evt.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<EvoucherType>> GetEVoucherTypesByValueRangeAsync(decimal minValue, decimal maxValue)
        {
            return await _context.EvoucherTypes
                .Where(evt => evt.ValueAmount >= minValue && evt.ValueAmount <= maxValue)
                .OrderBy(evt => evt.ValueAmount)
                .ToListAsync();
        }

        public async Task<IEnumerable<EvoucherType>> GetAvailableEVoucherTypesAsync()
        {
            return await _context.EvoucherTypes
                .Where(evt => evt.ValidFrom <= DateTime.UtcNow && evt.ValidTo >= DateTime.UtcNow && evt.TotalAvailable > 0)
                .OrderBy(evt => evt.ValueAmount)
                .ToListAsync();
        }

        // EvoucherType 庫存管理
        public async Task<bool> IncreaseStockAsync(int evoucherTypeId, int amount)
        {
            try
            {
                var evoucherType = await GetEVoucherTypeByIdAsync(evoucherTypeId);
                if (evoucherType == null) return false;

                evoucherType.TotalAvailable += amount;
                // UpdatedAt property does not exist in EvoucherType
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DecreaseStockAsync(int evoucherTypeId, int amount)
        {
            try
            {
                var evoucherType = await GetEVoucherTypeByIdAsync(evoucherTypeId);
                if (evoucherType == null || evoucherType.TotalAvailable < amount) return false;

                evoucherType.TotalAvailable -= amount;
                // UpdatedAt property does not exist in EvoucherType
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetRemainingStockAsync(int evoucherTypeId)
        {
            var evoucherType = await GetEVoucherTypeByIdAsync(evoucherTypeId);
            return evoucherType?.TotalAvailable ?? 0;
        }

        public async Task<bool> IsStockAvailableAsync(int evoucherTypeId)
        {
            var evoucherType = await GetEVoucherTypeByIdAsync(evoucherTypeId);
            return evoucherType != null && evoucherType.ValidFrom <= DateTime.UtcNow && evoucherType.ValidTo >= DateTime.UtcNow && evoucherType.TotalAvailable > 0;
        }

        // EvoucherType 統計
        public async Task<int> GetTotalEVoucherTypesCountAsync()
        {
            return await _context.EvoucherTypes.CountAsync();
        }

        public async Task<int> GetActiveEVoucherTypesCountAsync()
        {
            return await _context.EvoucherTypes.CountAsync(evt => evt.ValidFrom <= DateTime.UtcNow && evt.ValidTo >= DateTime.UtcNow);
        }

        public async Task<Dictionary<string, int>> GetEVoucherTypesDistributionAsync()
        {
            var types = await _context.EvoucherTypes.ToListAsync();
            var distribution = new Dictionary<string, int>();

            // 按價值範圍分組
            distribution["0-100"] = types.Count(t => t.ValueAmount < 100);
            distribution["100-500"] = types.Count(t => t.ValueAmount >= 100 && t.ValueAmount < 500);
            distribution["500-1000"] = types.Count(t => t.ValueAmount >= 500 && t.ValueAmount < 1000);
            distribution["1000+"] = types.Count(t => t.ValueAmount >= 1000);

            return distribution;
        }

        public async Task<IEnumerable<EVoucherTypeUsageStats>> GetEVoucherTypeUsageStatsAsync()
        {
            var evoucherTypes = await _context.EvoucherTypes.ToListAsync();
            var stats = new List<EVoucherTypeUsageStats>();

            foreach (var evoucherType in evoucherTypes)
            {
                var evouchers = await _context.Evouchers
                    .Where(e => e.EvoucherTypeId == evoucherType.EvoucherTypeId)
                    .ToListAsync();

                var totalIssued = evouchers.Count;
                var totalUsed = evouchers.Count(e => e.IsUsed);
                var totalUnused = evouchers.Count(e => !e.IsUsed);

                stats.Add(new EVoucherTypeUsageStats
                {
                    EVoucherTypeId = evoucherType.EvoucherTypeId,
                    Name = evoucherType.Name,
                    TotalIssued = totalIssued,
                    TotalUsed = totalUsed,
                    TotalUnused = totalUnused,
                    RemainingStock = evoucherType.TotalAvailable,
                    UsageRate = totalIssued > 0 ? (decimal)totalUsed / totalIssued * 100 : 0,
                    TotalValue = evoucherType.ValueAmount * totalIssued
                });
            }

            return stats.OrderByDescending(s => s.TotalValue);
        }
    }
}

