namespace Redit_api.Models
{
    public class Post
    {
        public string Title;
        public string Description;
        public int Aura;
        public User OriginalPoster { get; set; }
        public string[] Embeds { get; set; }
        public Status PostStatus { get; set; }
        public Comment[] Comments { get; set; }
    }
}