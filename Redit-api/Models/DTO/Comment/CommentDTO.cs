using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redit_api.Models.DTO
{
    [Table("comments")]
    public class CommentDTO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("text")]
        public string Text { get; set; } = string.Empty;

        [Column("embeds")]
        public string[] Embeds { get; set; } = Array.Empty<string>();

        [Required]
        [MaxLength(50)]
        [Column("commenter")]
        public string Commenter { get; set; } = string.Empty; // FK -> user.username

        [Column("aura")]
        public int Aura { get; set; } = 0;

        [Required]
        [Column("post_id")]
        public int PostId { get; set; } // FK -> post.id

        [Column("parent_id")]
        public int? ParentId { get; set; } // FK -> comments.id (nullable for top-level)
    }
}