using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookSwap.Models;
using System.Threading.Tasks;
using System.Linq;

namespace BookSwap.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login() => View();

      
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            var user = await _userManager.FindByNameAsync(username);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin")) return RedirectToAction("AdminDashboard", "Admin");
            if (roles.Contains("Buyer")) return RedirectToAction("Index", "Buyer");
            if (roles.Contains("Seller")) return RedirectToAction("Index", "Seller");

            TempData["UsernameForRole"] = username;
            return RedirectToAction("SelectRole");
        }

        [HttpGet]
        public IActionResult Register() => View();

      
        // [HttpPost]
        // public async Task<IActionResult> Register(string username, string password, string confirmPassword)
        // {
        //     if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        //     {
        //         ViewBag.Error = "Username and password are required.";
        //         return View();
        //     }

        //     if (password != confirmPassword)
        //     {
        //         ViewBag.Error = "Passwords do not match.";
        //         return View();
        //     }

        //     var existingUser = await _userManager.FindByNameAsync(username);
        //     if (existingUser != null)
        //     {
        //         ViewBag.Error = "Username already exists.";
        //         return View();
        //     }

        //     var newUser = new ApplicationUser { UserName = username };
        //     var result = await _userManager.CreateAsync(newUser, password);

        //     if (!result.Succeeded)
        //     {
        //         ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
        //         return View();
        //     }

        //     // Sign in
        //     await _signInManager.SignInAsync(newUser, isPersistent: false);

        //     // Redirect to role selection
        //     TempData["UsernameForRole"] = username;
        //     return RedirectToAction("SelectRole");
        // }

       [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists.";
                return View();
            }

            var newUser = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
                return View();
            }

         
            TempData["UsernameForRole"] = username;


            return RedirectToAction("SelectRole");
        }

        [HttpGet]
        public IActionResult SelectRole()
        {
            var username = TempData["UsernameForRole"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Error = "Session expired. Please login again.";
                return RedirectToAction("Login");
            }

            ViewBag.Username = username;
            ViewBag.Roles = new[] { "Buyer", "Seller" };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SelectRole(string username, string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                ViewBag.Error = "Please select a role.";
                ViewBag.Username = username;
                ViewBag.Roles = new[] { "Buyer", "Seller" };
                return View();
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return RedirectToAction("Login");

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            if (role == "Buyer") return RedirectToAction("Index", "Buyer");
            if (role == "Seller") return RedirectToAction("Index", "Seller");

            return RedirectToAction("Login");
        }
        public IActionResult About_Us()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        // // ---------------- LOGOUT ----------------
        // public async Task<IActionResult> Logout()
        // {
        //     await _signInManager.SignOutAsync();
        //     return RedirectToAction("Index", "Home");
        // }
    }
}
