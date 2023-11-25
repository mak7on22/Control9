using System.ComponentModel.DataAnnotations;

namespace WebMoney.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string? Login { get; set; }
        [Required]
        [Display(Name = "Номер телефона")]
        public string? Phone { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        public string PasswordConfirm { get; set; }
    }
}
