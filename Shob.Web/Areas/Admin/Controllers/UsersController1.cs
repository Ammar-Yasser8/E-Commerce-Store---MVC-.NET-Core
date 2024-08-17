using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mshop.DataAccess;
using Shop.Utilities;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        string userId = claim.Value;   
        var users = await _context.ApplicationUsers
                                  .Where(x => x.Id != userId)
                                  .ToListAsync();

        return View(users);
    }
    public async Task<IActionResult> LockUnlock(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
        {
            // Lock the user
            user.LockoutEnd = DateTime.Now.AddYears(1);
        }
        else
        {
            // Unlock the user
            user.LockoutEnd = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}
