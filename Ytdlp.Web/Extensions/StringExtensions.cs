using System.Text.RegularExpressions;

namespace Ytdlp.Web.Extensions;

public static partial class StringExtensions
{
    public static string ToFilename(this string name, string extension)
    {
        // 1. Normalize and validate the extension
        extension = extension.TrimStart('.').ToLower();
        if (extension.Length >= 128)
        {
            throw new ArgumentException("Extension too long.", nameof(extension));
        }

        // 2. Calculate max allowed name length
        int maxNameLength = 128 - 1 - extension.Length; // Account for the dot

        // 3. Replace invalid characters in the name and truncate if needed
        string safeName = Regex.Replace(name, "[^a-zA-Z0-9_.-]+", "_");
        safeName = Regex.Replace(safeName, "[_.-]+", "_");
        safeName = safeName.Trim('_');
        if (safeName.Length > maxNameLength)
        {
            safeName = safeName.Substring(0, maxNameLength);
        }

        // 4. Combine and return the filename
        return $"{safeName}.{extension}";
    }

    public static string SplitOnUpperCase(this string input)
    {
        // Regex pattern: 
        //  - Lookbehind for non-uppercase letter or start of string
        //  - Match an uppercase letter
        return SplitOnUpperCaseRegex().Replace(input, "-$0");
    }

    [GeneratedRegex(@"(?<!^|\p{Lu})\p{Lu}")]
    private static partial Regex SplitOnUpperCaseRegex();
}