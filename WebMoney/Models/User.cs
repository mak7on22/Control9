using Microsoft.AspNetCore.Identity;
using WebMoney.ViewModels;

namespace WebMoney.Models
{
	public class User : IdentityUser<int>
	{
		public int? Balance { get; set; }
		public int? Indificator { get; set; }
		public string? RoleName { get; set; }

		public ICollection<TransferHistory> TransferHistories { get; set; }
    }
}
