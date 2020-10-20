namespace Hydra.Basket.Domain.Entities
{
    public class Voucher
    {
        public string Code { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountPrice { get; set; }
        public VoucherDiscountType DiscountType { get; set; }
    }

    public enum VoucherDiscountType
    {
        Percentage = 0,
        Price = 1
    }
}