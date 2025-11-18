using System.Text.RegularExpressions;
using Google.Cloud.Firestore;
using Redit_api.Models.Status;

namespace Redit_api.Util;

public class UserRoleConverter : IFirestoreConverter<UserRole>
{
    public object ToFirestore(UserRole role)
    {
        return Regex.Replace(role.ToString(), @"([a-z0-9])([A-Z])", "$1_$2").ToLower();

    }

    public UserRole FromFirestore(object value)
    {
        if (value == null) return UserRole.User;

        var str = value.ToString()?.Trim();
        if (string.IsNullOrEmpty(str)) return UserRole.User;

        // snake_case -> PascalCase
        var pascal = Regex.Replace(str, "_([a-z])", m => m.Groups[1].Value.ToUpper());
        pascal = char.ToUpper(pascal[0]) + pascal.Substring(1);

        return Enum.TryParse<UserRole>(pascal, ignoreCase: true, out var parsed)
            ? parsed
            : UserRole.User;
    }
}