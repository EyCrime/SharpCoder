using UnityEngine;
using TMPro;
using System.Linq;

public class IntelliSense : MonoBehaviour
{
    public GameObject suggestionPanel; // Panel für Vorschläge
    public TMP_Text suggestionText; // Textfeld für die Vorschläge
    public RectTransform suggestionPanelTransform; // UI-Position des Panels

    private TMP_InputField codeInput; // Das Eingabefeld für den Code
    private int selectedIndex = 0; // Index für die Auswahl in der Vorschlagsliste
    private string[] suggestions = new string[0]; // Array mit passenden Vorschlägen
    private bool isIntelliSenseActive = false; // Verhindert Cursorbewegung, wenn aktiv

    private void Start()
    {
        // Deaktiviere das Vorschlags-Panel zu Beginn
        suggestionPanel.SetActive(false);
    }

    private void Update()
    {
        // Falls Vorschläge angezeigt werden, überprüfe Eingabetasten
        if (suggestionPanel.activeSelf)
        {
            isIntelliSenseActive = true;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Nach unten navigieren in der Vorschlagsliste
                selectedIndex = Mathf.Min(selectedIndex + 1, suggestions.Length - 1);
                UpdateSuggestionHighlight();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Nach oben navigieren in der Vorschlagsliste
                selectedIndex = Mathf.Max(selectedIndex - 1, 0);
                UpdateSuggestionHighlight();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                // Auswählen des aktuellen Vorschlags
                ApplySuggestion();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Schließt das Vorschlags-Panel, wenn ESC gedrückt wird
                ResetSuggestions();
            }
        }
        else 
        {
            isIntelliSenseActive = false;
        }
    }

    private void OnGUI()
    {
        var keyCode = Event.current.keyCode;
        if (isIntelliSenseActive && (keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow || keyCode == KeyCode.Return))
        {
            Event.current.Use(); // Verhindert, dass das InputField den Key verarbeitet, wenn das SuggestionPanel offen ist
        }
    }
    
    // Wird aufgerufen, wenn sich der Text im InputField ändert.
    public int ShowSuggestions(TMP_InputField codeInput)
    {
        this.codeInput = codeInput;
        int cursorPos = codeInput.stringPosition;
        string code = codeInput.text;

        // Verhindere Fehler, wenn der Cursor an der falschen Position ist
        if (cursorPos != 0 && cursorPos <= code.Length)
        {
            char lastChar = code[cursorPos - 1];

            // Öffne das SuggestionPanel, wenn der Benutzer alles außer eine neue Zeile eingibt
            if (lastChar == '\n' && !suggestionPanel.activeSelf) return cursorPos;

            char[] searchChars = { '.', '\n', ' ', '(' };

            int codeEnd = lastChar == '\n' ? cursorPos - 2 : cursorPos - 1;

            // Finde den Anfang des aktuellen Wortes (ab letztem Leerzeichen, Punkt oder der neuen Zeile)
            int startIndex = code.LastIndexOfAny(searchChars, codeEnd);
            if (startIndex == -1) startIndex = 0; // Falls kein Leerzeichen existiert

            // Extrahiere das aktuelle Wort
            string currentWord = code.Substring(startIndex, cursorPos - startIndex).Trim().Trim('.').Trim('(');
            if (string.IsNullOrEmpty(currentWord))
            {
                // Falls kein Wort gefunden wurde, schließe das Vorschlags-Panel
                ResetSuggestions();
                return cursorPos;
            }

            // Filtere die Liste mit C#-Keywords nach passenden Vorschlägen
            suggestions = CodeSet.Keywords.Where(k => k.StartsWith(currentWord)).ToArray();

            if (suggestions.Length > 0)
            {
                // Falls Vorschläge existieren, zeige das Panel und setze die Auswahl auf den ersten Eintrag
                suggestionPanel.SetActive(true);
                UpdateSuggestionPosition(); // Panel an die richtige Stelle setzen
                UpdateSuggestionHighlight();
            }
            else
            {
                // Falls keine Vorschläge passen, schließe das Panel
                ResetSuggestions();
            }
        }
        else
            ResetSuggestions();

        return cursorPos;
    }

    // Aktualisiert die Position des Panels unter dem Cursor.
    void UpdateSuggestionPosition()
    {
        TMP_TextInfo textInfo = codeInput.textComponent.textInfo;
        int cursorIndex = Mathf.Clamp(codeInput.stringPosition, 0, textInfo.characterCount - 1);

        if (textInfo.characterCount == 0) return;

        TMP_CharacterInfo charInfo = textInfo.characterInfo[cursorIndex];

        // Setze das Panel so, dass die linke obere Ecke des Panels unten rechts am Cursor beginnt
        suggestionPanelTransform.anchoredPosition = new Vector2(charInfo.bottomRight.x + 5, charInfo.bottomRight.y - 5); // 5px Abstand nach rechts und unten
    }


    // Hebt den aktuell ausgewählten Vorschlag hervor.
    void UpdateSuggestionHighlight()
    {
        suggestionText.text = ""; // Leere die Vorschlagsliste

        for (int i = 0; i < suggestions.Length; i++)
        {
            if (i == selectedIndex)
                suggestionText.text += $"<b><color=yellow>{suggestions[i]}</color></b>\n"; // Markiert den aktuell ausgewählten Vorschlag
            else
                suggestionText.text += $"{suggestions[i]}\n"; // Normaler Text für die restlichen Vorschläge
        }
    }

    // Ersetzt das aktuelle Wort im InputField mit dem ausgewählten Vorschlag.
    void ApplySuggestion()
    {
        string code = SyntaxHighlighter.RemoveHighlighting(codeInput.text);

        if (suggestions.Length == 0) return; // Falls keine Vorschläge vorhanden sind, nichts tun

        int pos = codeInput.caretPosition;

        char[] searchChars = { '.', '\n', ' ', '(' };

        int codeEnd = code[pos - 1] == '\n' ? pos - 2 : pos - 1;

        int startIndex = code.LastIndexOfAny(searchChars, codeEnd);

        // Zerlegen des Textes
        string beforeWord = code.Substring(0, startIndex + 1);
        string afterWord = code.Substring(pos);
        string selectedWord = suggestions[selectedIndex];

        if (beforeWord.EndsWith("\n    "))
            beforeWord = beforeWord.Substring(0, beforeWord.Length - 6); // Entferne Formatierung die durch CodeFormatter vorher hinzugefügt wird

        // Ersetze das aktuelle Wort mit dem Vorschlag
        codeInput.text = beforeWord + selectedWord + afterWord;

        codeInput.caretPosition = beforeWord.Length + selectedWord.Length;

        // Schließe das Vorschlags-Panel nach der Auswahl
        ResetSuggestions();
    }

    // Schließt das Vorschlags-Panel und setzt die Auswahl zurück.
    void ResetSuggestions()
    {
        suggestionPanel.SetActive(false);
        suggestions = new string[0];
        selectedIndex = 0;
        isIntelliSenseActive = false;
    }
}