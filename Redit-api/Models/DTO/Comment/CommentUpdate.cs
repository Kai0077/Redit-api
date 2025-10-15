using System.ComponentModel.DataAnnotations;

namespace Redit_api.Models.DTO
{
    public class CommentUpdateDTO
    {
        [MaxLength(5000)]
        public string? Text { get; set; }

        public string[]? Embeds { get; set; }
    }
}