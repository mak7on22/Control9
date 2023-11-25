using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using WebMoney.Models;
using WebMoney.ViewModels;

namespace WebMoney.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly MoneyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly Random _random = new Random();

        public UserController(MoneyContext context, UserManager<User> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }
        public async Task<IActionResult> Profile(int? id)
        {
            var currentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (currentId == currentId)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["I"] = "DushnilaDima";
                return View(user);
            }
            else
            {
                User anotherUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(currentUserId, out int currentUserIdInt);
                return View(anotherUser);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Replenish(int? id)
        {
            if (id == null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();
            var replenishModel = new Replenish
            {
                Id = id.Value, // Установите Id из параметра метода
                User = user
            };

            return View(replenishModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Replenish(Replenish model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _userManager.FindByIdAsync(userId.ToString());

                if (currentUser == null)
                {
                    ViewData["StatusErrorLog"] = "Пользователь не найден";
                    return View(model);
                }
                if (!string.IsNullOrEmpty(model.IndificatorOrPhone))
                {
                    User targetUser = null;
                    if (!string.IsNullOrEmpty(model.IndificatorOrPhone))
                    {
                        targetUser = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.IndificatorOrPhone);
                        if (targetUser == null)
                            targetUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Indificator == int.Parse(model.IndificatorOrPhone));
                    }
                    else
                        ViewData["StatusErrorLog"] = "Кошелек не найден";

                    if (targetUser == null)
                    {
                        ViewData["StatusErrorLog"] = "Кошелек не найден";
                        return View(model);
                    }
                    var replenishId = (int)GenerateRandom12DigitNumber();
                    var transaction = new TransferHistory
                    {
                        Id = replenishId,
                        User = currentUser,
                        Replenish = model,
                        TransactionDate = DateTime.Now,
                        Type = TransactionType.Replenish,
                        Amount = model.Amount ?? 0,
                        ReceiverId = targetUser.Id,
                    };

                    currentUser.Balance -= model.Amount ?? 0;
                    targetUser.Balance += model.Amount ?? 0;

                    await _userManager.UpdateAsync(currentUser);
                    await _userManager.UpdateAsync(targetUser);

                    _context.TransferHistories.Add(transaction);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Profile", "User");
                }
                else
                {
                    ViewData["StatusErrorLog"] = "Кошелек не найден";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewData["StatusErrorLog"] = "Произошла ошибка при выполнении транзакции";
                return View(model);
            }
        }
        private long GenerateRandom12DigitNumber()
        {
            var random = new Random();
            return random.Next(100000, 1000000);
        }

        public IActionResult TransactionHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users
                .Include(u => u.TransferHistories)
                .FirstOrDefault(u => u.Id == int.Parse(userId));

            if (user == null)
                return NotFound();
            var transactionHistory = user.TransferHistories
                .Where(th => th.UserId == user.Id || th.ReceiverId == user.Id)
                .OrderByDescending(th => th.TransactionDate)
                .ToList();

            return View(transactionHistory);
        }
    }
}
