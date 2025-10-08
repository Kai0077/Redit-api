using NpgsqlTypes;

namespace Redit_api.Models.Status
{
    public enum UserStatus
    {
        [PgName("online")]          
        Online,
        [PgName("do_not_disturb")]  
        DoNotDisturb,
        [PgName("idle")]            
        Idle,
        [PgName("offline")]         
        Offline,
        [PgName("invisible")]       
        Invisible
    }
}