using System.ComponentModel.DataAnnotations;

namespace Scheduler.Application.DTOs
{
    public class AdminCreateUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int GroupId { get; set; }

        public int? ManagerId { get; set; }
    }
}


