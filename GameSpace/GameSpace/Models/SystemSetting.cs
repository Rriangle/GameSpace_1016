using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 系統設定實體
    /// </summary>
    [Table("SystemSettings")]
    public class SystemSetting
    {
        /// <summary>
        /// 設定 ID
        /// </summary>
        [Key]
        public int SettingId { get; set; }

        /// <summary>
        /// 設定鍵值（唯一）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        /// <summary>
        /// 設定值（支援 JSON）
        /// </summary>
        public string? SettingValue { get; set; }

        /// <summary>
        /// 設定描述
        /// </summary>
        [StringLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// 設定類型（String, Number, Boolean, JSON）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SettingType { get; set; } = "String";

        /// <summary>
        /// 是否唯讀
        /// </summary>
        [Required]
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 更新者 ID
        /// </summary>
        public int? UpdatedBy { get; set; }

        // Property aliases for compatibility
        [NotMapped]
        public string Key { get => SettingKey; set => SettingKey = value; }

        [NotMapped]
        public string? Value { get => SettingValue; set => SettingValue = value; }

        [NotMapped]
        public DateTime CreatedTime { get => CreatedAt; set => CreatedAt = value; }

        [NotMapped]
        public DateTime? UpdatedTime { get => UpdatedAt; set => UpdatedAt = value; }
    }
}
