using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Redit_api.Models.Status;

namespace Redit_api.Models
{
    [Table("user")]
    public class UserDTO
    {
        [Key]
        [Column("username")]
        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Column("name")]
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Column("email")]
        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Column("age")]
        [Range(13, 120)]
        public int Age { get; set; }

        [Column("password_hash")]
        [Required]
        public string PasswordHash { get; set; }

        [Column("aura")]
        public int Aura { get; set; } = 0;

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("profile_picture")]
        public string? ProfilePicture { get; set; }

        [Column("account_status")]
        public UserStatus AccountStatus { get; set; } = UserStatus.Offline;
        
        [Column("role")]
        public UserRole Role { get; set; } = UserRole.User;

    }
}