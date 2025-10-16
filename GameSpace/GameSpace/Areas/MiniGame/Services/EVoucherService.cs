using GameSpace.Models;
using GameSpace.Areas.MiniGame.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Services
{
    public class EVoucherService : IEVoucherService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<EVoucherService> _logger;

        public EVoucherService(GameSpacedatabaseContext context, ILogger<EVoucherService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Evoucher>> GetAllEVouchersAsync()
        {
            try
            {
                return await _context.Evouchers
                    .Include(e => e.EvoucherType)
                    .OrderByDescending(e => e.AcquiredTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得所有電子禮券時發生錯誤");
                return new List<Evoucher>();
            }
        }

        public async Task<IEnumerable<Evoucher>> GetEVouchersByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Evouchers
                    .Include(e => e.EvoucherType)
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.AcquiredTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得使用者 {UserId} 的電子禮券時發生錯誤", userId);
                return new List<Evoucher>();
            }
        }

        public async Task<IEnumerable<Evoucher>> GetUnusedEVouchersByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Evouchers
                    .Include(e => e.EvoucherType)
                    .Where(e => e.UserId == userId && !e.IsUsed)
                    .OrderByDescending(e => e.AcquiredTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得使用者 {UserId} 未使用的電子禮券時發生錯誤", userId);
                return new List<Evoucher>();
            }
        }

        public async Task<Evoucher?> GetEVoucherByIdAsync(int eVoucherId)
        {
            try
            {
                return await _context.Evouchers
                    .Include(e => e.EvoucherType)
                    .FirstOrDefaultAsync(e => e.EvoucherId == eVoucherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得電子禮券 {EVoucherId} 時發生錯誤", eVoucherId);
                return null;
            }
        }

        public async Task<Evoucher?> GetEVoucherByCodeAsync(string eVoucherCode)
        {
            try
            {
                return await _context.Evouchers
                    .Include(e => e.EvoucherType)
                    .FirstOrDefaultAsync(e => e.EvoucherCode == eVoucherCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得電子禮券代碼 {EVoucherCode} 時發生錯誤", eVoucherCode);
                return null;
            }
        }

        public async Task<bool> CreateEVoucherAsync(Evoucher eVoucher)
        {
            try
            {
                _context.Evouchers.Add(eVoucher);
                await _context.SaveChangesAsync();
                _logger.LogInformation("成功建立電子禮券 {EVoucherCode}", eVoucher.EvoucherCode);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "建立電子禮券時發生錯誤");
                return false;
            }
        }

        public async Task<bool> UpdateEVoucherAsync(Evoucher eVoucher)
        {
            try
            {
                _context.Evouchers.Update(eVoucher);
                await _context.SaveChangesAsync();
                _logger.LogInformation("成功更新電子禮券 {EVoucherId}", eVoucher.EvoucherId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新電子禮券 {EVoucherId} 時發生錯誤", eVoucher.EvoucherId);
                return false;
            }
        }

        public async Task<bool> DeleteEVoucherAsync(int eVoucherId)
        {
            try
            {
                var eVoucher = await _context.Evouchers.FindAsync(eVoucherId);
                if (eVoucher == null) return false;

                _context.Evouchers.Remove(eVoucher);
                await _context.SaveChangesAsync();
                _logger.LogInformation("成功刪除電子禮券 {EVoucherId}", eVoucherId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除電子禮券 {EVoucherId} 時發生錯誤", eVoucherId);
                return false;
            }
        }

        public async Task<bool> UseEVoucherAsync(int eVoucherId)
        {
            try
            {
                var eVoucher = await _context.Evouchers.FindAsync(eVoucherId);
                if (eVoucher == null || eVoucher.IsUsed) return false;

                eVoucher.IsUsed = true;
                eVoucher.UsedTime = DateTime.Now;

                await _context.SaveChangesAsync();
                _logger.LogInformation("成功使用電子禮券 {EVoucherId}", eVoucherId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "使用電子禮券 {EVoucherId} 時發生錯誤", eVoucherId);
                return false;
            }
        }

        public async Task<bool> GrantEVoucherToUserAsync(int userId, int eVoucherTypeId)
        {
            try
            {
                var eVoucherType = await _context.EvoucherTypes.FindAsync(eVoucherTypeId);
                if (eVoucherType == null) return false;

                var eVoucherCode = GenerateEVoucherCode(eVoucherType.Name);
                var eVoucher = new Evoucher
                {
                    EvoucherCode = eVoucherCode,
                    EvoucherTypeId = eVoucherTypeId,
                    UserId = userId,
                    IsUsed = false,
                    AcquiredTime = DateTime.Now
                };

                _context.Evouchers.Add(eVoucher);
                await _context.SaveChangesAsync();
                _logger.LogInformation("成功發放電子禮券 {EVoucherCode} 給使用者 {UserId}", eVoucherCode, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發放電子禮券給使用者 {UserId} 時發生錯誤", userId);
                return false;
            }
        }

        public async Task<int> GetEVoucherCountByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Evouchers
                    .Where(e => e.UserId == userId && !e.IsUsed)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得使用者 {UserId} 電子禮券數量時發生錯誤", userId);
                return 0;
            }
        }

        public async Task<IEnumerable<EvoucherType>> GetAllEVoucherTypesAsync()
        {
            try
            {
                return await _context.EvoucherTypes
                    .OrderBy(et => et.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得所有電子禮券類型時發生錯誤");
                return new List<EvoucherType>();
            }
        }

        public async Task<EvoucherType?> GetEVoucherTypeByIdAsync(int eVoucherTypeId)
        {
            try
            {
                return await _context.EvoucherTypes.FindAsync(eVoucherTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得電子禮券類型 {EVoucherTypeId} 時發生錯誤", eVoucherTypeId);
                return null;
            }
        }

        private string GenerateEVoucherCode(string typeName)
        {
            var typeCode = GetTypeCode(typeName);
            var random = new Random();
            var randomCode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            var randomNumber = random.Next(100000, 999999);
            return $"EV-{typeCode}-{randomCode}-{randomNumber}";
        }

        private string GetTypeCode(string typeName)
        {
            if (typeName.Contains("現金")) return "CASH";
            if (typeName.Contains("咖啡")) return "COFFEE";
            if (typeName.Contains("影城")) return "MOVIE";
            if (typeName.Contains("餐飲")) return "FOOD";
            if (typeName.Contains("加油")) return "GAS";
            if (typeName.Contains("百貨")) return "STORE";
            return "OTHER";
        }
    }
}

