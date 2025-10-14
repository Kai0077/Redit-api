// Models/DTO/PostUpdateDTO.cs
using System.ComponentModel.DataAnnotations;
using Redit_api.Models.Status;

namespace Redit_api.Models.DTO
{
    public class PostUpdateDTO
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Community { get; set; }  

        public string[]? Embeds { get; set; }

        public PostStatus? Status { get; set; }
    }
}