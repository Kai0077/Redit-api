namespace Redit_api.Models
{
    public class Community
    {
        public Post[] Posts { get; set; }
        public User[] Members { get; set; }
        public string ProfilePicture { get; set; }
        public Post[] Pinned { get; set; }
        public User Owner { get; set; }
        public User[] Admins { get; set; }
        public User[] Moderators { get; set; }
    }
}