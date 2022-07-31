using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyChat.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно")]
        [Display(Name = "Электронная почта")]
        [DataType(DataType.EmailAddress)]
        [Remote("CheckEmail", "Validation", ErrorMessage = "Такой Email уже есть")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Это поле обязательно")]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        [Remote("CheckAge", "Validation", ErrorMessage = "Нельзя регестрировать пользователя моложе 18 лет")]
        public DateTime DateOfBirth { get; set; }
        
        [Required(ErrorMessage = "Это поле обязательно")]
        [Display(Name = "Логин")]
        [DataType(DataType.Text)]
        [Remote("CheckUserName", "Validation", ErrorMessage = "Такой логин уже есть")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Это поле обязательно")]
        [Display(Name = "Имя")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Загрузка фото обязательна")]
        [DataType(DataType.Upload)]
        [Display(Name = "Фото профиля")]
        public IFormFile File { get; set; }
        
        [Required(ErrorMessage = "Это поле обязательно")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage = "Минимальная длина 6 символов")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Это поле обязательно")]
        [Display(Name = "Введите пароль повторно")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [MinLength(6,ErrorMessage = "Минимальная длина 6 символов")]
        public string ConfirmPassword { get; set; }
        
    }
}