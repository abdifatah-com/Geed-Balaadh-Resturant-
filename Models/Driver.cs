using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dalabat.Models;

[Table("Drivers")]
public class Driver
{
    [Key]
    public int DriverID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(50)]
    public string VehicleType { get; set; } = "Motorcycle";

    [StringLength(50)]
    public string LicensePlate { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    [StringLength(100)]
    public string? CurrentLocation { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
