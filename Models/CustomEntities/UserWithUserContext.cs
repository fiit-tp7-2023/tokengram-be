using Tokengram.Database.Tokengram.Entities;

namespace Tokengram.Models.CustomEntities
{
    public class UserWithUserContext
    {
        public User User { get; set; } = null!;
        public bool IsFollower { get; set; } = false;
        public bool IsFollowed { get; set; } = false;
        public long PostCount { get; set; } = 0;
        public long FollowerCount { get; set; } = 0;
        public long FollowingCount { get; set; } = 0;
    }
}
