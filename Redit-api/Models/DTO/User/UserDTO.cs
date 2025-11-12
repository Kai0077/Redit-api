using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;
using Redit_api.Models.Status;

namespace Redit_api.Models
{
    [Table("user")]
    [FirestoreData]
    public class UserDTO
    {
        [Key]
        [Column("username")]
        [Required, MaxLength(50)]
        [FirestoreProperty("username")]
        public string Username { get; set; }

        [Column("name")]
        [Required, MaxLength(100)]
        [FirestoreProperty("name")]
        public string Name { get; set; }

        [Column("email")]
        [Required, MaxLength(150)]
        [FirestoreProperty("email")]
        public string Email { get; set; }

        [Column("age")]
        [Range(13, 120)]
        [FirestoreProperty("age")]
        public int Age { get; set; }

        [Column("password_hash")]
        [Required]
        [FirestoreProperty("password_hash")]
        public string PasswordHash { get; set; }

        [Column("aura")]
        [FirestoreProperty("aura")]
        public int Aura { get; set; } = 0;

        [Column("bio")]
        [FirestoreProperty("bio")]
        public string? Bio { get; set; }

        [Column("profile_picture")]
        [FirestoreProperty("profile_picture")]
        public string? ProfilePicture { get; set; }

        [Column("account_status")]
        [FirestoreProperty("account_status")]
        public UserStatus AccountStatus { get; set; } = UserStatus.Offline;
        
        [Column("role")]
        [FirestoreProperty("role")]
        public UserRole Role { get; set; } = UserRole.User;

    }
}