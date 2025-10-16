namespace GameSpace.Areas.OnlineStore.ViewModels
{
    public class SupplierVM
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;

        // 是否合作
        public bool IsActive { get; set; } = true;
    }

}
