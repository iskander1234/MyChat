using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MyChat.Models
{
    public class User : IdentityUser
    {
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string FirstName { get; set; }
        public string AvatarPath { get; set; }

        public virtual List<ChatMessage> ChatMessages { get; set; }
    }
}