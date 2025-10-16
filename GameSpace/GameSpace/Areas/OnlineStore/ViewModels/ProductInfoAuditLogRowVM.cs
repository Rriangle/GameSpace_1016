using System;
using System.ComponentModel.DataAnnotations;

//異動紀錄列
namespace GameSpace.Areas.OnlineStore.ViewModels
{
    public class ProductInfoAuditLogRowVM
    {
        public long LogId { get; set; }

        [Display(Name = "動作")]
        public string ActionType { get; set; } = "";// CREATE / UPDATE / DELETE ...

        [Display(Name = "欄位")]
        public string FieldName { get; set; } = "";

        [Display(Name = "舊值")]
        public string? OldValue { get; set; }

        [Display(Name = "新值")]
        public string? NewValue { get; set; }

        [Display(Name = "異動人員ID")]
        public int? ManagerId { get; set; }

        [Display(Name = "異動時間")]
        public DateTime ChangedAt { get; set; }
    }
}
