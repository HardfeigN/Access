﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        [NotMapped]
        public string FullAddress { get; set; }
    }
}
