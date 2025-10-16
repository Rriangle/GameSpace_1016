namespace GameSpace.Areas.OnlineStore.ViewModels
{
    public class ProductDetailModalVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductType { get; set; } = "game"; // or notgame
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; } = "TWD";
        public int? ShipmentQuantity { get; set; }

        public DateTime ProductCreatedAt { get; set; }
        public int? ProductCreatedBy { get; set; }
        public DateTime? ProductUpdatedAt { get; set; }
        public int? ProductUpdatedBy { get; set; }

        public string? ProductCode { get; set; }

        // Game
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int? PlatformId { get; set; }
        public string? PlatformName { get; set; }
        public string? GameType { get; set; }
        public string? DownloadLink { get; set; }
        public string? GameDescription { get; set; }

        // Not-Game
        public int? MerchTypeId { get; set; }
        public string? MerchTypeName { get; set; }
        public string? DigitalCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? Material { get; set; }
        public int? StockQuantity { get; set; }
        public string? OtherDescription { get; set; }

        public List<ImageVM> Images { get; set; } = new();
        public class ImageVM
        {
            public int Id { get; set; }
            public string Url { get; set; } = ""; // Controller 用 Url.Action("Image", ...) 補上
            public string? Alt { get; set; }
        }

        public List<LastLogVM>? LastLogs { get; set; }
        public class LastLogVM
        {
            public long LogId { get; set; }
            public string ActionType { get; set; } = "";
            public string FieldName { get; set; } = "";
            public string? OldValue { get; set; }
            public string? NewValue { get; set; }
            public int? ManagerId { get; set; }
            public DateTime ChangedAt { get; set; }
        }
    }
}
