using Microsoft.EntityFrameworkCore;

namespace GameSpace.Models
{
    public partial class GameSpacedatabaseContext
    {
        // Partial class for custom extensions to GameSpacedatabaseContext
        // DbSets are defined in the main GameSpacedatabaseContext.cs file

        // MiniGame Area 專用 DbSets（新增）
        public virtual DbSet<GameSpace.Areas.MiniGame.Models.DailyGameLimit>? DailyGameLimits { get; set; }
        public virtual DbSet<GameSpace.Areas.MiniGame.Models.ErrorLog>? ErrorLogs { get; set; }
        public virtual DbSet<GameSpace.Areas.MiniGame.Models.PetLevelRewardSetting>? PetLevelRewardSettings { get; set; }
        public virtual DbSet<GameSpace.Areas.MiniGame.Services.GameRule>? GameRules { get; set; }
        public virtual DbSet<GameSpace.Areas.MiniGame.Services.GameEventRule>? GameEventRules { get; set; }
        public virtual DbSet<GameSpace.Models.WalletType>? WalletTypes { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // Custom model configurations can be added here if needed
            // Entity configurations are already defined in the main context file

            // ✅ 修正 Pet 和 User 的關聯配置
            modelBuilder.Entity<Pet>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Pet__UserID__6E8B6712");
            });

            // ✅ 修正 UserSignInStat 和 User 的關聯配置
            modelBuilder.Entity<UserSignInStat>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSignInStats)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ✅ 修正 WalletHistory 和 User 的關聯配置
            modelBuilder.Entity<WalletHistory>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.WalletHistories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ✅ 修正 MiniGame 和 User 的關聯配置
            modelBuilder.Entity<MiniGame>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Games)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ✅ 修正 Coupon 的關聯配置
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.CouponType)
                    .WithMany(p => p.Coupons)
                    .HasForeignKey(d => d.CouponTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ✅ 修正 Evoucher 的關聯配置
            modelBuilder.Entity<Evoucher>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.EvoucherType)
                    .WithMany(p => p.Evouchers)
                    .HasForeignKey(d => d.EvoucherTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ✅ 修正 UserWallet 的關聯配置
            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // DailyGameLimits 實體設定
            if (DailyGameLimits != null)
            {
                modelBuilder.Entity<GameSpace.Areas.MiniGame.Models.DailyGameLimit>(entity =>
                {
                    entity.ToTable("DailyGameLimits");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.DailyLimit).IsRequired();
                    entity.Property(e => e.SettingName).HasMaxLength(100).IsRequired();
                    entity.Property(e => e.Description).HasMaxLength(500);
                    entity.Property(e => e.CreatedBy).HasMaxLength(50);
                    entity.Property(e => e.UpdatedBy).HasMaxLength(50);
                });
            }

            // ErrorLogs 實體設定
            if (ErrorLogs != null)
            {
                modelBuilder.Entity<GameSpace.Areas.MiniGame.Models.ErrorLog>(entity =>
                {
                    entity.ToTable("ErrorLogs");
                    entity.HasKey(e => e.LogId);
                    entity.Property(e => e.Level).HasMaxLength(20).IsRequired();
                    entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();
                    entity.Property(e => e.Source).HasMaxLength(255);
                    entity.Property(e => e.RequestPath).HasMaxLength(500);
                    entity.Property(e => e.IpAddress).HasMaxLength(50);
                    entity.Property(e => e.UserAgent).HasMaxLength(500);
                });
            }

            // PetLevelRewardSettings 實體設定
            if (PetLevelRewardSettings != null)
            {
                modelBuilder.Entity<GameSpace.Areas.MiniGame.Models.PetLevelRewardSetting>(entity =>
                {
                    entity.ToTable("PetLevelRewardSettings");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Level).IsRequired();
                    entity.Property(e => e.RewardType).HasMaxLength(50).IsRequired();
                    entity.Property(e => e.RewardAmount).IsRequired();
                    entity.Property(e => e.Description).HasMaxLength(200);
                    entity.Property(e => e.CreatedBy).HasMaxLength(50);
                    entity.Property(e => e.UpdatedBy).HasMaxLength(50);
                });
            }

            // GameRules 實體設定
            if (GameRules != null)
            {
                modelBuilder.Entity<GameSpace.Areas.MiniGame.Services.GameRule>(entity =>
                {
                    entity.ToTable("GameRules");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.RuleName).IsRequired();
                    entity.Property(e => e.RuleType).IsRequired();
                    entity.Property(e => e.RuleValue).IsRequired();
                });
            }

            // GameEventRules 實體設定
            if (GameEventRules != null)
            {
                modelBuilder.Entity<GameSpace.Areas.MiniGame.Services.GameEventRule>(entity =>
                {
                    entity.ToTable("GameEventRules");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.EventName).IsRequired();
                    entity.Property(e => e.EventType).IsRequired();
                    entity.Property(e => e.RewardMultiplier).IsRequired();
                });
            }

            // WalletTypes 實體設定
            if (WalletTypes != null)
            {
                modelBuilder.Entity<GameSpace.Models.WalletType>(entity =>
                {
                    entity.ToTable("WalletTypes");
                    entity.HasKey(e => e.WalletTypeId);
                });
            }
        }
    }
}
