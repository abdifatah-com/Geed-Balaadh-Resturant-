using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dalabat.Models;

[Table("Payments")]
public class Payment
{
    [Key]
    public int PaymentID { get; set; }

    public int OrderID { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = "Credit Card"; // Credit Card, Cash, Apple Pay, Wallet

    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string TransactionStatus { get; set; } = "Completed"; // Pending, Completed, Refunded, Failed

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Order? Order { get; set; }
}
