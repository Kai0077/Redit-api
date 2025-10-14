using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redit_api.Models.DTO
{
    [Table("community")]
    public class CommunityDTO
    {
        [Key]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("profile_picture")]
        public string? ProfilePicture { get; set; }

        [Column("owner_username")]
        public string? OwnerUsername { get; set; }

        [Column("pinned_post_ids")]
        public int[]? PinnedPostIds { get; set; }
    }
}