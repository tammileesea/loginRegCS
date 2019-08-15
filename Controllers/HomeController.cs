using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using loginReg.Models;

namespace loginReg.Controllers
{
    public class HomeController : Controller
    {
        private UserContext dbContext;
        public HomeController(UserContext context){
            dbContext = context; 
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet("loginPage")]
        public IActionResult LoginPage() {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser) {
            if(ModelState.IsValid){
                if(dbContext.users.Any(u => u.Email == newUser.Email)){
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Index");
                } else {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    if(HttpContext.Session.GetInt32("UserId") == null){
                        HttpContext.Session.SetInt32("UserId", newUser.UserId);
                    }
                    return RedirectToAction("Success", newUser);
                }
            } else {
                System.Console.WriteLine("*******************");
                System.Console.WriteLine("REGISTRATION NOT WORKING!!!!");
                System.Console.WriteLine("*******************");
                return View("Index");
            }
        }

        public IActionResult Login(LoginUser userLogin){
            if(ModelState.IsValid){
                var userInDB = dbContext.users.FirstOrDefault(u => u.Email == userLogin.Email);
                if(userInDB == null){
                    ModelState.AddModelError("Email", "Invalid email or password");
                    return View("LoginPage");
                } else {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(userLogin, userInDB.Password, userLogin.Password);
                    if(result == 0){
                        ModelState.AddModelError("Password", "Invalid email or password");
                        return RedirectToAction("LoginPage");
                    }
                    if(HttpContext.Session.GetInt32("UserId") == null){
                        HttpContext.Session.SetInt32("UserId", userInDB.UserId);
                    }
                    return RedirectToAction("Success");
                }
            } else {
                return View("LoginPage");
            }
        }

        [HttpGet("success")]
        public IActionResult Success() {
            if(HttpContext.Session.GetInt32("UserId") == null){
                return RedirectToAction("LoginPage");
            }
            return View();
        }

        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
