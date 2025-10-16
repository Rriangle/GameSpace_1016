using Microsoft.EntityFrameworkCore;
using GameSpace.Areas.MiniGame.Models;
using GameSpace.Areas.MiniGame.Models.Settings;

namespace GameSpace.Models
{
    /// <summary>
    /// Partial class extension for GameSpacedatabaseContext
    /// Contains additional DbSets for MiniGame area entities
    /// </summary>
    public partial class GameSpacedatabaseContext : DbContext
    {
        /// <summary>
        /// Sign-in rules configuration table
        /// </summary>
        public virtual DbSet<SignInRule> SignInRules { get; set; }

        /// <summary>
        /// Admin operation logging table
        /// </summary>
        public virtual DbSet<AdminOperationLog> AdminOperationLogs { get; set; }

        /// <summary>
        /// System settings and configuration table
        /// </summary>
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }

        /// <summary>
        /// Pet background change settings table
        /// </summary>
        public virtual DbSet<PetBackgroundChangeSettings> PetBackgroundChangeSettings { get; set; }

        /// <summary>
        /// Pet skin color cost settings table
        /// ⚠️ 修復：表不存在，使用 InMemoryPetSkinColorCostSettingService 替代
        /// </summary>
        // public virtual DbSet<PetSkinColorCostSetting> PetSkinColorCostSettings { get; set; }

        /// <summary>
        /// Pet color change settings table
        /// </summary>
        public virtual DbSet<PetColorChangeSettings> PetColorChangeSettings { get; set; }

        /// <summary>
        /// Pet color options table
        /// </summary>
        public virtual DbSet<PetColorOption> PetColorOptions { get; set; }

        /// <summary>
        /// Pet background options table
        /// </summary>
        public virtual DbSet<PetBackgroundOptionEntity> PetBackgroundOptions { get; set; }

        /// <summary>
        /// Pet level up rules table
        /// </summary>
        public virtual DbSet<PetLevelUpRule> PetLevelUpRules { get; set; }

        /// <summary>
        /// Pet skin color point settings table
        /// </summary>
        public virtual DbSet<PetSkinColorPointSettings> PetSkinColorPointSettings { get; set; }

        /// <summary>
        /// Pet background point settings table
        /// </summary>
        public virtual DbSet<PetBackgroundPointSettings> PetBackgroundPointSettings { get; set; }

        /// <summary>
        /// Pet interaction bonus rules table
        /// </summary>
        public virtual DbSet<PetInteractionBonusRules> PetInteractionBonusRules { get; set; }
    }
}
