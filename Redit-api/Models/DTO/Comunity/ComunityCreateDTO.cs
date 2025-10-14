using System.ComponentModel.DataAnnotations;

namespace Redit_api.Models.DTO
{
    public class CommunityCreateDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? ProfilePicture { get; set; }

        public string? OwnerUsername { get; set; }
    }
}