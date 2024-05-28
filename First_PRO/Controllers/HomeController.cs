using First_PRO.Models;
using First_PRO.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using System.Diagnostics;


namespace First_PRO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public HomeController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            int customerCount = _context.Users.Count(u => u.RoleId == 3);
            int chefCount = _context.Users.Count(u => u.RoleId == 2);
            var chefs = _context.Users.Where(u => u.RoleId == 2).ToList();
            var testimonials = _context.Testimonials.Include(t => t.User).ToList();

            ViewBag.CustomerCount = customerCount;
            ViewBag.ChefCount = chefCount;
            ViewBag.Chefs = chefs;
            ViewBag.Testimonials = testimonials;

            return View();
        }
        public IActionResult User(decimal categoryId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var categories = _context.Categories.Include(c => c.Recipes).ToList();
            int customerCount = _context.Users.Count(u => u.RoleId == 3);
            int chefCount = _context.Users.Count(u => u.RoleId == 2);
            var chefs = _context.Users.Where(u => u.RoleId == 2).ToList();
            var recipes = _context.Recipes.ToList();
            var loginUser = _context.Users.Where(u => u.Id == (decimal)userId).FirstOrDefault();
            var testimonials = _context.Testimonials.Include(t => t.User).ToList();
            ViewBag.userIdValue = HttpContext.Session.GetInt32("UserId");





            foreach (var cat in categories)
            {
                foreach (var Recipe in cat.Recipes)
                {
                    var rateList = _context.Testimonials.Where(x => x.RecipeId == Recipe.Id).Select(item => new
                    {
                        sumRate = item.Rate
                    }).ToList();

                    var sumOfRate = rateList.Sum(x => x.sumRate);
                    var countOfRate = rateList.Count;

                    if (sumOfRate != 0 && countOfRate != 0)
                    {
                        Recipe.AverageRating = (double)(sumOfRate / countOfRate);
                    }
                }
            }
            ViewBag.UserId = userId;

            ViewBag.Testimonials = testimonials;


            ViewBag.Categories = categories;
            ViewBag.CustomerCount = customerCount;
            ViewBag.ChefCount = chefCount;
            ViewBag.Chefs = chefs;
            ViewBag.Recipes = recipes;

            return View(loginUser);
        }
        public IActionResult Recipes(decimal chefId)
        {
            var chef = _context.Users.FirstOrDefault(u => u.RoleId == 2 && u.Id == chefId);
            var recipes = _context.Recipes.Include(r => r.Cat).Where(r => r.UserId == chefId).ToList();


            ViewBag.ChefName = chef != null ? chef.Username : "Chef";

            ViewBag.Recipes = recipes;

            foreach (var recipe in recipes)
            {
                var averageRating = _context.Testimonials
                    .Where(t => t.RecipeId == recipe.Id)
                    .Average(t => (double?)t.Rate) ?? 0; 
                recipe.AverageRating = averageRating;
            }

            return View(recipes);
        }


        public IActionResult About()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            int customerCount = _context.Users.Count(u => u.RoleId == 3);
            int chefCount = _context.Users.Count(u => u.RoleId == 2);
            var chefs = _context.Users.Where(u => u.RoleId == 2).ToList();
            var loginUser = _context.Users.Where(u => u.Id == (decimal)userId).FirstOrDefault();


            ViewBag.CustomerCount = customerCount;
            ViewBag.ChefCount = chefCount;
            ViewBag.Chefs = chefs;
            return View(loginUser);
        }
        public IActionResult GuestAbout()
        {
            int customerCount = _context.Users.Count(u => u.RoleId == 3);
            int chefCount = _context.Users.Count(u => u.RoleId == 2);
            var chefs = _context.Users.Where(u => u.RoleId == 2).ToList();

            ViewBag.CustomerCount = customerCount;
            ViewBag.ChefCount = chefCount;
            ViewBag.Chefs = chefs;
            return View();
        }
        public IActionResult Service()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var loginUser = _context.Users.Where(u => u.Id == (decimal)userId).FirstOrDefault();


            return View(loginUser);
        }
        public IActionResult Menu()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var loginUser = _context.Users.Where(u => u.Id == (decimal)userId).FirstOrDefault();


            return View(loginUser);
        }

        public IActionResult Team()
        {
            var chefs = _context.Users.Where(u => u.RoleId == 2).ToList();
            ViewBag.Chefs = chefs;

            var userId = HttpContext.Session.GetInt32("UserId");
            var loginUser = _context.Users.Where(u => u.Id == (decimal)userId).FirstOrDefault();


            return View(loginUser);
        }
        public IActionResult Contact()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var loginUser = _context.Users.Where(u => u.Id == (decimal)userId).FirstOrDefault();


            return View(loginUser);
        }
        public IActionResult GuestContact()
        {
            return View();
        }

        public async Task<IActionResult> Testimonial()
        {


            var userId = HttpContext.Session.GetInt32("UserId");
            var testimonials = await _context.Testimonials
                .Where(r => r.UserId == userId)
                .Include(t => t.Recipe)
                .Include(t => t.User)
                .ToListAsync();

            ViewBag.Testimonials = testimonials;

            var loginUser = _context.Users.Where(u => u.Id == (decimal)userId).FirstOrDefault();


            return View(loginUser);
        }



        public IActionResult TestimonialCreate(decimal? recipeId)
        {
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["recipeIdValue"] = recipeId;
            var userId = HttpContext.Session.GetInt32("UserId");

            var rate = _context.Testimonials.FirstOrDefault(x => x.RecipeId == recipeId && x.UserId == userId);
            return View(rate);
        }

        // POST: Testimonials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestimonialCreate([Bind("Id,Rate,Description,RecipeId")] Testimonial testimonial)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            try
            {
                var existingTestimonial = await _context.Testimonials.FindAsync(testimonial.Id);

                if (existingTestimonial != null)
                {
                    existingTestimonial.Rate = testimonial.Rate;
                    existingTestimonial.Description = testimonial.Description;
                    existingTestimonial.Date = DateTime.Now;
                    _context.Update(existingTestimonial);
                }
                else
                {
                    testimonial.UserId = userId.Value;
                    testimonial.Date = DateTime.Now;
                    _context.Add(testimonial);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(User));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while adding the testimonial.");
                return View(testimonial);
            }
        }
        public async Task<IActionResult> Requests(decimal? recipeId)
        {
            ViewData["recipeIdValue"] = recipeId;

            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Requests([Bind("Id,RecipeId,Stuats,ByCard")] Request request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                var recipe = _context.Recipes.FirstOrDefault(r => r.Id == request.RecipeId);
                request.Date = DateTime.Now;
                request.UserId = userId;
                _context.Add(request);
                await _context.SaveChangesAsync();

                var toEmail = HttpContext.Session.GetString("Email");
                EmailService.SendRecipeEmail(toEmail, recipe?.Name, recipe?.Description);

                return RedirectToAction(nameof(User));
            }
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "Id", "Id", request.RecipeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", request.UserId);
            return View(request);
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
        public async Task<IActionResult> EditProfile(decimal id, [Bind("Id,PhoneNumber,Password,Email,Address,ImageFile")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                    userDb.PhoneNumber = user.PhoneNumber;
                    userDb.Email = user.Email;
                    userDb.Password = user.Password;
                    userDb.Date = DateTime.Now;
                    userDb.Address = user.Address;
                    userDb.ImagePath = user.ImagePath;
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
                return RedirectToAction(nameof(User));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);

        }

        private bool UserExists(decimal id)
        {
            throw new NotImplementedException();
        }


        // GET: Guests/Create
        public IActionResult CreateGuest()
        {
            return View();
        }

        // POST: Guests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGuest([Bind("Id,Name,Date,PhoneNumber,Email,Question")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }
    }
}