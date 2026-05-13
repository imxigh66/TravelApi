using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum NotificationType
    {
        NewFollower = 1,
        PostLiked = 2,
        PostCommented = 3,
        TripInvite = 4,
        PlaceApproved = 5,
        PlaceRejected = 6,
    }
}
