using System.Text.Json.Serialization;

namespace dalabat.Models;

public class Restaurant
{
    public int RestaurantID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();

    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    [JsonIgnore]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    [JsonIgnore]
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
