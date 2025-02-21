using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CodeInterpreter : MonoBehaviour
{
    public TMP_Text blackBoard;
    public TMP_Text console;
    public LessonManager lessonManager;
    public Color errorColor;

    private Dictionary<string, string> variables = new Dictionary<string, string>();
    
    // Wird bei der Ausführung des Codes aufgerufen
    public void ExecuteCode(string code)
    {
        variables = new Dictionary<string, string>(); // Variablen zurücksetzen
        string[] commands = code.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string rawCommand in commands)
        {
            string command = rawCommand.Trim() + ";";
            if (string.IsNullOrEmpty(command)) continue;

             // Regex-Analyse, um herauszufinden, um was für einen Befehl es sich handelt
            Match match = Regex.Match(command, 
                @"^(Console\.(WriteLine|ReadKey|Clear|ReadLine))\s*\((.*?)\);|" + // Console-Befehle
                @"^(int|string|bool)\s+(\w+)\s*=\s*(.+);");                     // Variablen-Deklaration

            // Erlaubte Befehle je nach Lektion prüfen
            string commandKeyword = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[4].Value;
            CommandType commandType = GetCommandType(commandKeyword);

            if (!lessonManager.IsCommandAllowed(commandType))
            {
                console.text += IDE.LogInConsole($"Der Befehl '{commandKeyword}' ist nicht erlaubt!", errorColor);
                return;
            }

            switch (commandType)
            {
                case CommandType.ConsoleWriteLine:
                    ExecuteConsoleWriteLine(match.Groups[3].Value);
                    break;
                case CommandType.ConsoleReadKey:
                    ExecuteConsoleReadKey();
                    break;
                case CommandType.ConsoleClear:
                    ExecuteConsoleClear();
                    break;
                case CommandType.VariableDeclaration:
                    ExecuteVariableDeclaration(match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);
                    break;
                case CommandType.Unknown:
                    console.text += IDE.LogInConsole($"Unbekannter Befehl: {commandKeyword}", errorColor);
                    break;
            }
        }

        lessonManager.CheckLessonCompletion('\0');
    }

    // Methode für die Ausführung des Console.WriteLine()-Befehls
    private void ExecuteConsoleWriteLine(string message)
    {
        message = message.Trim('"');

        // Falls eine Variable statt Text eingegeben wurde, ersetze sie mit dem gespeicherten Wert
        if (variables.ContainsKey(message))
        {
            message = variables[message];
        }

        blackBoard.text = message;
        console.text += IDE.LogInConsole($"Console.WriteLine: \"{message}\"", Color.white);
    }

    // Methode für die Ausführung des Console.ReadKey()-Befehls
    private void ExecuteConsoleReadKey()
    {
        console.text += IDE.LogInConsole($"[Drücke eine beliebige Taste...]", Color.white);

        StartCoroutine(WaitForKeyPressCoroutine());
    }

    // Coroutine die auf die Tasteneingabe wartet
    private IEnumerator WaitForKeyPressCoroutine()
    {
        while (!Input.anyKeyDown || Input.inputString == "" || !char.IsLetterOrDigit(Input.inputString[0]))
        {
            yield return null;
        }

        var key = Input.inputString[0];
        console.text += IDE.LogInConsole($"[Taste '{key}' wurde erkannt]", Color.white);
        lessonManager.CheckLessonCompletion(key);
    }

    // Methode für die Ausführung des Console.Clear()-Befehls
    private void ExecuteConsoleClear()
    {
        blackBoard.text = "";
    }

    // Methode für die Variable-Deklaration
    private void ExecuteVariableDeclaration(string varType, string varName, string varValue)
    {
        if (!variables.ContainsKey(varName))
        {
            variables[varName] = varValue.Trim();
            blackBoard.text += $">> Variable gespeichert: {varType} {varName} = {varValue}\n";
        }
        else
        {
            blackBoard.text += $"<color=#ec1c24>>> Fehler: Variable '{varName}' existiert bereits!\n</color>";
        }
    }

    private CommandType GetCommandType(string commandKeyword)
    {
        return commandKeyword switch
        {
            "Console.WriteLine" => CommandType.ConsoleWriteLine,
            "Console.ReadKey" => CommandType.ConsoleReadKey,
            "Console.Clear" => CommandType.ConsoleClear,
            "Console.ReadLine" => CommandType.ConsoleReadLine,
            "int" or "string" or "bool" => CommandType.VariableDeclaration,
            _ => CommandType.Unknown
        };
    }
}
