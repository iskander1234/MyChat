using System;

namespace MyChat.Models
{
    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Message { get; set; }
        
        public string UserId { get; set; }
        public virtual  User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}