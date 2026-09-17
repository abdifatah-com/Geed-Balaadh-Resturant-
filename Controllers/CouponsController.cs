using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;
using dalabat.Models;

namespace dalabat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly DalabatDbContext _context;

    public CouponsController(DalabatDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
    {
        return await _context.Coupons.Where(c => c.IsActive && c.ExpiryDate > DateTime.UtcNow).ToListAsync();
    }

    [HttpGet("validate/{code}")]
    public async Task<ActionResult<Coupon>> ValidateCoupon(string code)
    {
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code.ToUpper() == code.ToUpper() && c.IsActive && c.ExpiryDate > DateTime.UtcNow);

        if (coupon == null) return NotFound(new { message = "Invalid or expired coupon code." });
        return coupon;
    }

    [HttpPost]
    public async Task<ActionResult<Coupon>> CreateCoupon(Coupon coupon)
    {
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();
        return Ok(coupon);
    }
}
