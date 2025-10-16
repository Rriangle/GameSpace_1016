using GameSpace.Areas.MiniGame.Models;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Services
{
    public class GameRulesService : IGameRulesService
    {
        private readonly GameSpacedatabaseContext _context;

        public GameRulesService(GameSpacedatabaseContext context)
        {
            _context = context;
        }

        // 每日遊戲限制
        public async Task<int> GetDailyGameLimitAsync()
        {
            // 預設每日限制為 10 次
            return await Task.FromResult(10);
        }

        public async Task<bool> UpdateDailyGameLimitAsync(int limit)
        {
            try
            {
                // 實際需要儲存到設定表
                // 這裡簡化為直接返回成功
                await Task.CompletedTask;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetUserGameCountTodayAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            // 使用簽到次數作為遊戲次數的簡化版
            var count = await _context.UserSignInStats
                .CountAsync(s => s.UserId == userId && s.SignTime >= today);

            return count;
        }

        public async Task<bool> CanUserPlayGameAsync(int userId)
        {
            var todayCount = await GetUserGameCountTodayAsync(userId);
            var limit = await GetDailyGameLimitAsync();
            return todayCount < limit;
        }

        // 遊戲獎勵設定
        public async Task<GameRewardSettings?> GetGameRewardSettingsAsync()
        {
            // 預設獎勵設定
            return await Task.FromResult(new GameRewardSettings
            {
                Id = 1,
                PointsRewardRate = 1.0m,
                ExpRewardRate = 1.0m,
                CouponRewardRate = 0.1m,
                PointsRewardEnabled = true,
                ExpRewardEnabled = true,
                CouponRewardEnabled = true,
                MinPointsReward = 5,
                MaxPointsReward = 50,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<bool> UpdateGameRewardSettingsAsync(GameRewardSettings settings)
        {
            try
            {
                settings.UpdatedAt = DateTime.UtcNow;
                // 實際需要儲存到設定表
                await Task.CompletedTask;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GameReward> CalculateGameRewardAsync(int gameId, int score, bool isWin)
        {
            var settings = await GetGameRewardSettingsAsync();
            var reward = new GameReward();

            if (settings != null)
            {
                // 基礎點數 = 分數 / 10
                var basePoints = score / 10;

                // 勝利加成
                if (isWin)
                {
                    reward.BonusMultiplier = 2;
                    basePoints *= 2;
                }
                else
                {
                    reward.BonusMultiplier = 1;
                }

                // 套用獎勵設定
                if (settings.PointsRewardEnabled)
                {
                    reward.Points = Math.Clamp(
                        (int)(basePoints * settings.PointsRewardRate),
                        settings.MinPointsReward,
                        settings.MaxPointsReward
                    );
                }

                if (settings.ExpRewardEnabled)
                {
                    reward.Experience = (int)(basePoints * 0.5m * settings.ExpRewardRate);
                }

                // 隨機優惠券（10% 機率）
                if (settings.CouponRewardEnabled && Random.Shared.NextDouble() < (double)settings.CouponRewardRate)
                {
                    reward.CouponCode = $"GAME_{gameId}_{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                }
            }

            return reward;
        }

        // 遊戲難度設定
        public async Task<IEnumerable<GameDifficultySettings>> GetAllDifficultySettingsAsync()
        {
            // 預設難度設定
            return await Task.FromResult(new List<GameDifficultySettings>
            {
                new GameDifficultySettings { Id = 1, Level = 1, MonsterCount = 5, SpeedMultiplier = 1.0m, RewardMultiplier = 1, IsActive = true },
                new GameDifficultySettings { Id = 2, Level = 2, MonsterCount = 8, SpeedMultiplier = 1.2m, RewardMultiplier = 2, IsActive = true },
                new GameDifficultySettings { Id = 3, Level = 3, MonsterCount = 12, SpeedMultiplier = 1.5m, RewardMultiplier = 3, IsActive = true },
                new GameDifficultySettings { Id = 4, Level = 4, MonsterCount = 15, SpeedMultiplier = 2.0m, RewardMultiplier = 5, IsActive = true }
            });
        }

        public async Task<GameDifficultySettings?> GetDifficultySettingByLevelAsync(int level)
        {
            var settings = await GetAllDifficultySettingsAsync();
            return settings.FirstOrDefault(s => s.Level == level);
        }

        public async Task<bool> UpdateDifficultySettingAsync(GameDifficultySettings setting)
        {
            try
            {
                // 實際需要儲存到設定表
                await Task.CompletedTask;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 遊戲規則管理
        public async Task<IEnumerable<GameRule>> GetAllGameRulesAsync()
        {
            return await _context.GameRules
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<GameRule?> GetGameRuleByIdAsync(int ruleId)
        {
            return await _context.GameRules
                .FirstOrDefaultAsync(r => r.Id == ruleId);
        }

        public async Task<bool> CreateGameRuleAsync(GameRule rule)
        {
            try
            {
                rule.CreatedAt = DateTime.UtcNow;
                rule.IsActive = true;
                _context.GameRules.Add(rule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateGameRuleAsync(GameRule rule)
        {
            try
            {
                rule.UpdatedAt = DateTime.UtcNow;
                _context.GameRules.Update(rule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteGameRuleAsync(int ruleId)
        {
            try
            {
                var rule = await GetGameRuleByIdAsync(ruleId);
                if (rule == null) return false;

                _context.GameRules.Remove(rule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleGameRuleAsync(int ruleId)
        {
            try
            {
                var rule = await GetGameRuleByIdAsync(ruleId);
                if (rule == null) return false;

                rule.IsActive = !rule.IsActive;
                rule.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 特殊事件規則
        public async Task<IEnumerable<GameEventRule>> GetActiveGameEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.GameEventRules
                .Where(e => e.IsActive && e.StartDate <= now && e.EndDate >= now)
                .OrderBy(e => e.EndDate)
                .ToListAsync();
        }

        public async Task<bool> CreateGameEventAsync(GameEventRule eventRule)
        {
            try
            {
                eventRule.IsActive = true;
                _context.GameEventRules.Add(eventRule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateGameEventAsync(GameEventRule eventRule)
        {
            try
            {
                _context.GameEventRules.Update(eventRule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EndGameEventAsync(int eventId)
        {
            try
            {
                var eventRule = await _context.GameEventRules
                    .FirstOrDefaultAsync(e => e.Id == eventId);
                if (eventRule == null) return false;

                eventRule.IsActive = false;
                eventRule.EndDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

