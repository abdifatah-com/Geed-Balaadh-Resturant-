using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dalabat.Models;

[Table("Reservations")]
public class Reservation
{
    [Key]
    public int ReservationID { get; set; }

    public int? UserID { get; set; }

    public int RestaurantID { get; set; }

    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; }

    public int GuestCount { get; set; } = 2;

    [StringLength(500)]
    public string? SpecialRequest { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Confirmed"; // Pending, Confirmed, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public Restaurant? Restaurant { get; set; }
}
