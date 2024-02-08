﻿using Microsoft.AspNetCore.Identity;

namespace UdemyCore.Models
{
    public class UserApp : IdentityUser<string>
    {
        public string City { get; set; }
    }        
}
