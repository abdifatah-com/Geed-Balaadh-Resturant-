using System.Text.Json.Serialization;

namespace dalabat.Models;

public class OrderItem
{
    public int OrderItemID { get; set; }
    public int OrderID { get; set; }
    public int FoodID { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    [JsonIgnore]
    public Order? Order { get; set; }

    public FoodItem? FoodItem { get; set; }
}
