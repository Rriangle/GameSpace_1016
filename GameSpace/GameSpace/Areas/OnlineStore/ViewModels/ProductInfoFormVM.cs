using GameSpace.Models;
using System.ComponentModel.DataAnnotations;
namespace GameSpace.Areas.OnlineStore.ViewModels
{
    public class ProductInfoFormVM
    {
        // === Info ===
        public int ProductId { get; set; }
        [Required] public string ProductName { get; set; } = "";
        public string? ProductType { get; set; } // "game" / "notgame"
        public string? CurrencyCode { get; set; } = "TWD";
        public decimal Price { get; set; }
        public int? ShipmentQuantity { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime ProductCreatedAt { get; set; }
        public int? ProductCreatedBy { get; set; }
        public DateTime? ProductUpdatedAt { get; set; }
        public int? ProductUpdatedBy { get; set; }

        // 共用  Code 不確定要不要用到
        [Required] public int? SupplierId { get; set; }
		public string? ProductCode { get; set; }

		// Game
		public int? PlatformId { get; set; }
        public string? PlatformName { get; set; }
        public string? GameType { get; set; }
        public string? DownloadLink { get; set; }
        public string? GameProductDescription { get; set; }

        // Not-Game
        public int? MerchTypeId { get; set; }
        public string? DigitalCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? Material { get; set; }
        public int? StockQuantity { get; set; }
        public string? OtherProductDescription { get; set; }

        // 圖片（本機）
        [Display(Name = "上傳圖片")]
        public string? ProductImageUrl { get; set; }

        public IFormFile? Image { get; set; }  // 圖片檔案上傳

        public ProductImage? productImage { get; set; }
    
    }



}

