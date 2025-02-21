using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class IDE : MonoBehaviour
{
    public TMP_InputField codeInput; // Das Eingabefeld für den Code
    public CodeFormatter codeFormatter; // Script zum Formattieren des Codes
    public IntelliSense intelliSense; // Script für Codevorschläge
    public CodeInterpreter codeInterpreter; // Script zum Interpretieren des Codes
    
    public TMP_Text console; // TMP-Element für Consolen-Outputs

    void Start()
    {
        codeInput.onValueChanged.AddListener(OnCodeChanged);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CompileAndRunCode();
        }
    }

    void OnCodeChanged(string code)
    {
        // Event deaktivieren, um Endlosschleife zu vermeiden
        codeInput.onValueChanged.RemoveListener(OnCodeChanged);

        int caretPosition = codeInput.caretPosition;

        // Farben Entfernen
        codeInput.text = SyntaxHighlighter.RemoveHighlighting(codeInput.text);

        SetCaretPosition(caretPosition);
        
        // Code formattieren
        int caretPositionFormat = codeFormatter.FormatCode(codeInput);

        caretPosition = caretPositionFormat != -1 ? caretPositionFormat : caretPosition;

        // Code-Vorschläge einblenden
        intelliSense.ShowSuggestions(codeInput);

        // Code farblich highlighten
        codeInput.text = SyntaxHighlighter.ApplyHighlighting(codeInput.text);

        // Event wieder aktivieren
        codeInput.onValueChanged.AddListener(OnCodeChanged);

        // Cursor setzen
        SetCaretPosition(caretPosition);
    }

    // Wandelt die caretPosition in stringPosition um und setzt sie
    private void SetCaretPosition(int caretPosition)
    {
        var charInfo = codeInput.textComponent.textInfo.characterInfo;

        if(charInfo.Length <= caretPosition || charInfo[caretPosition].character == '\0')
        {
            codeInput.stringPosition = caretPosition;
        }
        else
        {
            codeInput.stringPosition = charInfo[caretPosition].index;
        }
    }
    
    // Führt den in der IDE geschriebenen Code aus
    public void CompileAndRunCode()
    {
        console.text = "";
        
        var code = SyntaxHighlighter.RemoveHighlighting(codeInput.text);
        
        string compileError = Compiler.CompileCode(code);

        if (compileError != "")
        {
            console.text += LogInConsole(compileError, Color.red);
        }
        else
        {
            // Execute Code
            codeInterpreter.ExecuteCode(code);
        }
    }

    public static string LogInConsole(string text, Color color)
    {
        return $"<color=#{color.ToHexString()}>>> {text}</color>\n";
    }
}
