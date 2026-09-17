using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;
using dalabat.Models;

namespace dalabat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly DalabatDbContext _context;

    public ReviewsController(DalabatDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] int? restaurantId)
    {
        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Restaurant)
            .AsQueryable();

        if (restaurantId.HasValue)
        {
            query = query.Where(r => r.RestaurantID == restaurantId.Value);
        }

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.ReviewID,
                r.UserID,
                UserName = r.User != null ? r.User.Name : "Valued Customer",
                r.RestaurantID,
                RestaurantName = r.Restaurant != null ? r.Restaurant.Name : "Dalabat Restaurant",
                r.Rating,
                r.Comment,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpPost]
    public async Task<ActionResult<Review>> CreateReview(Review review)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserID == review.UserID);
        if (!userExists) return BadRequest(new { message = "Invalid UserID." });

        var restaurantExists = await _context.Restaurants.AnyAsync(r => r.RestaurantID == review.RestaurantID);
        if (!restaurantExists) return BadRequest(new { message = "Invalid RestaurantID." });

        review.CreatedAt = DateTime.UtcNow;
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(review);
    }
}
