using GameSpace.Areas.MiniGame.Models;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Services
{
    public class WalletService : IWalletService
    {
        private readonly GameSpacedatabaseContext _context;

        public WalletService(GameSpacedatabaseContext context)
        {
            _context = context;
        }

        // Wallet 基本操作
        public async Task<UserWallet?> GetWalletByUserIdAsync(int userId)
        {
            return await _context.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<int> GetUserPointsAsync(int userId)
        {
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.UserPoint ?? 0;
        }

        public async Task<bool> AddPointsAsync(int userId, int points, string description, string itemCode = "")
        {
            try
            {
                var wallet = await GetWalletByUserIdAsync(userId);
                if (wallet == null) return false;

                wallet.UserPoint += points;

                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "Add",
                    PointsChanged = points,
                    ItemCode = itemCode,
                    Description = description,
                    ChangeTime = DateTime.UtcNow
                };
                _context.WalletHistories.Add(history);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeductPointsAsync(int userId, int points, string description, string itemCode = "")
        {
            try
            {
                var wallet = await GetWalletByUserIdAsync(userId);
                if (wallet == null || wallet.UserPoint < points) return false;

                wallet.UserPoint -= points;

                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "Deduct",
                    PointsChanged = -points,
                    ItemCode = itemCode,
                    Description = description,
                    ChangeTime = DateTime.UtcNow
                };
                _context.WalletHistories.Add(history);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TransferPointsAsync(int fromUserId, int toUserId, int points, string description)
        {
            try
            {
                var fromWallet = await GetWalletByUserIdAsync(fromUserId);
                var toWallet = await GetWalletByUserIdAsync(toUserId);

                if (fromWallet == null || toWallet == null || fromWallet.UserPoint < points)
                    return false;

                fromWallet.UserPoint -= points;
                toWallet.UserPoint += points;

                var historyFrom = new WalletHistory
                {
                    UserId = fromUserId,
                    ChangeType = "Transfer_Out",
                    PointsChanged = -points,
                    ItemCode = $"TO_USER_{toUserId}",
                    Description = $"轉帳給用戶 {toUserId}: {description}",
                    ChangeTime = DateTime.UtcNow
                };

                var historyTo = new WalletHistory
                {
                    UserId = toUserId,
                    ChangeType = "Transfer_In",
                    PointsChanged = points,
                    ItemCode = $"FROM_USER_{fromUserId}",
                    Description = $"收到來自用戶 {fromUserId} 的轉帳: {description}",
                    ChangeTime = DateTime.UtcNow
                };

                _context.WalletHistories.Add(historyFrom);
                _context.WalletHistories.Add(historyTo);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Wallet 歷史記錄查詢
        public async Task<IEnumerable<WalletHistory>> GetWalletHistoryAsync(int userId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.WalletHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ChangeTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<WalletHistory>> GetWalletHistoryByTypeAsync(int userId, string changeType)
        {
            return await _context.WalletHistories
                .Where(h => h.UserId == userId && h.ChangeType == changeType)
                .OrderByDescending(h => h.ChangeTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<WalletHistory>> GetWalletHistoryByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.WalletHistories
                .Where(h => h.UserId == userId && h.ChangeTime >= startDate && h.ChangeTime <= endDate)
                .OrderByDescending(h => h.ChangeTime)
                .ToListAsync();
        }

        public async Task<WalletHistory?> GetHistoryDetailAsync(int logId)
        {
            return await _context.WalletHistories
                .FirstOrDefaultAsync(h => h.LogId == logId);
        }

        public async Task<int> GetTotalHistoryCountAsync(int userId)
        {
            return await _context.WalletHistories
                .Where(h => h.UserId == userId)
                .CountAsync();
        }

        // Wallet 統計數據
        public async Task<Dictionary<string, int>> GetPointsSummaryAsync(int userId)
        {
            var wallet = await GetWalletByUserIdAsync(userId);
            var history = await _context.WalletHistories
                .Where(h => h.UserId == userId)
                .ToListAsync();

            var totalEarned = history
                .Where(h => h.PointsChanged > 0)
                .Sum(h => h.PointsChanged);

            var totalSpent = history
                .Where(h => h.PointsChanged < 0)
                .Sum(h => Math.Abs(h.PointsChanged));

            return new Dictionary<string, int>
            {
                { "CurrentPoints", wallet?.UserPoint ?? 0 },
                { "TotalEarned", totalEarned },
                { "TotalSpent", totalSpent },
                { "TransactionCount", history.Count }
            };
        }

        public async Task<Dictionary<string, int>> GetPointsStatsByTypeAsync(int userId)
        {
            var history = await _context.WalletHistories
                .Where(h => h.UserId == userId)
                .GroupBy(h => h.ChangeType)
                .Select(g => new { Type = g.Key, Total = g.Sum(h => h.PointsChanged) })
                .ToListAsync();

            return history.ToDictionary(x => x.Type, x => x.Total);
        }

        public async Task<int> GetTotalPointsEarnedAsync(int userId)
        {
            return await _context.WalletHistories
                .Where(h => h.UserId == userId && h.PointsChanged > 0)
                .SumAsync(h => h.PointsChanged);
        }

        public async Task<int> GetTotalPointsSpentAsync(int userId)
        {
            return await _context.WalletHistories
                .Where(h => h.UserId == userId && h.PointsChanged < 0)
                .SumAsync(h => Math.Abs(h.PointsChanged));
        }

        // 管理員功能
        public async Task<bool> AdjustUserPointsAsync(int userId, int points, string reason, int? managerId = null)
        {
            try
            {
                var wallet = await GetWalletByUserIdAsync(userId);
                if (wallet == null) return false;

                wallet.UserPoint += points;

                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = points > 0 ? "Admin_Add" : "Admin_Deduct",
                    PointsChanged = points,
                    ItemCode = $"ADMIN_{managerId ?? 0}",
                    Description = $"管理員調整: {reason}",
                    ChangeTime = DateTime.UtcNow
                };
                _context.WalletHistories.Add(history);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetUserPointsAsync(int userId, string reason, int? managerId = null)
        {
            try
            {
                var wallet = await GetWalletByUserIdAsync(userId);
                if (wallet == null) return false;

                var currentPoints = wallet.UserPoint;
                wallet.UserPoint = 0;

                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "Admin_Reset",
                    PointsChanged = -currentPoints,
                    ItemCode = $"ADMIN_RESET_{managerId ?? 0}",
                    Description = $"管理員重置: {reason} (原有點數: {currentPoints})",
                    ChangeTime = DateTime.UtcNow
                };
                _context.WalletHistories.Add(history);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<WalletHistory>> GetAllHistoryAsync(int pageNumber = 1, int pageSize = 50)
        {
            return await _context.WalletHistories
                .OrderByDescending(h => h.ChangeTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 批次操作
        public async Task<bool> GrantPointsToMultipleUsersAsync(IEnumerable<int> userIds, int points, string description)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                foreach (var userId in userIds)
                {
                    var wallet = await GetWalletByUserIdAsync(userId);
                    if (wallet != null)
                    {
                        wallet.UserPoint += points;

                        var history = new WalletHistory
                        {
                            UserId = userId,
                            ChangeType = "Batch_Grant",
                            PointsChanged = points,
                            ItemCode = "BATCH_OPERATION",
                            Description = $"批次發放: {description}",
                            ChangeTime = DateTime.UtcNow
                        };
                        _context.WalletHistories.Add(history);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserWallet>> GetTopWalletsAsync(int count = 10)
        {
            return await _context.UserWallets
                .OrderByDescending(w => w.UserPoint)
                .Take(count)
                .ToListAsync();
        }
    }
}

