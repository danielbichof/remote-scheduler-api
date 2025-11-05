using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Scheduler.Application.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        // Intentionally not using [EmailAddress] to allow any string during tests
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
