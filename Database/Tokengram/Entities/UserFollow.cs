namespace Tokengram.Database.Tokengram.Entities
{
    public class UserFollow : BaseEntity
    {
        public string UserAddress { get; set; } = null!;
        public string FollowedUserAddress { get; set; } = null!;
        public User User { get; set; } = null!;
        public User FollowedUser { get; set; } = null!;
    }
}
