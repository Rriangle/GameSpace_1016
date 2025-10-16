using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace GameSpace.Areas.OnlineStore.ViewModels { 
public class ShippingAddressEditViewModels
{
	public int OrderId { get; set; }

	[Required, StringLength(100)]
	public string Recipient { get; set; } = null!;

	[Required, StringLength(30)]
	public string Phone { get; set; } = null!;

	[Required, StringLength(10)]
	public string Zipcode { get; set; } = null!;

	[Required, StringLength(200)]
	public string Address1 { get; set; } = null!;

	[StringLength(200)]
	public string? Address2 { get; set; }

	[Required, StringLength(50)]
	public string City { get; set; } = null!;

	[Required, StringLength(30)]
	public string Country { get; set; } = null!;

	[BindNever]           // ★ CHANGED: 不從表單繫結
	[ValidateNever]       // ★ CHANGED: 不做模型驗證
	public string? OrderStatus { get; set; }
} // ★ CHA
}