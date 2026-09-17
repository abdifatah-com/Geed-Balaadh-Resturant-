using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dalabat.Models;

[Table("Coupons")]
public class Coupon
{
    [Key]
    public int CouponID { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    public decimal DiscountPercentage { get; set; }

    public decimal MaxDiscountAmount { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;
}
