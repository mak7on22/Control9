using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebMoney.Models;

namespace WebMoney.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly MoneyContext _context;
        private readonly UserManager<User> _userManager;

        public CompaniesController(MoneyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
              return _context.Companys != null ? 
                         View(await _context.Companys.ToListAsync()) :
                          Problem("Entity set 'ElectronicWalletContext.Companys'  is null.");
        }
       
    }
}
