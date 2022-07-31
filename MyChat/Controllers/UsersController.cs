using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyChat.Models;

namespace MyChat.Controllers
{
    
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;
        public ChatContext _db;
        
        public UsersController(UserManager<User> userManager, ChatContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        // [Authorize(Roles = "admin")]
        // GET
        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }
        
        [Authorize(Roles = "admin")]
        public IActionResult SearchAjax()
        {
            return View();
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult SearchAjaxResult(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord)) return PartialView("UsersPartial", _db.Users.ToList());
            var filterWord = keyWord.ToUpper();
            var users = _db.Users.Where(u =>
                    u.UserName.ToUpper().Contains(filterWord) ||
                    u.Email.ToUpper().Contains(filterWord) ||
                    u.FirstName.ToUpper().Contains(filterWord))
                .ToList();
            if (users.Count > 0)
                return PartialView("UsersPartial", users);
            return Json(false);
        }
    }
}