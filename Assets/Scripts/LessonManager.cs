using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class LessonManager : MonoBehaviour
{
    public TMP_Text lessonExercise; // UI-Text für die aktuelle Aufgabe
    public TMP_Text lessonHeader; // UI-Text für die Überschrift
    public TMP_Text lessonBoard; // UI-Text auf der Tafel
    public TMP_Text lessonDocu; // UI-Text für die Dokumentation
    public TMP_Text console; // UI-Text für die Console

    public int currentLesson = 1; // Speicherung des aktuellen Levels

    public Color successColor;
    public Color errorColor;


    // Definiert die einzelnen Lektionen
    [SerializeField] public List<Lesson> lessons;

    private void Start()
    {
        if (SceneData.currentLesson != 0) currentLesson = SceneData.currentLesson;

        LoadLesson(currentLesson);
    }
    

    // Wird beim Start der LessonScene oder beim komplettieren einer vorherigen Lektion aufgerufen
    public void LoadLesson(int lessonNumber)
    {
        if (lessonNumber >= lessons.Count) return; // Lektion nicht vorhanden

        currentLesson = lessonNumber;

        // Zuweisung der UI-Texte mit den Lektionsdaten aus der Liste
        lessonHeader.text = lessons[lessonNumber].header;
        lessonExercise.text = lessons[lessonNumber].exercise;
        lessonBoard.text = lessons[lessonNumber].boardText;

        console.text = ""; // leere die Konsole
    }

    public bool IsCommandAllowed(CommandType command)
    {
        return lessons[currentLesson].allowedCommands.Contains(command);
    }

    // Ruft je nachdem, welche Lektion gerade aktiv ist, dessen Überprüfungsmethode auf
    public void CheckLessonCompletion(char input)
    {
        if (currentLesson == 1)
        {
            CheckLessonOneComplete();
        }
        else if (currentLesson == 2 && input != '\0')
        {
            CheckLessonTwoComplete(input);
        }
    }

    public void BackToPath()
    {
        SceneManager.LoadScene("LessonPathScene");
    }

    // Überprüfe, ob Lesson 1 geschafft wurde
    private void CheckLessonOneComplete()
    {
        // Level geschafft
        if (lessonBoard.text.Contains(lessons[1].targetSentence))
        {
            // Gebe Message in Konsole aus
            lessonBoard.text = $"<color=#{successColor.ToHexString()}>Hello World!</color>";
            console.text += IDE.LogInConsole("Lektion 1 abgeschlossen! Weiter zur nächsten Aufgabe...", successColor);

            Invoke(nameof(NextLesson), 2f); // Wechsle zur nächsten Lektion nach 2 Sekunden
        }
        else
        {
            // Level noch nicht erfolgreich -> Gebe Fehlermeldung in Konsole aus
            lessonBoard.text = $"<color=#{errorColor.ToHexString()}>" + lessonBoard.text + "</color>";
            console.text += IDE.LogInConsole($"\"{lessonBoard.text}\" ist nicht das gesuchte Wort!", errorColor);
        }
    }

    // Überprüfe, ob Lesson 2 geschafft wurde
    private void CheckLessonTwoComplete(char input)
    {
        string targetSentence = lessons[2].targetSentence; // Erwartetes Ergebnis
        char pressedKey = input.ToString().ToUpper()[0]; // Erfasst den gedrückten Buchstaben

        // Es wurde kein Buchstabe gedrückt
        if (!char.IsLetter(pressedKey)) 
        {
            console.text += IDE.LogInConsole("Bitte drücke einen Buchstaben!", Color.white);
            return;
        }

        // Aktualisiere den Satz mit dem Buchstaben
        bool foundLetter = false;
        char[] boardText = lessonBoard.text.ToCharArray();

        for (int i = 0; i < targetSentence.Length; i++)
        {
            if (targetSentence[i] == pressedKey)
            {
                boardText[i] = pressedKey;
                foundLetter = true;
            }
        }

        if (foundLetter)
        {
            console.text += IDE.LogInConsole($"'{pressedKey}' gefunden!", successColor);
        }
        else
        {
            console.text += IDE.LogInConsole($"'{pressedKey}' nicht im Satz!", errorColor);
        }

        lessonBoard.text = string.Join("", boardText); // Setzt das aktualisierte Wort auf die Tafel

        // Prüfe, ob der Satz vollständig gelöst ist
        if (lessonBoard.text == targetSentence)
        {
            console.text += IDE.LogInConsole("Satz vollständig gelöst! Lektion abgeschlossen!", successColor);
            Invoke(nameof(NextLesson), 2f);
        }
    }

    // Lädt die nächste Lektion
    private void NextLesson()
    {
        LoadLesson(currentLesson + 1);
    }
}
