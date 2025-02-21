using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Subject
{
    public string name; // Name des Themenbereichs
    public Sprite sprite; // Bild des Themenbereichs
    public Sprite lessonSprite; // Bild der zugehörigen Lektionen
    public List<LessonInPath> lessons; // Liste der enthaltenen Lektionen
    public int lessonCount; // Gesamtanzahl der Lektionen
    public int lessonsCompleted; // Anzahl der abgeschlossenen Lektionen
}
