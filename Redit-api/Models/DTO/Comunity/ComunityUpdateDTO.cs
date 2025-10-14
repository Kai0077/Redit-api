using System.ComponentModel.DataAnnotations;

namespace Redit_api.Models.DTO
{
    public class CommunityUpdateDTO
    {
        [MaxLength(255)]
        public string? Description { get; set; }

        public string? ProfilePicture { get; set; }

        // Optional owner change (only super_user allowed ideally)
        public string? OwnerUsername { get; set; }

        public int[]? PinnedPostIds { get; set; }
    }
}