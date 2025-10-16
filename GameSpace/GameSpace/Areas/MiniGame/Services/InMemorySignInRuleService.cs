using GameSpace.Areas.MiniGame.Models.ViewModels;
using GameSpace.Models;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 簽到規則管理服務
    /// 支援查詢、新增、修改、刪除簽到規則
    /// </summary>
    public interface IInMemorySignInRuleService
    {
        Task<List<SignInRuleDisplay>> GetAllRulesAsync();
        Task<SignInRuleDisplay?> GetRuleByIdAsync(int id);
        Task<bool> UpdateRuleAsync(int id, int points, int experience, bool hasCoupon, string? couponTypeCode, bool isActive, string? description);
        Task<bool> CreateRuleAsync(int signInDay, int points, int experience, bool hasCoupon, string? couponTypeCode, string? description);
        Task<bool> DeleteRuleAsync(int id);
        Task ResetToDefaultAsync();
    }

    public class InMemorySignInRuleService : IInMemorySignInRuleService
    {
        private readonly string _dataFilePath;
        private static readonly SemaphoreSlim _lock = new(1, 1);
        private static int _nextId = 11; // 起始 ID（預設有 10 筆）

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), // 支援中文
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public InMemorySignInRuleService(IWebHostEnvironment env)
        {
            // 將規則存在 Areas/MiniGame/App_Data/signin-rules.json
            var dataDir = Path.Combine(env.ContentRootPath, "Areas", "MiniGame", "App_Data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            _dataFilePath = Path.Combine(dataDir, "signin-rules.json");
        }

        /// <summary>
        /// 取得所有簽到規則（從檔案載入，若檔案不存在則創建預設規則）
        /// </summary>
        public async Task<List<SignInRuleDisplay>> GetAllRulesAsync()
        {
            await _lock.WaitAsync();
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    // 檔案不存在，創建預設規則並保存
                    var defaultRules = GetDefaultRules();
                    await SaveRulesAsync(defaultRules);
                    return defaultRules;
                }

                // 從檔案讀取
                var json = await File.ReadAllTextAsync(_dataFilePath);
                var rules = JsonSerializer.Deserialize<List<SignInRuleDisplay>>(json, JsonOptions);
                return rules ?? GetDefaultRules();
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// 內部讀取方法（不加鎖，由調用者處理）
        /// </summary>
        private async Task<List<SignInRuleDisplay>> GetAllRulesInternalAsync()
        {
            if (!File.Exists(_dataFilePath))
            {
                return GetDefaultRules();
            }

            var json = await File.ReadAllTextAsync(_dataFilePath);
            var rules = JsonSerializer.Deserialize<List<SignInRuleDisplay>>(json, JsonOptions);
            return rules ?? GetDefaultRules();
        }

        /// <summary>
        /// 保存規則到檔案（私有方法）
        /// </summary>
        private async Task SaveRulesAsync(List<SignInRuleDisplay> rules)
        {
            var json = JsonSerializer.Serialize(rules, JsonOptions);
            await File.WriteAllTextAsync(_dataFilePath, json, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 根據 ID 取得單一規則
        /// </summary>
        public async Task<SignInRuleDisplay?> GetRuleByIdAsync(int id)
        {
            var rules = await GetAllRulesAsync();
            return rules.FirstOrDefault(r => r.Id == id);
        }

        /// <summary>
        /// 更新規則（持久化到檔案）
        /// </summary>
        public async Task<bool> UpdateRuleAsync(int id, int points, int experience, bool hasCoupon, string? couponTypeCode, bool isActive, string? description)
        {
            await _lock.WaitAsync();
            try
            {
                var rules = await GetAllRulesInternalAsync();
                var rule = rules.FirstOrDefault(r => r.Id == id);
                
                if (rule == null)
                    return false;

                rule.Points = points;
                rule.Experience = experience;
                rule.HasCoupon = hasCoupon;
                rule.CouponTypeCode = hasCoupon ? couponTypeCode : null;
                rule.IsActive = isActive;
                rule.Description = description;

                // 持久化到檔案
                await SaveRulesAsync(rules);
                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// 創建新規則（持久化到檔案）
        /// </summary>
        public async Task<bool> CreateRuleAsync(int signInDay, int points, int experience, bool hasCoupon, string? couponTypeCode, string? description)
        {
            await _lock.WaitAsync();
            try
            {
                var rules = await GetAllRulesInternalAsync();
                
                // 檢查是否已有相同天數的規則
                if (rules.Any(r => r.DayNumber == signInDay))
                    return false;

                var newRule = new SignInRuleDisplay
                {
                    Id = _nextId++,
                    DayNumber = signInDay,
                    Points = points,
                    Experience = experience,
                    HasCoupon = hasCoupon,
                    CouponTypeCode = hasCoupon ? couponTypeCode : null,
                    IsActive = true,
                    Description = description
                };

                rules.Add(newRule);

                // 持久化到檔案
                await SaveRulesAsync(rules);
                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// 刪除規則（持久化到檔案）
        /// </summary>
        public async Task<bool> DeleteRuleAsync(int id)
        {
            await _lock.WaitAsync();
            try
            {
                var rules = await GetAllRulesInternalAsync();
                var rule = rules.FirstOrDefault(r => r.Id == id);
                
                if (rule == null)
                    return false;

                rules.Remove(rule);

                // 持久化到檔案
                await SaveRulesAsync(rules);
                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// 重置為預設規則（持久化到檔案）
        /// </summary>
        public async Task ResetToDefaultAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var rules = GetDefaultRules();
                await SaveRulesAsync(rules);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// 取得預設簽到規則（對應 Create_SignInRule_Table.sql 中的種子資料）
        /// </summary>
        private static List<SignInRuleDisplay> GetDefaultRules()
        {
            return new List<SignInRuleDisplay>
            {
                new() { Id = 1, DayNumber = 1, Points = 10, Experience = 5, HasCoupon = false, CouponTypeCode = null, IsActive = true, Description = "第一天簽到獎勵" },
                new() { Id = 2, DayNumber = 2, Points = 10, Experience = 5, HasCoupon = false, CouponTypeCode = null, IsActive = true, Description = "第二天簽到獎勵" },
                new() { Id = 3, DayNumber = 3, Points = 15, Experience = 8, HasCoupon = false, CouponTypeCode = null, IsActive = true, Description = "第三天簽到獎勵" },
                new() { Id = 4, DayNumber = 4, Points = 15, Experience = 8, HasCoupon = false, CouponTypeCode = null, IsActive = true, Description = "第四天簽到獎勵" },
                new() { Id = 5, DayNumber = 5, Points = 20, Experience = 10, HasCoupon = false, CouponTypeCode = null, IsActive = true, Description = "第五天簽到獎勵" },
                new() { Id = 6, DayNumber = 6, Points = 20, Experience = 10, HasCoupon = false, CouponTypeCode = null, IsActive = true, Description = "第六天簽到獎勵" },
                new() { Id = 7, DayNumber = 7, Points = 30, Experience = 15, HasCoupon = true, CouponTypeCode = "WEEK_BONUS", IsActive = true, Description = "第七天簽到獎勵 + 週獎勵優惠券" },
                new() { Id = 8, DayNumber = 14, Points = 50, Experience = 25, HasCoupon = true, CouponTypeCode = "TWO_WEEK_BONUS", IsActive = true, Description = "連續簽到 14 天獎勵" },
                new() { Id = 9, DayNumber = 21, Points = 80, Experience = 40, HasCoupon = true, CouponTypeCode = "THREE_WEEK_BONUS", IsActive = true, Description = "連續簽到 21 天獎勵" },
                new() { Id = 10, DayNumber = 30, Points = 150, Experience = 75, HasCoupon = true, CouponTypeCode = "MONTH_BONUS", IsActive = true, Description = "連續簽到 30 天獎勵（滿月獎勵）" }
            };
        }
    }
}

