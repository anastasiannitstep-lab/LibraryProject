using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryProject.Models
{
    public class User
    {
        public int userid { get; set; }
        public string username { get; set; } 
        public string password_hash { get; set; }
    }
}
