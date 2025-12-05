using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;
using Redit_api.Models.Status;
using Redit_api.Util;

namespace Redit_api.Models
{
    [Table("post")]
    [FirestoreData]
    public class PostDTO
    {
        [Key]
        [Column("id")]
        [FirestoreProperty("id")]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Column("title")]
        [FirestoreProperty("title")]
        public string Title { get; set; }

        [Column("description")]
        [FirestoreProperty("description")]
        public string? Description { get; set; }

        [Column("aura")]
        [FirestoreProperty("aura")]
        public int Aura { get; set; } = 0;

        [Required, MaxLength(50)]
        [Column("original_poster")]
        [FirestoreProperty("original_poster")]
        public string OriginalPoster { get; set; }

        [MaxLength(100)]
        [Column("community")]
        [FirestoreProperty("community")]
        public string? Community { get; set; }

        [Column("embeds")]
        [FirestoreProperty("embeds")]
        public string[] Embeds { get; set; } = Array.Empty<string>();

        [Column("status")]
        [FirestoreProperty("status", ConverterType = typeof(PostStatusConverter))]
        public PostStatus Status { get; set; } = PostStatus.Active;

        [Column("is_public")] 
        [FirestoreProperty("is_public")]
        public bool IsPublic { get; set; } = true;
        
        [Column("publish_at")]
        [FirestoreProperty("publish_at")]
        public DateTime? PublishAt { get; set; }
    }
}