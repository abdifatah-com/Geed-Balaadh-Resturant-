using System.Text.Json.Serialization;

namespace dalabat.Models;

public class OrderProgress
{
    public int ProgressID { get; set; }
    public int OrderID { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Order? Order { get; set; }
}
