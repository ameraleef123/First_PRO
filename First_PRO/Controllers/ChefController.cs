using First_PRO.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;
using System.Security.Claims;

namespace First_Project_MVC.Controllers
{
    public class ChefController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ChefController(ModelContext context, IWebHostEnvironment webHostEnvironment   )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
     

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userLogin = _context.Users.FirstOrDefault(x => x.Id == userId);
            var totalChefsCount = _context.Users.Count(u => u.RoleId == 2);
            var totalUsersCount = _context.Users.Count(u => u.RoleId == 3);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user?.Username;
            ViewBag.TotalChefsCount = totalChefsCount;
            ViewBag.TotalUsersCount = totalUsersCount;

            return View(userLogin);
        }

        public async Task<IActionResult> Categories()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user?.Username;


            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Categories'  is null.");
        }
       

        public async Task<IActionResult> GetChef()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user?.Username;
            var modelContext = _context.Users
                                        .Include(u => u.Role)
                                        .Where(u => u.RoleId == 2); 
            return View(await modelContext.ToListAsync());
        }

        public async Task<IActionResult> Recipe()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user?.Username;

            IQueryable<Recipe> recipes = _context.Recipes.Where(r=>r.UserId == userId).Include(r => r.Cat).Include(r => r.User);

            return View(await recipes.ToListAsync());
        }


        private decimal GetCurrentChefId()
        {
            throw new NotImplementedException();
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> ChefDetails(decimal? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user1 = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user1?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user1?.Username;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user?.Username;
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Cat)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult ChefCreate()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user1 = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user1?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user1?.Username;
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChefCreate([Bind("Id,CatId,Date,Name,Price,Description,ImageFile")] Recipe recipe)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (ModelState.IsValid)
            {
                var recipeIMG = await _context.Recipes.FirstOrDefaultAsync(x => x.Id == recipe.Id);

                if (recipeIMG != null)
                {
                    recipe.ImagePath = recipeIMG.ImagePath;
                }

                if (recipe.ImageFile != null)
                {
                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + recipe.ImageFile.FileName;
                    string fullPath = Path.Combine(wwwrootPath + "/Images/", imageName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await recipe.ImageFile.CopyToAsync(fileStream);
                    }

                    recipe.ImagePath = imageName;
                }

                recipe.Stuats = 3; 
                recipe.UserId = userId;

                _context.Add(recipe);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Recipe));
            }

            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", recipe.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", recipe.UserId);

            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> ChefEdit(decimal? id)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            var user1 = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user1?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user1?.Username;

            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", recipe.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChefEdit(decimal id, [Bind("Id,CatId,Name,Price,Description,ImageFile")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (recipe.ImageFile != null)
                {
                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + recipe.ImageFile.FileName;
                    string fullPath = Path.Combine(wwwrootPath + "/Images/", imageName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await recipe.ImageFile.CopyToAsync(fileStream);
                    }

                    recipe.ImagePath = imageName;
                }
                try
                {
                    var recipeDb = await _context.Recipes.FindAsync(recipe.Id);
                    recipeDb.Name = recipe.Name;
                    recipeDb.Description = recipe.Description;
                    recipeDb.Price = recipe.Price;
                    recipeDb.CatId = recipe.CatId;
                    recipeDb.Date = DateTime.Now;
                    recipeDb.ImagePath = recipe.ImagePath;
                    _context.Update(recipeDb);
                    await _context.SaveChangesAsync();

                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Recipe));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", recipe.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> ChefDelete(decimal? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user1 = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user1?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user1?.Username;
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Cat)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("ChefDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Recipes == null)
            {
                return Problem("Entity set 'ModelContext.Recipes'  is null.");
            }
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Recipe));
        }

        private bool RecipeExists(decimal id)
        {
            return (_context.Recipes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> GetRecipes(int chefId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user1 = _context.Users.FirstOrDefault(u => u.Id == userId);

            ViewBag.UserImagePath = user1?.ImagePath ?? "~/images/download.png";
            ViewBag.Username = user1?.Username;
            // Retrieve recipes associated with the specified chef ID
            var recipes = await _context.Recipes
                                        .Where(r => r.UserId == chefId)
                                        .Include(r => r.Cat).Include(r => r.User)
                                        .ToListAsync();

            // Pass the recipes to the view
            return View(recipes);
        }
    

    }
}
