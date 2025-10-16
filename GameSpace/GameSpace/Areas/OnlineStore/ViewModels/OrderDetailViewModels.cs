using GameSpace.Models;

namespace GameSpace.Areas.OnlineStore.ViewModels
{
	public class OrderDetailViewModels
	{
		//訂單基本資料OrderBasicInfo
		public long OrderCode { get; set; }

		public int OrderId { get; set; }

		public int UserId { get; set; }

		public decimal OrderTotal { get; set; }

		public string OrderStatus { get; set; } = null!;

		public DateTime OrderDate { get; set; }

		public string PaymentStatus { get; set; } = null!;

		public long? PaymentCode { get; set; }

		public DateTime? PaymentAt { get; set; }

		public string Status { get; set; } = null!;//出貨狀態

		public long? ShipmentCode { get; set; }

		public string? TrackingNo { get; set; }//出貨追蹤號

		public DateTime? ShippedAt { get; set; }

		public DateTime? CompletedAt { get; set; }

		//訂單物品資料OrderItemInfoes

		public int LineNo { get; set; }

		public int ProductId { get; set; }

		public string ProductName { get; set; } = null!;

		public decimal UnitPrice { get; set; }

		public int Quantity { get; set; }

		public decimal? Subtotal { get; set; }

		//出貨地址
		public string Recipient { get; set; } = null!;

		public string Phone { get; set; } = null!;

		public string Zipcode { get; set; } = null!;

		public string Address1 { get; set; } = null!;

		public string? Address2 { get; set; }

		public string City { get; set; } = null!;

		public string Country { get; set; } = null!;

		//商品

		public class OrderItemRowVM
		{
			public int ProductId { get; set; }              // 來源：OrderItems.ProductId

			public string? ProductCode { get; set; }		// 來源：ProductCode.product_code
			public string ProductName { get; set; } = "";   // 來源：ProductInfos.ProductName（若沒有就顯示 ProductId）
			public int Quantity { get; set; }               // 來源：OrderItems.Quantity
			public decimal UnitPrice { get; set; }          // 來源：OrderItems.UnitPrice
			public decimal Subtotal { get; set; }           // 來源：Quantity * UnitPrice（或 OrderItems.Subtotal）
		}

		public List<OrderItemRowVM> Items { get; set; } = new(); // 訂單的所有明細


	}
}
