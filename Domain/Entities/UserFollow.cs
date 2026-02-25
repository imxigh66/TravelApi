using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserFollow
    {
        public int FollowerId { get; set; }  // кто подписывается
        public int FollowingId { get; set; }  // на кого подписываются
        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;

        // NAV
        public User Follower { get; set; } = null!;
        public User Following { get; set; } = null!;
    }
}
