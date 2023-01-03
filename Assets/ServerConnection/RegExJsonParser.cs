using System.Text.RegularExpressions;

namespace ServerConnection
{
    public static class RegExJsonParser
    {
        public static string GetValueOfField(string field, string json)
        {
            var pattern = $"(?<=\"{field}\":\").[^\"]*";
            Regex regex = new Regex(pattern);
            var result = regex.Match(json);
            return result.Value;
        }
    }
}