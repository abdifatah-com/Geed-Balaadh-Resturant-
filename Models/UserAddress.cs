using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dalabat.Models;

[Table("User_Addresses")]
public class UserAddress
{
    [Key]
    public int AddressID { get; set; }

    public int UserID { get; set; }

    [Required]
    [StringLength(50)]
    public string AddressTitle { get; set; } = "Home"; // e.g. Home, Work

    [Required]
    [StringLength(255)]
    public string StreetAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(50)]
    public string? BuildingNo { get; set; }

    [StringLength(20)]
    public string? Floor { get; set; }

    public bool IsDefault { get; set; } = true;

    [JsonIgnore]
    public User? User { get; set; }
}
