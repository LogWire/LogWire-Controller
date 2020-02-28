using System;
using System.ComponentModel.DataAnnotations;

namespace LogWire.Controller.Data.Model
{
    public class UserEntry
    {

        [Key]
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

    }
}
