using System.Text.RegularExpressions;
using Google.Cloud.Firestore;
using Redit_api.Models.Status;

namespace Redit_api.Util;

public class PostStatusConverter : IFirestoreConverter<PostStatus>
{
    public object ToFirestore(PostStatus status)
    {
        return Regex.Replace(status.ToString(), @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    public PostStatus FromFirestore(object status)
    {
        if (status == null) return PostStatus.Active;

        var str = status.ToString()?.Trim();
        if (string.IsNullOrEmpty(str)) return PostStatus.Active;

        // snake_case -> PascalCase
        var pascal = Regex.Replace(str, "_([a-z])", m => m.Groups[1].Value.ToUpper());
        pascal = char.ToUpper(pascal[0]) + pascal.Substring(1);

        return Enum.TryParse<PostStatus>(pascal, ignoreCase: true, out var parsed)
            ? parsed
            : PostStatus.Active;
    }
}