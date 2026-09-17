using System.Text.Json.Serialization;

namespace dalabat.Models;

public class FoodItem
{
    public int FoodID { get; set; }
    public int RestaurantID { get; set; }
    public int? CategoryID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; } = true;

    [JsonIgnore]
    public Restaurant? Restaurant { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }

    [JsonIgnore]
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
