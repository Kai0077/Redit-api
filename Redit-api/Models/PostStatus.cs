using System.Runtime.Serialization;

namespace Redit_api.Models.Status
{
    public enum PostStatus
    {
        [EnumMember(Value = "active")]
        Active,
        [EnumMember(Value = "archived")]
        Archived
    }
}