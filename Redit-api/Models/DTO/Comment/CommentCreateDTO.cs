using System.ComponentModel.DataAnnotations;

namespace Redit_api.Models.DTO
{
    public class CommentCreateDTO
    {
        [Required]
        public int PostId { get; set; }

        public int? ParentId { get; set; } 

        [Required]
        [MaxLength(5000)]
        public string Text { get; set; } = string.Empty;

        public string[]? Embeds { get; set; }
    }
}