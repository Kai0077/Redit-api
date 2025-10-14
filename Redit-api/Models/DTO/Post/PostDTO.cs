// Models/DTO/PostDTO.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Redit_api.Models.Status;

namespace Redit_api.Models
{
    [Table("post")]
    public class PostDTO
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("aura")]
        public int Aura { get; set; } = 0;

        [Required, MaxLength(50)]
        [Column("original_poster")]
        public string OriginalPoster { get; set; }

        // Optional FK to community.name
        [MaxLength(100)]
        [Column("community")]
        public string? Community { get; set; }

        [Column("embeds")]
        public string[] Embeds { get; set; } = Array.Empty<string>();

        [Column("status")]
        public PostStatus Status { get; set; } = PostStatus.Active;
    }
}