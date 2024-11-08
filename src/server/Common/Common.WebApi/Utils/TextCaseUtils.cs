namespace Common.WebApi.Utils;

public static class TextCaseUtils
{
    private static readonly char[] separator = [' ', '-', '_'];

    public static string ToPascalCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        string[] words = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i++)
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);

        return string.Join("", words);
    }
}