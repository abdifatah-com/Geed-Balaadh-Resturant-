using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;
using dalabat.DTOs;
using dalabat.Models;

namespace dalabat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly DalabatDbContext _context;

    public OrdersController(DalabatDbContext context)
    {
        _context = context;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
            .Include(o => o.OrderProgresses)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => MapToOrderResponse(o))
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
            .Include(o => o.OrderProgresses)
            .FirstOrDefaultAsync(o => o.OrderID == id);

        if (order == null)
        {
            return NotFound(new { message = $"Order with ID {id} not found." });
        }

        return Ok(MapToOrderResponse(order));
    }

    // GET: api/Orders/user/1
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetUserOrders(int userId)
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
            .Include(o => o.OrderProgresses)
            .Where(o => o.UserID == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => MapToOrderResponse(o))
            .ToListAsync();

        return Ok(orders);
    }

    // POST: api/Orders
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
        {
            return BadRequest(new { message = "Order must contain at least one item." });
        }

        var user = await _context.Users.FindAsync(dto.UserID);
        if (user == null)
        {
            return BadRequest(new { message = $"User with ID {dto.UserID} not found." });
        }

        var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantID);
        if (restaurant == null)
        {
            return BadRequest(new { message = $"Restaurant with ID {dto.RestaurantID} not found." });
        }

        var foodIds = dto.Items.Select(i => i.FoodID).ToList();
        var foodItemsMap = await _context.FoodItems
            .Where(f => foodIds.Contains(f.FoodID))
            .ToDictionaryAsync(f => f.FoodID);

        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            if (!foodItemsMap.TryGetValue(item.FoodID, out var foodItem))
            {
                return BadRequest(new { message = $"Food item ID {item.FoodID} was not found." });
            }

            var subtotal = foodItem.Price * item.Quantity;
            totalAmount += subtotal;

            orderItems.Add(new OrderItem
            {
                FoodID = item.FoodID,
                Quantity = item.Quantity,
                Price = foodItem.Price
            });
        }

        var order = new Order
        {
            UserID = dto.UserID,
            RestaurantID = dto.RestaurantID,
            TotalAmount = totalAmount,
            PaymentStatus = dto.PaymentStatus,
            OrderStatus = "Placed",
            OrderDate = DateTime.UtcNow,
            OrderItems = orderItems,
            OrderProgresses = new List<OrderProgress>
            {
                new OrderProgress
                {
                    Status = "Placed",
                    Description = "Order received by system",
                    UpdatedAt = DateTime.UtcNow
                }
            }
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Reload to include navigation properties
        var createdOrder = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
            .Include(o => o.OrderProgresses)
            .FirstAsync(o => o.OrderID == order.OrderID);

        return Ok(MapToOrderResponse(createdOrder));
    }

    // PUT: api/Orders/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.OrderProgresses)
            .FirstOrDefaultAsync(o => o.OrderID == id);

        if (order == null)
        {
            return NotFound(new { message = $"Order with ID {id} not found." });
        }

        order.OrderStatus = dto.Status;
        order.OrderProgresses.Add(new OrderProgress
        {
            OrderID = id,
            Status = dto.Status,
            Description = dto.Description ?? $"Status updated to {dto.Status}",
            UpdatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(new { message = $"Order status updated to '{dto.Status}'" });
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound(new { message = $"Order with ID {id} not found." });
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Order {id} cancelled and deleted." });
    }

    private static OrderResponseDto MapToOrderResponse(Order o)
    {
        return new OrderResponseDto
        {
            OrderID = o.OrderID,
            UserID = o.UserID,
            UserName = o.User?.Name ?? string.Empty,
            RestaurantID = o.RestaurantID,
            RestaurantName = o.Restaurant?.Name ?? string.Empty,
            TotalAmount = o.TotalAmount,
            PaymentStatus = o.PaymentStatus,
            OrderStatus = o.OrderStatus,
            OrderDate = o.OrderDate,
            Items = o.OrderItems.Select(oi => new OrderItemResponseDto
            {
                OrderItemID = oi.OrderItemID,
                FoodID = oi.FoodID,
                FoodName = oi.FoodItem?.Name ?? string.Empty,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList(),
            ProgressHistory = o.OrderProgresses.OrderBy(p => p.UpdatedAt).Select(p => new OrderProgressResponseDto
            {
                ProgressID = p.ProgressID,
                Status = p.Status,
                Description = p.Description,
                UpdatedAt = p.UpdatedAt
            }).ToList()
        };
    }
}
