// Services/HtmlUtils.cs
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Redit_api.Services
{
    public static class HtmlUtils
    {
        public static string HtmlToPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // remove script/style
            foreach (var n in doc.DocumentNode.SelectNodes("//script|//style") ?? Enumerable.Empty<HtmlNode>())
                n.Remove();

            var text = doc.DocumentNode.InnerText;
            text = System.Net.WebUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }
        
    }
}