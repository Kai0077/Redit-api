namespace Redit_api.Models
{
    public class Comment
    {
        public string Text { get; set; }
        public string[] Embeds { get; set; }
        public User Commenter { get; set; }
        public int Aura { get; set; }
        public Comment[] Replies { get; set; }
        public Post PostId { get; set; }
    }
}