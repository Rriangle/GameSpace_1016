namespace GameSpace.Areas.OnlineStore.ViewModels
{
    public class SupplierProductVM
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        // 若你的欄位叫 Code，改成 string? Code 並同步調整 Controller
        public string? ProductCode { get; set; }

        // "遊戲商品" / "周邊商品"
        public string Category { get; set; } = string.Empty;
    }
}
