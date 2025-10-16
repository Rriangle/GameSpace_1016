using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.MiniGame.Services
{
    public class UserService : IUserService
    {
        private readonly GameSpacedatabaseContext _context;

        public UserService(GameSpacedatabaseContext context)
        {
            _context = context;
        }

        // 使用者基本 CRUD
        public async Task<IEnumerable<User>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 50)
        {
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .OrderByDescending(u => u.UserIntroduce != null ? u.UserIntroduce.CreateAccount : DateTime.MinValue)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserRight)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByAccountAsync(string account)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserAccount == account);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Where(u => u.UserIntroduce != null && u.UserIntroduce.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                // Note: User_CreatedAt doesn't exist on User entity
                // CreateAccount should be set on UserIntroduce instead
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 自動建立錢包
                var wallet = new UserWallet
                {
                    UserId = user.UserId,
                    UserPoint = 0
                };
                _context.UserWallets.Add(wallet);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 使用者狀態管理
        public async Task<bool> ActivateUserAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null) return false;

                if (user.UserRight == null)
                {
                    user.UserRight = new GameSpace.Models.UserRight { UserId = userId };
                    _context.UserRights.Add(user.UserRight);
                }
                user.UserRight.UserStatus = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null) return false;

                if (user.UserRight == null)
                {
                    user.UserRight = new GameSpace.Models.UserRight { UserId = userId };
                    _context.UserRights.Add(user.UserRight);
                }
                user.UserRight.UserStatus = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LockUserAsync(int userId, DateTime? lockoutEnd = null)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null) return false;

                user.UserLockoutEnd = lockoutEnd ?? DateTime.UtcNow.AddDays(30);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnlockUserAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null) return false;

                user.UserLockoutEnd = null;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 使用者搜尋與篩選
        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Where(u => u.UserAccount.Contains(searchTerm) ||
                           (u.UserIntroduce != null && u.UserIntroduce.Email.Contains(searchTerm)) ||
                           u.UserName.Contains(searchTerm))
                .OrderByDescending(u => u.UserIntroduce != null ? u.UserIntroduce.CreateAccount : DateTime.MinValue)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserRight)
                .Where(u => u.UserRight != null && u.UserRight.UserStatus == true)
                .OrderByDescending(u => u.UserIntroduce != null ? u.UserIntroduce.CreateAccount : DateTime.MinValue)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetInactiveUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserRight)
                .Where(u => u.UserRight != null && u.UserRight.UserStatus == false)
                .OrderByDescending(u => u.UserIntroduce != null ? u.UserIntroduce.CreateAccount : DateTime.MinValue)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetLockedUsersAsync()
        {
            return await _context.Users
                .Where(u => u.UserLockoutEnabled && u.UserLockoutEnd != null && u.UserLockoutEnd > DateTime.UtcNow)
                .OrderByDescending(u => u.UserLockoutEnd)
                .ToListAsync();
        }

        // 使用者統計
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _context.Users
                .Include(u => u.UserRight)
                .CountAsync(u => u.UserRight != null && u.UserRight.UserStatus == true);
        }

        public async Task<int> GetNewUsersCountAsync(DateTime since)
        {
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .CountAsync(u => u.UserIntroduce != null && u.UserIntroduce.CreateAccount >= since);
        }

        public async Task<Dictionary<string, int>> GetUserStatsByDateAsync(int days = 30)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days);
            var users = await _context.Users
                .Include(u => u.UserIntroduce)
                .Where(u => u.UserIntroduce != null && u.UserIntroduce.CreateAccount >= startDate)
                .Select(u => u.UserIntroduce!.CreateAccount.Date)
                .GroupBy(d => d)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            return users.ToDictionary(
                x => x.Date.ToString("yyyy-MM-dd"),
                x => x.Count
            );
        }

        // 使用者權限
        public async Task<bool> GrantRightAsync(int userId, string rightName)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null) return false;

                // 取得或建立使用者權限記錄
                var userRight = await _context.UserRights
                    .FirstOrDefaultAsync(r => r.UserId == userId);

                if (userRight == null)
                {
                    userRight = new GameSpace.Models.UserRight
                    {
                        UserId = userId,
                        UserStatus = true,
                        ShoppingPermission = false,
                        MessagePermission = false,
                        SalesAuthority = false
                    };
                    _context.UserRights.Add(userRight);
                }

                // 根據權限名稱設定對應欄位
                switch (rightName.ToLower())
                {
                    case "shopping":
                        userRight.ShoppingPermission = true;
                        break;
                    case "message":
                        userRight.MessagePermission = true;
                        break;
                    case "sales":
                        userRight.SalesAuthority = true;
                        break;
                    case "status":
                        userRight.UserStatus = true;
                        break;
                    default:
                        return false;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RevokeRightAsync(int userId, string rightName)
        {
            try
            {
                var userRight = await _context.UserRights
                    .FirstOrDefaultAsync(r => r.UserId == userId);

                if (userRight == null) return false;

                // 根據權限名稱撤銷對應欄位
                switch (rightName.ToLower())
                {
                    case "shopping":
                        userRight.ShoppingPermission = false;
                        break;
                    case "message":
                        userRight.MessagePermission = false;
                        break;
                    case "sales":
                        userRight.SalesAuthority = false;
                        break;
                    case "status":
                        userRight.UserStatus = false;
                        break;
                    default:
                        return false;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetUserRightsAsync(int userId)
        {
            var userRight = await _context.UserRights
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (userRight == null) return new List<string>();

            var rights = new List<string>();
            if (userRight.UserStatus == true) rights.Add("Status");
            if (userRight.ShoppingPermission == true) rights.Add("Shopping");
            if (userRight.MessagePermission == true) rights.Add("Message");
            if (userRight.SalesAuthority == true) rights.Add("Sales");

            return rights;
        }

        public async Task<bool> HasRightAsync(int userId, string rightName)
        {
            var userRight = await _context.UserRights
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (userRight == null) return false;

            return rightName.ToLower() switch
            {
                "shopping" => userRight.ShoppingPermission == true,
                "message" => userRight.MessagePermission == true,
                "sales" => userRight.SalesAuthority == true,
                "status" => userRight.UserStatus == true,
                _ => false
            };
        }
    }
}

