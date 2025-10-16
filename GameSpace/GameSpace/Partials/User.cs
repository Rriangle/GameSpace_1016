using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    public partial class User
    {
        // Navigation properties for MiniGame area
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
        public virtual ICollection<WalletHistory> WalletHistories { get; set; } = new List<WalletHistory>();
        public virtual ICollection<UserSignInStat> UserSignInStats { get; set; } = new List<UserSignInStat>();
        public virtual ICollection<MiniGame> Games { get; set; } = new List<MiniGame>();
    }
}
