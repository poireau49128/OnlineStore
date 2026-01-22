namespace Store.Application.Utilities;

public static class TransliterationHelper
{
    private static readonly Dictionary<char, string> TranslitMap = new()
    {
        {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
        {'е', "e"}, {'ё', "e"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
        {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
        {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
        {'у', "u"}, {'ф', "f"}, {'х', "h"}, {'ц', "c"}, {'ч', "ch"},
        {'ш', "sh"}, {'щ', "shch"}, {'ъ', ""}, {'ы', "y"}, {'ь', ""},
        {'э', "e"}, {'ю', "yu"}, {'я', "ya"}
    };

    public static string Transliterate(string text)
    {
        if (string. IsNullOrWhiteSpace(text))
            return string.Empty;

        var result = new System.Text.StringBuilder();
        
        foreach (var c in text. ToLowerInvariant())
        {
            if (TranslitMap. ContainsKey(c))
                result.Append(TranslitMap[c]);
            else if (char.IsLetterOrDigit(c))
                result.Append(c);
        }
        
        return result.ToString();
    }
}