using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 點數設定統計服務實作
    /// </summary>
    public class PointsSettingsStatisticsService : IPointsSettingsStatisticsService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<PointsSettingsStatisticsService> _logger;

        public PointsSettingsStatisticsService(
            GameSpacedatabaseContext context,
            ILogger<PointsSettingsStatisticsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Dictionary<string, object>> GetStatisticsAsync()
        {
            try
            {
                var stats = new Dictionary<string, object>
                {
                    ["TotalUsers"] = await _context.UserWallets.CountAsync(),
                    ["TotalPoints"] = await _context.UserWallets.SumAsync(w => w.UserPoint),
                    ["AveragePoints"] = await _context.UserWallets.AverageAsync(w => (double)w.UserPoint),
                    ["MaxPoints"] = await _context.UserWallets.MaxAsync(w => w.UserPoint),
                    ["MinPoints"] = await _context.UserWallets.MinAsync(w => w.UserPoint)
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取點數統計時發生錯誤");
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string, int>> GetPointsDistributionAsync()
        {
            try
            {
                var distribution = new Dictionary<string, int>
                {
                    ["0-1000"] = await _context.UserWallets.CountAsync(w => w.UserPoint >= 0 && w.UserPoint < 1000),
                    ["1000-5000"] = await _context.UserWallets.CountAsync(w => w.UserPoint >= 1000 && w.UserPoint < 5000),
                    ["5000-10000"] = await _context.UserWallets.CountAsync(w => w.UserPoint >= 5000 && w.UserPoint < 10000),
                    ["10000+"] = await _context.UserWallets.CountAsync(w => w.UserPoint >= 10000)
                };

                return distribution;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取點數分佈時發生錯誤");
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, int>> GetPointsUsageTrendAsync(int days = 30)
        {
            try
            {
                var startDate = DateTime.Now.AddDays(-days);
                var trend = new Dictionary<string, int>();

                // 查詢 WalletHistory 表獲取點數變化趨勢
                var histories = await _context.WalletHistories
                    .Where(h => h.ChangeTime >= startDate && h.ChangeType == "Point")
                    .ToListAsync();

                // 按日期分組統計
                var groupedByDate = histories
                    .GroupBy(h => h.ChangeTime.Date)
                    .OrderBy(g => g.Key);

                foreach (var group in groupedByDate)
                {
                    var dateKey = group.Key.ToString("yyyy-MM-dd");
                    trend[dateKey] = group.Sum(h => Math.Abs(h.PointsChanged));
                }

                return trend;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取點數使用趨勢時發生錯誤");
                return new Dictionary<string, int>();
            }
        }

        public async Task<int> GetTotalColorSettingsAsync()
        {
            try
            {
                return await _context.PetColorChangeSettings.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取顏色設定總數時發生錯誤");
                return 0;
            }
        }

        public async Task<int> GetTotalBackgroundSettingsAsync()
        {
            try
            {
                return await _context.PetBackgroundChangeSettings.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取背景設定總數時發生錯誤");
                return 0;
            }
        }

        public async Task<int> GetActiveColorSettingsAsync()
        {
            try
            {
                return await _context.PetColorChangeSettings
                    .CountAsync(s => s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取啟用的顏色設定數量時發生錯誤");
                return 0;
            }
        }

        public async Task<int> GetActiveBackgroundSettingsAsync()
        {
            try
            {
                return await _context.PetBackgroundChangeSettings
                    .CountAsync(s => s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取啟用的背景設定數量時發生錯誤");
                return 0;
            }
        }

        public async Task<int> GetTotalColorPointsAsync()
        {
            try
            {
                // 計算所有顏色變更設定的總點數
                var total = await _context.PetColorChangeSettings
                    .Where(s => s.IsActive)
                    .SumAsync(s => s.PointsRequired);

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有顏色變更總點數時發生錯誤");
                return 0;
            }
        }

        public async Task<int> GetTotalBackgroundPointsAsync()
        {
            try
            {
                // 計算所有背景變更設定的總點數
                var total = await _context.PetBackgroundChangeSettings
                    .Where(s => s.IsActive)
                    .SumAsync(s => s.RequiredPoints);

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有背景變更總點數時發生錯誤");
                return 0;
            }
        }

        public async Task<int> GetTotalPointsAsync()
        {
            try
            {
                var colorPoints = await GetTotalColorPointsAsync();
                var backgroundPoints = await GetTotalBackgroundPointsAsync();

                return colorPoints + backgroundPoints;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取總點數時發生錯誤");
                return 0;
            }
        }
    }
}

