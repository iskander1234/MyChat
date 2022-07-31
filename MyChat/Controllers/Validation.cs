using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using MyChat.Models;

namespace MyChat.Controllers
{
    public class Validation : Controller
    {
        private ChatContext _db;
        private readonly UserManager<User> _userManager;

        public Validation(ChatContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public bool CheckAge(DateTime DateOfBirth)
        {
            DateTime now = DateTime.Now.AddYears(-18);
            bool data = DateOfBirth < now;
            return data;
        }
        
        [HttpGet]
        public bool CheckEmail(string Email)
        {
            return !_db.Users.Any(u => u.Email == Email);
        }
        
        [HttpGet]
        public bool CheckUserName(string UserName)
        {
            return !_db.Users.Any(u => u.UserName == UserName);
        }
        
    }
}