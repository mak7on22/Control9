using System.ComponentModel.DataAnnotations;

namespace WebMoney.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Номер телефона")]
        public string? Phone { get; set; }
        [Display(Name = "Логин или Email")]
        public string? Login { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
