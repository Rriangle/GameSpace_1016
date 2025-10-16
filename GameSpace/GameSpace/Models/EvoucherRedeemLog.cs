using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class EvoucherRedeemLog
{
    public int RedeemId { get; set; }

    public int EvoucherId { get; set; }

    public int? TokenId { get; set; }

    public int UserId { get; set; }

    public DateTime ScannedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
