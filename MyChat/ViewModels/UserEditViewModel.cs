using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyChat.ViewModels
{
    public class UserEditViewModel
    {
        public DateTime? BirthDate { get; set; }
        public string FirstName { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
        public string Id { get; set; }
    }
}