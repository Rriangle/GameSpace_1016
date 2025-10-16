using GameSpace.Areas.MiniGame.Models;
using GameSpace.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GameSpace.Areas.MiniGame.Services
{
    public class CouponService : ICouponService
    {
        private readonly string _connectionString;

        public CouponService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<Coupon>> GetAllCouponsAsync()
        {
            var coupons = new List<Coupon>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT c.*, ct.Name as CouponTypeName
                FROM Coupon c
                LEFT JOIN CouponType ct ON c.CouponTypeId = ct.CouponTypeId
                ORDER BY c.AcquiredTime DESC", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                coupons.Add(new Coupon
                {
                    CouponId = reader.GetInt32("CouponId"),
                    CouponCode = reader.GetString("CouponCode"),
                    CouponTypeId = reader.GetInt32("CouponTypeId"),
                    UserId = reader.GetInt32("UserId"),
                    IsUsed = reader.GetBoolean("IsUsed"),
                    AcquiredTime = reader.GetDateTime("AcquiredTime"),
                    UsedTime = reader.IsDBNull("UsedTime") ? null : reader.GetDateTime("UsedTime"),
                    UsedInOrderId = reader.IsDBNull("UsedInOrderId") ? null : reader.GetInt32("UsedInOrderId")
                });
            }
            return coupons;
        }

        public async Task<IEnumerable<Coupon>> GetCouponsByUserIdAsync(int userId)
        {
            var coupons = new List<Coupon>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT c.*, ct.Name as CouponTypeName
                FROM Coupon c
                LEFT JOIN CouponType ct ON c.CouponTypeId = ct.CouponTypeId
                WHERE c.UserId = @UserId
                ORDER BY c.AcquiredTime DESC", connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                coupons.Add(new Coupon
                {
                    CouponId = reader.GetInt32("CouponId"),
                    CouponCode = reader.GetString("CouponCode"),
                    CouponTypeId = reader.GetInt32("CouponTypeId"),
                    UserId = reader.GetInt32("UserId"),
                    IsUsed = reader.GetBoolean("IsUsed"),
                    AcquiredTime = reader.GetDateTime("AcquiredTime"),
                    UsedTime = reader.IsDBNull("UsedTime") ? null : reader.GetDateTime("UsedTime"),
                    UsedInOrderId = reader.IsDBNull("UsedInOrderId") ? null : reader.GetInt32("UsedInOrderId")
                });
            }
            return coupons;
        }

        public async Task<Coupon?> GetCouponByIdAsync(int couponId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT * FROM Coupon WHERE CouponId = @CouponId", connection);
            command.Parameters.AddWithValue("@CouponId", couponId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Coupon
                {
                    CouponId = reader.GetInt32("CouponId"),
                    CouponCode = reader.GetString("CouponCode"),
                    CouponTypeId = reader.GetInt32("CouponTypeId"),
                    UserId = reader.GetInt32("UserId"),
                    IsUsed = reader.GetBoolean("IsUsed"),
                    AcquiredTime = reader.GetDateTime("AcquiredTime"),
                    UsedTime = reader.IsDBNull("UsedTime") ? null : reader.GetDateTime("UsedTime"),
                    UsedInOrderId = reader.IsDBNull("UsedInOrderId") ? null : reader.GetInt32("UsedInOrderId")
                };
            }
            return null;
        }

        public async Task<Coupon?> GetCouponByCodeAsync(string couponCode)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT * FROM Coupon WHERE CouponCode = @CouponCode", connection);
            command.Parameters.AddWithValue("@CouponCode", couponCode);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Coupon
                {
                    CouponId = reader.GetInt32("CouponId"),
                    CouponCode = reader.GetString("CouponCode"),
                    CouponTypeId = reader.GetInt32("CouponTypeId"),
                    UserId = reader.GetInt32("UserId"),
                    IsUsed = reader.GetBoolean("IsUsed"),
                    AcquiredTime = reader.GetDateTime("AcquiredTime"),
                    UsedTime = reader.IsDBNull("UsedTime") ? null : reader.GetDateTime("UsedTime"),
                    UsedInOrderId = reader.IsDBNull("UsedInOrderId") ? null : reader.GetInt32("UsedInOrderId")
                };
            }
            return null;
        }

        public async Task<bool> CreateCouponAsync(Coupon coupon)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                INSERT INTO Coupon (CouponCode, CouponTypeId, UserId, IsUsed, AcquiredTime, UsedTime, UsedInOrderId)
                VALUES (@CouponCode, @CouponTypeId, @UserId, @IsUsed, @AcquiredTime, @UsedTime, @UsedInOrderId)", connection);

            command.Parameters.AddWithValue("@CouponCode", coupon.CouponCode);
            command.Parameters.AddWithValue("@CouponTypeId", coupon.CouponTypeId);
            command.Parameters.AddWithValue("@UserId", coupon.UserId);
            command.Parameters.AddWithValue("@IsUsed", coupon.IsUsed);
            command.Parameters.AddWithValue("@AcquiredTime", coupon.AcquiredTime);
            command.Parameters.AddWithValue("@UsedTime", (object?)coupon.UsedTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@UsedInOrderId", (object?)coupon.UsedInOrderId ?? DBNull.Value);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> UpdateCouponAsync(Coupon coupon)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE Coupon SET
                    CouponCode = @CouponCode,
                    CouponTypeId = @CouponTypeId,
                    UserId = @UserId,
                    IsUsed = @IsUsed,
                    AcquiredTime = @AcquiredTime,
                    UsedTime = @UsedTime,
                    UsedInOrderId = @UsedInOrderId
                WHERE CouponId = @CouponId", connection);

            command.Parameters.AddWithValue("@CouponId", coupon.CouponId);
            command.Parameters.AddWithValue("@CouponCode", coupon.CouponCode);
            command.Parameters.AddWithValue("@CouponTypeId", coupon.CouponTypeId);
            command.Parameters.AddWithValue("@UserId", coupon.UserId);
            command.Parameters.AddWithValue("@IsUsed", coupon.IsUsed);
            command.Parameters.AddWithValue("@AcquiredTime", coupon.AcquiredTime);
            command.Parameters.AddWithValue("@UsedTime", (object?)coupon.UsedTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@UsedInOrderId", (object?)coupon.UsedInOrderId ?? DBNull.Value);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> DeleteCouponAsync(int couponId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM Coupon WHERE CouponId = @CouponId", connection);
            command.Parameters.AddWithValue("@CouponId", couponId);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> UseCouponAsync(int couponId, int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE Coupon SET
                    IsUsed = 1,
                    UsedTime = GETDATE(),
                    UsedInOrderId = @OrderId
                WHERE CouponId = @CouponId AND IsUsed = 0", connection);

            command.Parameters.AddWithValue("@CouponId", couponId);
            command.Parameters.AddWithValue("@OrderId", orderId);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<IEnumerable<CouponType>> GetAllCouponTypesAsync()
        {
            var couponTypes = new List<CouponType>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT * FROM CouponType ORDER BY Name", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                couponTypes.Add(new CouponType
                {
                    CouponTypeId = reader.GetInt32("CouponTypeId"),
                    Name = reader.GetString("Name"),
                    DiscountType = reader.GetString("DiscountType"),
                    DiscountValue = reader.GetDecimal("DiscountValue"),
                    MinSpend = reader.GetDecimal("MinSpend"),
                    ValidFrom = reader.GetDateTime("ValidFrom"),
                    ValidTo = reader.GetDateTime("ValidTo"),
                    PointsCost = reader.GetInt32("PointsCost"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description")
                });
            }
            return couponTypes;
        }

        public async Task<CouponType?> GetCouponTypeByIdAsync(int couponTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT * FROM CouponType WHERE CouponTypeId = @CouponTypeId", connection);
            command.Parameters.AddWithValue("@CouponTypeId", couponTypeId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CouponType
                {
                    CouponTypeId = reader.GetInt32("CouponTypeId"),
                    Name = reader.GetString("Name"),
                    DiscountType = reader.GetString("DiscountType"),
                    DiscountValue = reader.GetDecimal("DiscountValue"),
                    MinSpend = reader.GetDecimal("MinSpend"),
                    ValidFrom = reader.GetDateTime("ValidFrom"),
                    ValidTo = reader.GetDateTime("ValidTo"),
                    PointsCost = reader.GetInt32("PointsCost"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description")
                };
            }
            return null;
        }

        public async Task<bool> CreateCouponTypeAsync(CouponType couponType)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                INSERT INTO CouponType (Name, DiscountType, DiscountValue, MinSpend, ValidFrom, ValidTo, PointsCost, Description)
                VALUES (@Name, @DiscountType, @DiscountValue, @MinSpend, @ValidFrom, @ValidTo, @PointsCost, @Description)", connection);

            command.Parameters.AddWithValue("@Name", couponType.Name);
            command.Parameters.AddWithValue("@DiscountType", couponType.DiscountType);
            command.Parameters.AddWithValue("@DiscountValue", couponType.DiscountValue);
            command.Parameters.AddWithValue("@MinSpend", couponType.MinSpend);
            command.Parameters.AddWithValue("@ValidFrom", couponType.ValidFrom);
            command.Parameters.AddWithValue("@ValidTo", couponType.ValidTo);
            command.Parameters.AddWithValue("@PointsCost", couponType.PointsCost);
            command.Parameters.AddWithValue("@Description", (object?)couponType.Description ?? DBNull.Value);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> UpdateCouponTypeAsync(CouponType couponType)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE CouponType SET
                    Name = @Name,
                    DiscountType = @DiscountType,
                    DiscountValue = @DiscountValue,
                    MinSpend = @MinSpend,
                    ValidFrom = @ValidFrom,
                    ValidTo = @ValidTo,
                    PointsCost = @PointsCost,
                    Description = @Description
                WHERE CouponTypeId = @CouponTypeId", connection);

            command.Parameters.AddWithValue("@CouponTypeId", couponType.CouponTypeId);
            command.Parameters.AddWithValue("@Name", couponType.Name);
            command.Parameters.AddWithValue("@DiscountType", couponType.DiscountType);
            command.Parameters.AddWithValue("@DiscountValue", couponType.DiscountValue);
            command.Parameters.AddWithValue("@MinSpend", couponType.MinSpend);
            command.Parameters.AddWithValue("@ValidFrom", couponType.ValidFrom);
            command.Parameters.AddWithValue("@ValidTo", couponType.ValidTo);
            command.Parameters.AddWithValue("@PointsCost", couponType.PointsCost);
            command.Parameters.AddWithValue("@Description", (object?)couponType.Description ?? DBNull.Value);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> DeleteCouponTypeAsync(int couponTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM CouponType WHERE CouponTypeId = @CouponTypeId", connection);
            command.Parameters.AddWithValue("@CouponTypeId", couponTypeId);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> GrantCouponToUserAsync(int userId, int couponTypeId)
        {
            // Generate unique coupon code
            var couponCode = GenerateCouponCode();
            
            var coupon = new Coupon
            {
                CouponCode = couponCode,
                CouponTypeId = couponTypeId,
                UserId = userId,
                IsUsed = false,
                AcquiredTime = DateTime.Now
            };

            return await CreateCouponAsync(coupon);
        }

        private string GenerateCouponCode()
        {
            var random = new Random();
            var yearMonth = DateTime.Now.ToString("yyMM");
            var randomCode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"CPN-{yearMonth}-{randomCode}";
        }
    }
}

