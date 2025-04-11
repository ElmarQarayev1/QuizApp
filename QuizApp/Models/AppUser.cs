using System;
using Microsoft.AspNetCore.Identity;

namespace QuizApp.Models
{
	public class AppUser : IdentityUser
    {
        public string FullName { get; set; }

    }
}

