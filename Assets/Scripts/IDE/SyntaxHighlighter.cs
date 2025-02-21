using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class SyntaxHighlighter : MonoBehaviour
{
    public static string ApplyHighlighting(string code)
    {
        // 1 Schlüsselwörter in Blau hervorheben
        string keywordPattern = @"\b(class|public|private|protected|void|int|float|double|string|
        bool|if|else|for|while|return|new|static|using|namespace|this|base|break|continue|switch|
        case|default|try|catch|finally|throw|typeof|as|is|do|foreach|in|get|set|struct|enum|const|
        readonly|virtual|override|sealed|abstract|interface|implements|extends)\b";
        code = Regex.Replace(code, keywordPattern, "<color=#569CD6>$1</color>");

        // 2️ Strings in Orange hervorheben (alles zwischen " " oder ' ')
        string stringPattern = @"(\"".*?\""|'.*?')";
        code = Regex.Replace(code, stringPattern, "<color=#C3602C>$1</color>");

        // 3️ Zahlen in Rot (Ganzzahlen, Dezimalzahlen, Hexadezimalzahlen)
        string numberPattern = @"\b\d+(\.\d+)?\b|0x[0-9A-Fa-f]+";
        code = Regex.Replace(code, numberPattern, "<color=#A79941>$0</color>");

        // 4️ Einzeilige Kommentare in Grün ("// Kommentar")
        string singleLineCommentPattern = @"(//.*?$)";
        code = Regex.Replace(code, singleLineCommentPattern, "<color=#52993E>$1</color>", RegexOptions.Multiline);

        // 5️ Mehrzeilige Kommentare in Grün ("/* Kommentar */")
        string multiLineCommentPattern = @"/\*[\s\S]*?\*/";
        code = Regex.Replace(code, multiLineCommentPattern, "<color=#52993E>$0</color>");

        // 6️ Klammern farbig machen
        string bracketPattern = @"([\(\)\{\}\[\]])";
        code = Regex.Replace(code, bracketPattern, "<color=#DA63A1>$1</color>");

        return code;
    }

    // Entfernt sämtliche farbliche Darstellungen
    public static string RemoveHighlighting(string code)
    {
        return Regex.Replace(code, @"<color=[^>]+>|</color>", "");
    }
}
