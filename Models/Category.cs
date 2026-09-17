using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dalabat.Models;

[Table("Categories")]
public class Category
{
    [Key]
    public int CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? IconUrl { get; set; }

    // Navigation properties
    public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
}
