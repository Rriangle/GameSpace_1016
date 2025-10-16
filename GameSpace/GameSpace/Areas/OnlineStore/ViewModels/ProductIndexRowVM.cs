// =========================
// ProductIndexRowVM（表單清單列）
// - 給 Index 表單版用的輕量資料
// - ProductCodeSort：DataTables 用的數值排序碼（從商品代碼中擷取數字）
// =========================
using System;

namespace GameSpace.Areas.OnlineStore.ViewModels
{
    public class ProductIndexRowVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductType { get; set; } = ""; // "game" / "notgame"
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; } = "TWD";
        public int? ShipmentQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime ProductCreatedAt { get; set; }
        public int? CreatedByManagerId { get; set; }

        public string? ProductCode { get; set; }
        public int? ProductCodeSort { get; set; } // 由 Controller 幫忙計算

        public LastLogDto? LastLog { get; set; }
    }

    public class LastLogDto
    {
        public long LogId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
