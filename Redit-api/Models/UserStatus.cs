using System.Runtime.Serialization;

namespace Redit_api.Models
{
    public enum UserStatus
    {
        [EnumMember(Value = "offline")]
        Offline,
        [EnumMember(Value = "online")]
        Online,
        [EnumMember(Value = "do_not_disturb")]
        DoNotDisturb,
        [EnumMember(Value = "idle")]
        Idle,
        [EnumMember(Value = "invisible")]
        Invisible
    }
}