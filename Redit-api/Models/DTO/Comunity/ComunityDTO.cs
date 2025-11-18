using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;

namespace Redit_api.Models.DTO
{
    [Table("community")]
    [FirestoreData]
    public class CommunityDTO
    {
        [Key]
        [Column("name")]
        [FirestoreProperty("name")]
        public string Name { get; set; }

        [Column("description")]
        [FirestoreProperty("description")]
        public string? Description { get; set; }

        [Column("profile_picture")]
        [FirestoreProperty("profile_picture")]
        public string? ProfilePicture { get; set; }

        [Column("owner_username")]
        [FirestoreProperty("owner_username")]
        public string? OwnerUsername { get; set; }

        [Column("pinned_post_ids")]
        [FirestoreProperty("pinned_post_ids")]
        public int[]? PinnedPostIds { get; set; }
    }
}