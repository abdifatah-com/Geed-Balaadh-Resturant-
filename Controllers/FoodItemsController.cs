using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;
using dalabat.DTOs;
using dalabat.Models;

namespace dalabat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodItemsController : ControllerBase
{
    private readonly DalabatDbContext _context;

    public FoodItemsController(DalabatDbContext context)
    {
        _context = context;
    }

    // GET: api/FoodItems?restaurantId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FoodItemResponseDto>>> GetFoodItems([FromQuery] int? restaurantId)
    {
        var query = _context.FoodItems.AsQueryable();

        if (restaurantId.HasValue)
        {
            query = query.Where(f => f.RestaurantID == restaurantId.Value);
        }

        var items = await query
            .Select(f => new FoodItemResponseDto
            {
                FoodID = f.FoodID,
                RestaurantID = f.RestaurantID,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                Availability = f.Availability
            })
            .ToListAsync();

        return Ok(items);
    }

    // GET: api/FoodItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FoodItemResponseDto>> GetFoodItem(int id)
    {
        var f = await _context.FoodItems.FindAsync(id);
        if (f == null)
        {
            return NotFound(new { message = $"Food item with ID {id} not found." });
        }

        var dto = new FoodItemResponseDto
        {
            FoodID = f.FoodID,
            RestaurantID = f.RestaurantID,
            Name = f.Name,
            Description = f.Description,
            Price = f.Price,
            Availability = f.Availability
        };

        return Ok(dto);
    }

    // POST: api/FoodItems
    [HttpPost]
    public async Task<ActionResult<FoodItemResponseDto>> CreateFoodItem(CreateFoodItemDto dto)
    {
        var restaurantExists = await _context.Restaurants.AnyAsync(r => r.RestaurantID == dto.RestaurantID);
        if (!restaurantExists)
        {
            return BadRequest(new { message = $"Restaurant with ID {dto.RestaurantID} does not exist." });
        }

        var foodItem = new FoodItem
        {
            RestaurantID = dto.RestaurantID,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Availability = dto.Availability
        };

        _context.FoodItems.Add(foodItem);
        await _context.SaveChangesAsync();

        var responseDto = new FoodItemResponseDto
        {
            FoodID = foodItem.FoodID,
            RestaurantID = foodItem.RestaurantID,
            Name = foodItem.Name,
            Description = foodItem.Description,
            Price = foodItem.Price,
            Availability = foodItem.Availability
        };

        return CreatedAtAction(nameof(GetFoodItem), new { id = foodItem.FoodID }, responseDto);
    }

    // PUT: api/FoodItems/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFoodItem(int id, UpdateFoodItemDto dto)
    {
        var item = await _context.FoodItems.FindAsync(id);
        if (item == null)
        {
            return NotFound(new { message = $"Food item with ID {id} not found." });
        }

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Price = dto.Price;
        item.Availability = dto.Availability;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/FoodItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFoodItem(int id)
    {
        var item = await _context.FoodItems.FindAsync(id);
        if (item == null)
        {
            return NotFound(new { message = $"Food item with ID {id} not found." });
        }

        _context.FoodItems.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Food item {id} deleted successfully." });
    }
}
