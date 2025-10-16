using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models;

public partial class SignInRule
{
    public int Id { get; set; }

    public int SignInDay { get; set; }

    public int Points { get; set; }

    public int Experience { get; set; }

    public bool HasCoupon { get; set; }

    public string? CouponTypeCode { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Description { get; set; }

    // Property aliases for compatibility
    [NotMapped]
    public int DayNumber { get => SignInDay; set => SignInDay = value; }
}
