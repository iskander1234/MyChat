using System.ComponentModel.DataAnnotations;

namespace MyChat.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Обязательно укажите email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Это поле обязательно для заполнения")]
        public string NewPassword { get; set; }
    }
}