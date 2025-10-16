using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 管理員操作日誌實體
    /// </summary>
    [Table("AdminOperationLog")]
    public class AdminOperationLog
    {
        /// <summary>
        /// 日誌 ID
        /// </summary>
        [Key]
        public int LogId { get; set; }

        /// <summary>
        /// 管理員 ID
        /// </summary>
        [Required]
        public int ManagerId { get; set; }

        /// <summary>
        /// 操作類型（Create, Update, Delete, Grant, Revoke 等）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string OperationType { get; set; } = string.Empty;

        /// <summary>
        /// 操作描述
        /// </summary>
        [Required]
        [StringLength(200)]
        public string OperationDescription { get; set; } = string.Empty;

        /// <summary>
        /// 操作詳細資訊（JSON 格式）
        /// </summary>
        public string? OperationDetails { get; set; }

        /// <summary>
        /// 操作時間
        /// </summary>
        [Required]
        public DateTime OperationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// IP 位址
        /// </summary>
        [StringLength(50)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// User Agent
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// 管理員導航屬性
        /// </summary>
        [ForeignKey(nameof(ManagerId))]
        public virtual ManagerDatum? Manager { get; set; }

        // Property aliases for compatibility
        [NotMapped]
        public string Operation { get => OperationType; set => OperationType = value; }

        [NotMapped]
        public string Details { get => OperationDetails ?? string.Empty; set => OperationDetails = value; }

        [NotMapped]
        public int? TargetUserId { get; set; }
    }
}
