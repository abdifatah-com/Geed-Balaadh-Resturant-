using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;
using dalabat.Models;

namespace dalabat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly DalabatDbContext _context;

    public ReservationsController(DalabatDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations([FromQuery] int? restaurantId)
    {
        var query = _context.Reservations.AsQueryable();
        if (restaurantId.HasValue)
        {
            query = query.Where(r => r.RestaurantID == restaurantId.Value);
        }
        return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();
        return reservation;
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> CreateReservation(Reservation reservation)
    {
        var restaurantExists = await _context.Restaurants.AnyAsync(r => r.RestaurantID == reservation.RestaurantID);
        if (!restaurantExists) return BadRequest(new { message = "Invalid RestaurantID." });

        reservation.CreatedAt = DateTime.UtcNow;
        if (string.IsNullOrEmpty(reservation.Status)) reservation.Status = "Confirmed";

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Ok(reservation);
    }
}
