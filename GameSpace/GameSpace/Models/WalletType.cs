using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class WalletType
{
    public int WalletTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedTime { get; set; }
}
