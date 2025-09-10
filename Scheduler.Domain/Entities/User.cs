using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class User
    {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }

            public int RoleId { get; set; }
            public virtual Role Role { get; set; }

            public int GroupId { get; set; }
            public virtual Group Group { get; set; }
            public int? ManagerId { get; set; } 
            public virtual User Manager { get; set; }

            public virtual ICollection<User> Subordinates { get; set; } = new List<User>();
    }
}
