namespace Infrastructure.Persistence.Helpers;

using System.Text.RegularExpressions;


public static class HashtagHelper {

    public static List<string> ExtractHashtags(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            return new List<string>();

        // Matches valid hashtags:
        // - Starts with #
        // - Contains letters, numbers, underscores
        // - Doesn't start/end with underscore
        // - Minimum 2 characters after #
        var matches = Regex.Matches(inputText, @"(?<!\w)#\w{2,}(?!\w)");

        return matches
            .Select(m => m.Value.ToLower())// Normalize to lowercase
            .Distinct()// Remove duplicates
            .ToList();
    }

}
