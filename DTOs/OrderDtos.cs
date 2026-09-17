namespace dalabat.DTOs;

public class CreateOrderItemDto
{
    public int FoodID { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderDto
{
    public int UserID { get; set; }
    public int RestaurantID { get; set; }
    public string PaymentStatus { get; set; } = "Paid";
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class OrderItemResponseDto
{
    public int OrderItemID { get; set; }
    public int FoodID { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal ItemSubtotal => Quantity * Price;
}

public class OrderProgressResponseDto
{
    public int ProgressID { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderResponseDto
{
    public int OrderID { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int RestaurantID { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
    public List<OrderProgressResponseDto> ProgressHistory { get; set; } = new();
}
