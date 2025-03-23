using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    public TMP_Text header;
    public GameObject pathPanel;
    public GameObject subjectPathPanel;
    public GameObject subjectTemplate;
    public GameObject lessonTemplate;
    public GameObject backButton;

    [SerializeField] public List<Subject> subjects;

    private List<GameObject> createdSubjects = new List<GameObject>();
    private List<GameObject> createdLessons = new List<GameObject>();

    void Start()
    {
        // Wenn ein Scenenwechsel von einer Lektion zum Pfad stattfindet, wird direkt der Themenbereich geladen
        if (SceneData.currentSubject != 0)
            LoadLessonPath(SceneData.currentSubject - 1);
        else
            LoadSubjectPath();
    }

    // Wird beim erstmaligen Laden der Scene aufgerufen
    public void LoadSubjectPath()
    {
        DestroyObjects(createdSubjects); // Zerstört alle vorher erstellten Objekte
        subjectTemplate.SetActive(true); // Aktiviert Template-Objekt

        subjectPathPanel.SetActive(false); // Deaktiviere den Lektionspfad
        pathPanel.SetActive(true); // Aktiviere den Lernpfad mit den Themenbereichen
        backButton.SetActive(false); // Deaktiviere den Zurück-Button

        header.text = "Dein C#-Lernpfad";

        for (var i = 0; i < subjects.Count; i++)
        {
            var subjectItem = Instantiate(subjectTemplate, pathPanel.transform); // Für jedes Subject wird eine neue Instanz des subjectTemplate Objekts erstellt
            createdSubjects.Add(subjectItem); // Speichern der Objekte in eine Liste, um sie später löschen zu können
            var subjectGO = subjectItem.GetComponent<SubjectGO>(); // Enthält die Komponenten, die befüllt werden müssen
            var subjectButton = subjectItem.GetComponent<Button>();

            // Zuweisung der Werte aus der Liste
            subjectItem.name = subjects[i].name + " Object";
            subjectGO.header.text = subjects[i].name;
            subjectGO.image.sprite = subjects[i].sprite;
            subjectGO.progressBar.fillAmount = (float)subjects[i].lessonsCompleted / (float)subjects[i].lessonCount;
            subjectGO.progressText.text = subjects[i].lessonsCompleted + "/" + subjects[i].lessonCount;

            var index = i;
            subjectButton.onClick.AddListener(() => {LoadLessonPath(index);}); // beim Anklicken eines Subjects werden die zugehörigen Lektionen geladen
        }

        subjectTemplate.SetActive(false); // Deaktiviert Template-Objekt
    }

    // Wird beim Anklicken eines Themenbereichs aufgerufen. Dabei wird der zugehörige subjectIndex zur Zuordnung mitgegeben.
    public void LoadLessonPath(int subjectIndex)
    {
        DestroyObjects(createdLessons); // Zerstört alle vorher erstellten Objekte
        lessonTemplate.SetActive(true); // Aktiviert Template-Objekt

        pathPanel.SetActive(false); // Deaktiviere den Lernpfad mit den Themenbereichen
        subjectPathPanel.SetActive(true); // Aktiviere den Lektionspfad
        backButton.SetActive(true); // Aktiviere den Zurück-Button

        SceneData.currentSubject = subjectIndex + 1; // Speichern des aktuellen Themenbereichs für den Scenenwechsel
        
        header.text = subjects[subjectIndex].name; // Setze den Header mit dem Namen des gewählten Themenbereichs

        var lessons = subjects[subjectIndex].lessons;

        for (int i = 0; i < lessons.Count; i++)
        {
            var lessonItem = Instantiate(lessonTemplate, subjectPathPanel.transform); // Für jede Lesson wird eine neue Instanz des lessonTemplate Objekts erstellt
            createdLessons.Add(lessonItem); // Speichern der Objekte in eine Liste, um sie später löschen zu können
            var lessonGO = lessonItem.GetComponent<LessonGO>(); // Enthält die Komponenten, die befüllt werden müssen
            var lessonButton = lessonItem.GetComponent<Button>();

            // Zuweisung der Werte aus der Liste
            lessonItem.name = lessons[i].name + " Object";
            lessonGO.header.text = lessons[i].name;
            lessonGO.image.sprite = subjects[subjectIndex].lessonSprite;
            lessonGO.counter.text = "{ " + lessons[i].number  + " }";

            var index = i;
            lessonButton.onClick.AddListener(() => {LoadLesson(index + 1);}); // beim Anklicken einer Lesson wird die zugehörigen Lektion in der LessonScene geladen
        }

        lessonTemplate.SetActive(false); // Deaktiviert Template-Objekt
    }

    public void LoadLesson(int lessonIndex)
    {
        SceneData.currentLesson = lessonIndex; // Speichern der aktuellen Lektion für den Scenenwechsel
        SceneManager.LoadScene("LessonScene");
    }

    private void DestroyObjects(List<GameObject> gameObjects)
    {
        foreach (var gameObject in gameObjects) 
        {
            Destroy(gameObject);
        }
    }
}
