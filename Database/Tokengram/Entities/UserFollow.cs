namespace Tokengram.Database.Tokengram.Entities
{
    public class UserFollow : BaseEntity
    {
        public string FollowerAddress { get; set; } = null!;
        public string FollowedUserAddress { get; set; } = null!;
        public User Follower { get; set; } = null!;
        public User FollowedUser { get; set; } = null!;
    }
}
