using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CodeFormatter : MonoBehaviour
{
    // Formatiert den Code, sodass Klammern automatisch geschlossen und bzw. Anführungszeichen ergänzt werden
    // Außerdem wird bei Enter in einem Codeblock oder zwischen zwei gescheiften Klammer der Codeblock aufgebaut
    public int FormatCode(TMP_InputField codeInput)
    {
        int pos = codeInput.stringPosition;
        string code = codeInput.text;

        if(pos == 0) return -1;

        char closingChar = GetClosingChar(code[pos - 1]); // Gibt das zugehörige schließende Zeichen zurück

        if (closingChar != ' ')
        {
            codeInput.text = code.Insert(pos, closingChar.ToString()); // Fügt das schließende Zeichen ein
        }
        
        // Prüfe, ob die Enter-Taste gedrückt wurde
        if (Input.GetKeyDown(KeyCode.Return))
        {
            pos = OnEnterPressed(codeInput);
        }
        else
        {
            pos = -1;
        }

        return pos;
    }

    // Gibt die schließende Klammern oder das Anführungszeichen zurück
    private char GetClosingChar(char lastChar)
    {
        switch (lastChar)
        {
            case '(':
                return ')';
            case '{':
                return '}';
            case '[':
                return ']';
            case '"':
                return '\"';
            case '\'':
                return '\'';
            default:
                return ' ';
        }
    }

    // Sorgt für korrekte Einrückung nach Drücken der Enter-Taste.
    private int OnEnterPressed(TMP_InputField codeInput)
    {
        int pos = codeInput.stringPosition;
        string code = codeInput.text;
        
        if (pos > 0 && pos < code.Length)
        {
            int indentLevel = GetIndentationLevel(code, pos - 2);
            string indentInLine = new string(' ', (indentLevel + 1) * 4); // Nächste Einrückungsebene berechnen
            string indentEnd = new string(' ', indentLevel * 4); // Nächste Einrückungsebene berechnen

            if (code[pos - 2] == '{' && code[pos] == '}')
            {
                codeInput.text = code.Insert(pos, indentInLine + "\n" + indentEnd);
                pos += indentInLine.Length;
            }
            else
            {
                codeInput.text = code.Insert(pos, indentEnd);
                pos += indentEnd.Length;
            }
        }

        return pos;
    }

    // Berechnet die aktuelle Einrückungsebene basierend auf geschweiften Klammern.
    private int GetIndentationLevel(string code, int position)
    {
        int indentLevel = 0;
        for (int i = 0; i < position; i++)
        {
            if (code[i] == '{') indentLevel++;
            if (code[i] == '}') indentLevel--;
        }
        return Mathf.Max(indentLevel, 0); // Verhindert negative Einrückungsebenen
    }
}
