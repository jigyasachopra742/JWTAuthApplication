using System;
using System.Collections.Generic;

namespace JWTAuthApplication.Models
{
    public partial class Usertable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
