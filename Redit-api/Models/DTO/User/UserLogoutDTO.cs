using System.ComponentModel.DataAnnotations;

namespace Redit_api.Models.DTO
{
    public class LogoutDTO
    {
        [Required]
        public string Username { get; set; }
    }
}