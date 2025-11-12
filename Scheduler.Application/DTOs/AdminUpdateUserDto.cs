using System.ComponentModel.DataAnnotations;

namespace Scheduler.Application.DTOs
{
    public class AdminUpdateUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int GroupId { get; set; }

        public int? ManagerId { get; set; }
    }
}

