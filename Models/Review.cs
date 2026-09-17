using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dalabat.Models;

[Table("Reviews")]
public class Review
{
    [Key]
    public int ReviewID { get; set; }

    public int UserID { get; set; }

    public int RestaurantID { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public Restaurant? Restaurant { get; set; }
}
