using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Redit_api.Models.Views
{
    [Keyless]
    [Table("v_user_followers")]
    public class ViewUserFollowers
    {
        [Column("target_username")]
        public string TargetUsername { get; set; } = null!;

        [Column("follower_username")]
        public string FollowerUsername { get; set; } = null!;
    }
}