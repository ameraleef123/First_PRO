using First_PRO.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace First_PRO.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public AdminController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("Username") ?? "Guest";
            var role = HttpContext.Session.GetString("Role") ?? "Guest";
            var totalUsersCount = _context.Users.Count();
            var totalChefsCount = _context.Users.Count(u => u.RoleId == 2);
            var totalRecipesCount = _context.Recipes.Count();


            ViewBag.TotalUsersCount = totalUsersCount;
            ViewBag.TotalChefsCount = totalChefsCount;
            ViewBag.TotalRecipesCount = totalRecipesCount;
            ViewBag.UserName = userName;
            ViewBag.Role = role;
            ViewBag.userIdValue = HttpContext.Session.GetInt32("UserId");
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "LoginAndRegister");
        }

        public async Task<IActionResult> Categories()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Categories'  is null.");
        }
        public async Task<IActionResult> DetailsCat(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        public IActionResult CreateCat()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCat([Bind("Id,Name,ImageFile")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ImageFile != null)
                {
                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
                    string fullPath = Path.Combine(wwwrootPath + "/Images/", imageName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        category.ImageFile.CopyToAsync(fileStream);
                    }
                    category.ImagePath = imageName;
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }
        // GET: Categories/Edit/5
        public async Task<IActionResult> EditCat(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCat(decimal id, [Bind("Id,Name,ImageFile")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var catIMG = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

                if (catIMG != null)
                {
                    category.ImagePath = catIMG.ImagePath;
                }

                if (category.ImageFile != null)
                {
                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
                    string fullPath = Path.Combine(wwwrootPath + "/Images/", imageName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await category.ImageFile.CopyToAsync(fileStream);
                    }

                    category.ImagePath = imageName;
                }
                _context.Entry(catIMG).State = EntityState.Detached;
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> DeleteCat(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("DeleteCat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ModelContext.Categories'  is null.");
            }



            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        private bool CategoryExists(decimal id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> GetUser()
        {
            var modelContext = _context.Users.Include(u => u.Role).Where(u => u.RoleId == 2 || u.RoleId == 3);
            return View(await modelContext.ToListAsync());
        }
        // GET: Users/Details/5
        public async Task<IActionResult> UserDetails(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        // GET: Users/Edit/5
        public async Task<IActionResult> UserEdit(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(decimal id, [Bind("Id,Username,PhoneNumber,Email,Address")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userDb = await _context.Users.FindAsync(user.Id);
                    userDb.Username = user.Username;
                    userDb.PhoneNumber = user.PhoneNumber;
                    userDb.Email = user.Email;
                    userDb.Date = DateTime.Now;
                    userDb.Address = user.Address;
                    _context.Update(userDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetUser));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> UserDelete(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedUSer(decimal id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ModelContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetUser));
        }

        private bool UserExists(decimal id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Recipes()
        {
            IQueryable<Recipe> recipes = _context.Recipes.Include(r => r.Cat).Include(r => r.User);



            return View(await recipes.ToListAsync());
        }
        // GET: Recipes/Edit/5
        public async Task<IActionResult> EditRecipe(decimal? id)
        {
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
        public async Task<IActionResult> EditRecipe(decimal id, [Bind("Id,Stuats,Name,Price,Description,ImageFile")] Recipe recipe)
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
                        recipe.ImageFile.CopyToAsync(fileStream);
                    }
                    recipe.ImagePath = imageName;
                }
                try
                {
                    var recipeDb = await _context.Recipes.FindAsync(recipe.Id);
                    recipeDb.Stuats = recipe.Stuats;
                    recipeDb.Name = recipe.Name;
                    recipeDb.Date = DateTime.Now;
                    recipeDb.Description = recipe.Description;
                    recipeDb.Price = recipe.Price;
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
                return RedirectToAction(nameof(Recipes));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", recipe.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        private bool RecipeExists(decimal id)
        {
            throw new NotImplementedException();
        }
        public IActionResult Search()
        {
            var result = _context.Recipes.Include(x => x.User).ToList();
            ViewBag.TotalPrice = result.Sum(x => x.Price);
            return View(result);
        }

        [HttpPost]
        public IActionResult Search(DateTime? startDate, DateTime? endDate)
        {
            var result = _context.Recipes.Include(x => x.User).ToList();

            if (startDate == null && endDate == null)
            {
                ViewBag.TotalPrice = result.Sum(x => x.Price);
                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                result = result.Where(x => x.Date >= startDate).ToList();
                ViewBag.TotalPrice = result.Sum(x => x.Price);
                return View(result);
            }
            else if (startDate == null && endDate != null)
            {
                result = result.Where(x => x.Date <= endDate).ToList();
                ViewBag.TotalPrice = result.Sum(x => x.Price);
                return View(result);
            }
            else
            {
                result = result.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
                ViewBag.TotalPrice = result.Sum(x => x.Price);
                return View(result);
            }
        }
        public IActionResult Chart()
        {
            return View();
        }
        public async Task<IActionResult> EditProfile(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(decimal id, [Bind("Id,Username,PhoneNumber,Password,Email,Gender,Address,ImageFile")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);

                if (user.ImageFile != null)
                {
                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + user.ImageFile.FileName;
                    string fullPath = Path.Combine(wwwrootPath + "/Images/", imageName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        user.ImageFile.CopyToAsync(fileStream);
                    }
                    user.ImagePath = imageName;
                }
                try
                {
                    var userDb = await _context.Users.FindAsync(user.Id);
                    userDb.Username = user.Username;
                    userDb.PhoneNumber = user.PhoneNumber;
                    userDb.Email = user.Email;
                    userDb.Password = user.Password;
                    userDb.Date = DateTime.Now;
                    userDb.Gender = user.Gender;
                    userDb.Address = user.Address;
                    _context.Update(userDb);
                    await _context.SaveChangesAsync();
                    _context.Update(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }



    }
}

