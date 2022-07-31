using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyChat.Models
{
    public class ChatContext : IdentityDbContext<User>
    {
        public override DbSet<User> Users { get; set; }
        
        public  DbSet<ChatMessage> ChatMessages { get; set; }
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }
    }
}