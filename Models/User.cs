using System.Text.Json.Serialization;

namespace dalabat.Models;

public class User
{
    public int UserID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    [JsonIgnore]
    public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();

    [JsonIgnore]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    [JsonIgnore]
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
