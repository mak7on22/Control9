using WebMoney.Models;

namespace WebMoney.ViewModels
{
    public class ReplenishViewModel
    {
        public string IndificatorOrPhoneAn { get; set; }
        public int AmountAn { get; set; }
        public DateTime TimeAn { get; set; }
        public User UserAn { get; set; }
        public Replenish ReplenishAn { get; set; }
    }
}
