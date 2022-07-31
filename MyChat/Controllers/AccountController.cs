using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MyChat.Models;
using MyChat.Services;
using MyChat.ViewModels;

namespace MyChat.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHostEnvironment _environment; //Добавляем сервис взаимодействия с файлами в рамках хоста
        private readonly UploadFileService _uploadFileService; // Добавляем сервис для получения файлов из формы
        public ChatContext _db;
        public AccountController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            IHostEnvironment environment,
            UploadFileService uploadFileService,
            ChatContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _environment = environment;
            _uploadFileService = uploadFileService;
            _db = db;
        }
        
        [Authorize]
        public IActionResult Index(string id = null){
            User user = id == null? _userManager.GetUserAsync(User).Result : _userManager.FindByIdAsync(id).Result;
            return View(user);
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
                string path = Path.Combine(_environment.ContentRootPath,"wwwroot\\images\\");
                string photoPath = $"images/{model.File.FileName}";
                _uploadFileService.Upload(path, model.File.FileName, model.File);

                User user = new User
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    BirthDate = model.DateOfBirth,
                    FirstName = model.FirstName,
                    AvatarPath = photoPath
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // EmailService emailService = new EmailService();
                    // await emailService.SendMessageAsync(user.Email,
                    //     "Уведомление",
                    //     $"<b>Привет добро пожаловать на страницу</b> \n<a style='border: solid 2px red;border-radius: 6px;' href='http://localhost:5000/Account/Index'>Ссылка</a>");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Account");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(String.Empty, error.Description);
            }
            return View(model);
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl)&&
                        Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Ok(model);
                }
                ModelState.AddModelError("", "Неправильный логин или пароль");
            }

            return View(model);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        
        [Authorize]
        public IActionResult Edit(string id = null)
        {
            User user = null;
            if (User.IsInRole("admin"))
            {
                user = id == null? _userManager.GetUserAsync(User).Result : _userManager.FindByIdAsync(id).Result;
            }
            else
            {
                user = _userManager.FindByIdAsync(id).Result;
            }
            UserEditViewModel model = new UserEditViewModel()
            {
                FirstName = user.FirstName,
                Email = user.Email,
                UserName = user.UserName,
                BirthDate = user.BirthDate,
                Id = user.Id,
                PhotoPath = user.AvatarPath
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    if (model.File != null)
                    {
                        string path = Path.Combine(_environment.ContentRootPath, "wwwroot\\images\\");
                        string photoPath = $"images/{model.File.FileName}";
                        _uploadFileService.Upload(path, model.File.FileName, model.File);
                        user.AvatarPath = photoPath;
                    }
                    else
                        user.AvatarPath = model.PhotoPath;
                    user.FirstName = model.FirstName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.BirthDate = model.BirthDate;
                    
                    var result = await _userManager.UpdateAsync(user);
                    EmailService emailService = new EmailService();
                    await emailService.SendMessageAsync(user.Email,
                        "Уведомление",
                        $"<b>Привет вы изменили профиль на</b>\n <b>Имя : </b>{user.FirstName}\n <b>Ваш emal : </b>{user.Email} \n <b>Логин : </b>{user.UserName} \n <b>Дата рождение : </b> {user.BirthDate}");
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                Id = user.Id,
                Email =  user.Email
            };
            return View(model);
        }
        
       
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _userManager.FindByIdAsync(model.Id).Result;
                if (user != null)
                {
                    var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
                    var result = await passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        EmailService emailService = new EmailService();
                        await emailService.SendMessageAsync(user.Email,
                            "Уведомление",
                            $"<b style='color : blue;>Привет вы изменили пароль успешно!!!</b>");
                        return RedirectToAction("Index");
                    }
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("NewPassword", error.Description);
                }
                ModelState.AddModelError("", "Пользователь не существует");
                
            }
            
           
            return View(model);
        }

        public async Task<IActionResult> DetailedInformationEmail(string id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            
            EmailService emailService = new EmailService();
            await emailService.SendMessageAsync(user.Email,
                "Уведомление",
                $"<b style='color : blue; '>Подробная информатция Пользователя</b> <b>Имя : </b>{user.UserName}<b>Email :</b> {user.Email} <b>Дата рождение :</b> {user.BirthDate} <b>Фото находится : </b> {user.AvatarPath} <a style='border: solid 2px red;border-radius: 6px;' href='http://localhost:5000/Account/Index'>Ссылка</a>");
            
            return RedirectToAction("Index");
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Message(string userId, string message)
        {
            if (userId != null && message != null)
            {
                ChatMessage messages = new ChatMessage
                {
                    UserId = userId,
                    Message = message
                };
                
                var result = _db.ChatMessages.AddAsync(messages);
                if (result.IsCompletedSuccessfully)
                    await _db.SaveChangesAsync();
            }
            
            string data = "";
            List<ChatMessage> chatMessages = _db.ChatMessages.ToList();
            if (chatMessages.Count > 100)
                    chatMessages.RemoveRange(0, 95);
            foreach (var msg in chatMessages)
            {
                if (msg.User.Id == userId)
                    data +=
                        $"<div style='margin-right: -80%; display: block; margin-top : 5px; margin-bottom:5px;'><img style='width: 50px; height: 50px; margin-right: -120px;' src='/{msg.User.AvatarPath}'>\n<p style='margin-top: -30px; display : block'>{msg.User.FirstName}\n<p style='font-size: 10px; margin-right: -40px; margin-top: -50px;'>{msg.CreatedAt.ToShortTimeString()}<p style='font-size: 19px; margin-top: 30px; display: block;' >{msg.Message}</p></div>";
                else
                    data +=
                        $"<div style='margin-left: -80%; display: block; margin-top : 5px; margin-bottom:5px;'><img style='width: 50px; height: 50px; margin-left: -120px;' src='/{msg.User.AvatarPath}'>\n<p style='margin-top: -30px; display : block'>{msg.User.FirstName}\n <p style='font-size: 10px;margin-left: -40px;margin-top: -50px;'>{msg.CreatedAt.ToShortTimeString()}<p style='font-size: 19px; margin-top: 30px; display: block;' >{msg.Message}</p></div>";
            }
            return Json(data);
        }
    }
}