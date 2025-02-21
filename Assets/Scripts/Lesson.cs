using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[Serializable] 
public class Lesson
{
    public int number; // Lektionsnummer
    public string header; // Titel der Lektion
    public string exercise; // Aufgabenbeschreibung
    public string boardText; // Text der an der Tafel stehen soll
    public string targetSentence; // Erwartetes Ergebnis
    [TextArea(3,10)] public string documentation; // Erklärungen zur Lektion

    public List<CommandType> allowedCommands; // Erlaubte Befehle für die Lektion
}

