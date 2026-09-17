using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;
using dalabat.DTOs;
using dalabat.Models;

namespace dalabat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly DalabatDbContext _context;

    public RestaurantsController(DalabatDbContext context)
    {
        _context = context;
    }

    // GET: api/Restaurants
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantResponseDto>>> GetRestaurants()
    {
        var restaurants = await _context.Restaurants
            .Include(r => r.FoodItems)
            .Select(r => new RestaurantResponseDto
            {
                RestaurantID = r.RestaurantID,
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                MenuItems = r.FoodItems.Select(f => new FoodItemResponseDto
                {
                    FoodID = f.FoodID,
                    RestaurantID = f.RestaurantID,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    Availability = f.Availability
                }).ToList()
            })
            .ToListAsync();

        return Ok(restaurants);
    }

    // GET: api/Restaurants/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RestaurantResponseDto>> GetRestaurant(int id)
    {
        var r = await _context.Restaurants
            .Include(r => r.FoodItems)
            .FirstOrDefaultAsync(r => r.RestaurantID == id);

        if (r == null)
        {
            return NotFound(new { message = $"Restaurant with ID {id} not found." });
        }

        var dto = new RestaurantResponseDto
        {
            RestaurantID = r.RestaurantID,
            Name = r.Name,
            Address = r.Address,
            Phone = r.Phone,
            MenuItems = r.FoodItems.Select(f => new FoodItemResponseDto
            {
                FoodID = f.FoodID,
                RestaurantID = f.RestaurantID,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                Availability = f.Availability
            }).ToList()
        };

        return Ok(dto);
    }

    // POST: api/Restaurants
    [HttpPost]
    public async Task<ActionResult<RestaurantResponseDto>> CreateRestaurant(CreateRestaurantDto dto)
    {
        var restaurant = new Restaurant
        {
            Name = dto.Name,
            Address = dto.Address,
            Phone = dto.Phone
        };

        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        var responseDto = new RestaurantResponseDto
        {
            RestaurantID = restaurant.RestaurantID,
            Name = restaurant.Name,
            Address = restaurant.Address,
            Phone = restaurant.Phone,
            MenuItems = new List<FoodItemResponseDto>()
        };

        return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.RestaurantID }, responseDto);
    }

    // PUT: api/Restaurants/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(int id, UpdateRestaurantDto dto)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null)
        {
            return NotFound(new { message = $"Restaurant with ID {id} not found." });
        }

        restaurant.Name = dto.Name;
        restaurant.Address = dto.Address;
        restaurant.Phone = dto.Phone;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Restaurants/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null)
        {
            return NotFound(new { message = $"Restaurant with ID {id} not found." });
        }

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Restaurant {id} deleted successfully." });
    }
}
