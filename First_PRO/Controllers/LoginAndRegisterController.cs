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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(x => x.Username == user.Username);
            if (existingUser == null)
            {
                //if (ModelState.IsValid)
                //{
                    var newUser = new User
                    {
                        Username = user.Username,
                        Password = user.Password,
                        RoleId = user.RoleId,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Gender = user.Gender,
                        Address = user.Address,
                        ImagePath = "download.png",
                        Date = DateTime.Now
                    };

                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    return RedirectToAction("Login");
                //}
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
            //if(string.IsNullOrEmpty( user.Username) || string.IsNullOrEmpty(user.Password))
            //{
            //    return View();
            //}
            var userLogin = _context.Users.
                Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault();

            if (userLogin != null)
            {
                HttpContext.Session.SetInt32("UserId", (int)userLogin.Id);
                HttpContext.Session.SetInt32("RoleId", (int)userLogin?.RoleId);
                HttpContext.Session.SetString("Username",userLogin.Username);
                HttpContext.Session.SetString("Email", userLogin.Email);

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
