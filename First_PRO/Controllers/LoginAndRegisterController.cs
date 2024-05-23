using First_PRO.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace First_Project_MVC.Controllers
{
    public class LoginAndRegisterController : Controller
    {
            private readonly ModelContext _context;
            public LoginAndRegisterController(ModelContext context)
            {
                _context = context;
            }
        public IActionResult Register(User user, string userName, string password, Role role, string PhoneNumber, string Email, decimal Gender, string Address)
        {
            var existingUser = _context.Users.FirstOrDefault(x => x.Username == userName);
            if (existingUser == null)
            {
                if (ModelState.IsValid)
                {
                    var defaultProfilePicturePath = "~/Home/img/download.png";  

                    var newUser = new User
                    {
                        Username = userName,
                        Password = password,
                        RoleId = role.Id,
                        PhoneNumber = PhoneNumber,
                        Email = Email,
                        Gender = Gender,
                        Address = Address,
                        ImagePath = defaultProfilePicturePath 
                    };

                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    return RedirectToAction("Login");
                }
            }
            else
            {
                ModelState.AddModelError("", "Username already exists.");
            }

            return View(user);
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var userLogin = await _context.Users.
                Where(x => x.Username == user.Username && x.Password == user.Password).SingleOrDefaultAsync();

            if (userLogin != null)
            {
                HttpContext.Session.SetInt32("UserId", (int)userLogin.Id);
                HttpContext.Session.SetInt32("RoleId", (int)userLogin.RoleId);
                HttpContext.Session.SetString("Username",userLogin.Username);
                ViewData["Message"] = "Hello from controller";

                ViewBag.UserImage = userLogin.ImagePath;
                switch (userLogin.RoleId)
                {
                    case 1:
                        return Redirect("https://localhost:7237/Admin/Index");
                    case 2:
                        return Redirect("https://localhost:7237/Chef/Index");
                    case 3:
                        return RedirectToAction("User", "Home");

                }
            }
            ModelState.AddModelError("", "UserName or Password are incorret");
            return View();
        }
    }
}
