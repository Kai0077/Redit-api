using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redit_api.Models.DTO
{
    [Table("community")]
    public class CommunityDTO
    {
        [Key]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("profile_picture")]
        public string? ProfilePicture { get; set; }

        [Column("owner_username")]
        public string? OwnerUsername { get; set; }

        [Column("pinned_post_ids")]
        public int[] PinnedPostIds { get; set; } = Array.Empty<int>();

        // Ignore these for now (they represent relationships you’ll define later)
        [NotMapped] public Post[]? Posts { get; set; }
        [NotMapped] public User[]? Members { get; set; }
        [NotMapped] public Post[]? Pinned { get; set; }
        [NotMapped] public User? Owner { get; set; }
        [NotMapped] public User[]? Admins { get; set; }
        [NotMapped] public User[]? Moderators { get; set; }
    }
}