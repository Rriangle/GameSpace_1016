using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class SignInRewardRule
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
}
