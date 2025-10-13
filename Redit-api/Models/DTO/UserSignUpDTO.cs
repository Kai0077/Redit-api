using System.ComponentModel.DataAnnotations;

namespace Redit_api.Models.DTO
{
    public class UserSignupDTO
    {
        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }

        [Required ,Range(13, 120)]
        public int Age { get; set; }

        [Required, MinLength(8)]
        public string Password { get; set; }
    }
}