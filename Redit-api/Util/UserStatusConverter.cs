using System.Text.RegularExpressions;
using Google.Cloud.Firestore;
using Redit_api.Models;

namespace Redit_api.Util;

public class UserStatusConverter : IFirestoreConverter<UserStatus>
{
    public object ToFirestore(UserStatus status)
    {
        // Convert PascalCase -> snake_case
        return Regex.Replace(status.ToString(), @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    public UserStatus FromFirestore(object value)
    {
        if (value == null) return UserStatus.Offline;

        var str = value.ToString()?.Trim();
        if (string.IsNullOrEmpty(str)) return UserStatus.Offline;

        // snake_case -> PascalCase
        var pascal = Regex.Replace(str, "_([a-z])", m => m.Groups[1].Value.ToUpper());
        pascal = char.ToUpper(pascal[0]) + pascal.Substring(1);

        return Enum.TryParse<UserStatus>(pascal, ignoreCase: true, out var parsed)
            ? parsed
            : UserStatus.Offline;
    }
}
