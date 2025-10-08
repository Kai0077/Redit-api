using Redit_api.Models.Status;

namespace Redit_api.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title;
        public string Description;
        public int Aura;
        public User OriginalPoster { get; set; }
        public string[] Embeds { get; set; }
        public PostStatus PostStatus { get; set; }
        public Comment[] Comments { get; set; }
    }
}