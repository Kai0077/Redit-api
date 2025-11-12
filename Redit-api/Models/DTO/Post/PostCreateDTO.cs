using System.ComponentModel.DataAnnotations;
using Redit_api.Models.Status;

namespace Redit_api.Models.DTO
{
    public class PostCreateDTO
    {
        [Required, MaxLength(200)] 
        public string Title { get; set; } = default!;

        [Required]
        public string? DescriptionHtml { get; set; } = default!;

        [MaxLength(100)]
        public string? Community { get; set; }

        public string[]? Embeds { get; set; }

        public PostStatus? Status { get; set; }
        public DateTime? PublishAt { get; set; }
    }
}