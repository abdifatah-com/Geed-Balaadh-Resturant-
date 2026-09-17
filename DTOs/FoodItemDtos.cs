namespace dalabat.DTOs;

public class CreateFoodItemDto
{
    public int RestaurantID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; } = true;
}

public class UpdateFoodItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; }
}

public class FoodItemResponseDto
{
    public int FoodID { get; set; }
    public int RestaurantID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; }
}
