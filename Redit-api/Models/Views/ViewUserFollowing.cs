using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Redit_api.Models.Views
{
    [Keyless]
    [Table("v_user_following")]
    public class ViewUserFollowing
    {
        [Column("source_username")]
        public string SourceUsername { get; set; } = null!;

        [Column("following_username")]
        public string FollowingUsername { get; set; } = null!;
    }
}