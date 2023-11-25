using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using WebMoney.Models;
using WebMoney.ViewModels;

namespace WebMoney.Controllers
{
    [ResponseCache(CacheProfileName = "Cashing")]
    public class AccountController : Controller
    {
        private readonly MoneyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, MoneyContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUserLogin = await _userManager.FindByNameAsync(model.Login);
                var existingUserPhone = await _userManager.FindByEmailAsync(model.Phone);

                if (existingUserLogin != null)
                    ViewData["StatusError"] = "Пользователь с таким Логином уже существует.";
                if (existingUserPhone != null)
                    ViewData["StatusErrors"] = "Пользователь с таким Номером уже существует.";
                if (existingUserLogin != null || existingUserPhone != null)
                    return View(model);
                int balance = int.Parse(GenerateRandomBalance());
                int indificator = GenerateRandomIndificator();
                User user = new()
                {
                    UserName = model.Login,
                    PhoneNumber = model.Phone,
                    Balance = balance,
                    Indificator = indificator,
                    RoleName = "Member"
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("Profile", "User");
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        private string GenerateRandomBalance()
        {
            var password = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < 9; i++)
            {
                string characters = "123456789";
                char randomChar = characters[random.Next(characters.Length)];
                password.Append(randomChar);
            }
            return password.ToString();
        }
        private int GenerateRandomIndificator()
        {
           Random random = new();
            return random.Next(100000, 1000000);
             
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null!)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

                User user = null!;
                if (!string.IsNullOrEmpty(model.Phone))
                {
                        user = await _userManager.FindByEmailAsync(model.Phone);
                }
                else if (!string.IsNullOrEmpty(model.Login))
                    user = await _userManager.FindByNameAsync(model.Login);
                if (user != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                        user,
                        model.Password,
                        model.RememberMe,
                        false
                    );
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            return Redirect(model.ReturnUrl);
                        else
                            return RedirectToAction("Profile", "User");
                    }
                }
                ViewData["StatusErrorLog"] = "Неправильный логин или пароль";
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult ChangeCulture(string culture)
        {
            CultureInfo preferredCulture = CultureInfo.CurrentCulture;
            var cultureInfo = new CultureInfo(culture);
            var requestCulture = new RequestCulture(cultureInfo);
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
            Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            cookieValue,
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            var preferredCultureCookie = HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            if (!string.IsNullOrEmpty(preferredCultureCookie))
            {
                var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
                preferredCulture = requestCultureFeature?.RequestCulture?.Culture ?? CultureInfo.CurrentCulture;
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
        [HttpGet]
        public IActionResult Replenish()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Replenish(ReplenishViewModel model)
        {
            User user = null;

            if (!string.IsNullOrEmpty(model.IndificatorOrPhoneAn))
            {
                if (user == null)
                    user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.IndificatorOrPhoneAn);
                if (user == null)
                    user = await _userManager.Users.SingleOrDefaultAsync(u => u.Indificator == int.Parse(model.IndificatorOrPhoneAn)); 
            }
            else
                ViewData["StatusErrorLog"] = "Кошелек не найден";
            if (user != null)
            {
                model.TimeAn = DateTime.Now;
                user.Balance += model.AmountAn;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Replenish", "Account");
            }
            else
                ViewData["StatusErrorLog"] = "Пользователь не найден";
            return View(model);
        }




    }
}
