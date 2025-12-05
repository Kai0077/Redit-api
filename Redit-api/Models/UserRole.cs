using System.Runtime.Serialization;

namespace Redit_api.Models.Status
{
    public enum UserRole
    {
        [EnumMember(Value = "user")]
        User,
        [EnumMember(Value = "super_user")]
        SuperUser
    }
}