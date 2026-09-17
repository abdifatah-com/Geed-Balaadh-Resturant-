using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;
using dalabat.Models;

namespace dalabat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly DalabatDbContext _context;

    public CategoriesController(DalabatDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return await _context.Categories.Include(c => c.FoodItems).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories.Include(c => c.FoodItems).FirstOrDefaultAsync(c => c.CategoryID == id);
        if (category == null) return NotFound();
        return category;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }
}
