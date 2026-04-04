using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BusinessLayer.Models;
using DataLayer.Contexts;

namespace MVC.Controllers
{
    public class AccountController: Controller
    {
        private readonly IdentityContext _identityContext;
        private readonly SignInManager<User> _signInManager;

        public AccountController(IdentityContext identityContext, SignInManager<User> signInManager)
        {
            _identityContext = identityContext;
            _signInManager = signInManager;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string email, string firstName, string lastName, string personalId, string address, string phonenumber, string role)
        {
            try
            {
                await _identityContext.CreateUserAsync(username, password, email, firstName, lastName, role);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _identityContext.LogInUserAsync(username, password);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}