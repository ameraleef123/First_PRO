using First_PRO.Models;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace First_PRO.Controllers
{
    public class GuestController : Controller
    {
        private readonly ModelContext _context;

        public GuestController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guest guest)
        {
            if (ModelState.IsValid)
            {
                guest.Date = DateTime.Now;
                _context.Add(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home"); 
            }

            return View(guest); 
        }
    }
}
