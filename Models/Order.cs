using System.Text.Json.Serialization;

namespace dalabat.Models;

public class Order
{
    public int OrderID { get; set; }
    public int UserID { get; set; }
    public int RestaurantID { get; set; }
    public int? DriverID { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public string OrderStatus { get; set; } = "Pending";
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Restaurant? Restaurant { get; set; }
    
    [JsonIgnore]
    public Driver? Driver { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<OrderProgress> OrderProgresses { get; set; } = new List<OrderProgress>();

    [JsonIgnore]
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
