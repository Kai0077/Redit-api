using System.ComponentModel.DataAnnotations;

namespace Redit_api.Models.DTO
{
    public class UserLoginDTO
    {
        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; }

        [Required, MinLength(8)]
        public string Password { get; set; }
    }
}