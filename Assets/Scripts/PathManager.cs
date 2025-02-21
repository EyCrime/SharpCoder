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

    [SerializeField] public List<Subject> subjects;

    void Start()
    {
        LoadSubjectPath();
    }

    // Wird beim erstmaligen Laden der Scene aufgerufen
    public void LoadSubjectPath()
    {
        for (var i = 0; i < subjects.Count; i++)
        {
            var subjectItem = Instantiate(subjectTemplate, pathPanel.transform); // Für jedes Subject wird eine neue Instanz des subjectTemplate Objekts erstellt
            var subjectGO = subjectItem.GetComponent<SubjectGO>(); // Enthält die Komponenten, die befüllt werden müssen
            var subjectButton = subjectItem.GetComponent<Button>();

            // Zuweisung der Werte aus der Liste
            subjectGO.header.text = subjects[i].name;
            subjectGO.image.sprite = subjects[i].sprite;
            subjectGO.progressBar.fillAmount = (float)subjects[i].lessonsCompleted / (float)subjects[i].lessonCount;
            subjectGO.progressText.text = subjects[i].lessonsCompleted + "/" + subjects[i].lessonCount;

            var index = i;
            subjectButton.onClick.AddListener(() => {LoadLessonPath(index);}); // beim Anklicken eines Subjects werden die zugehörigen Lektionen geladen
        }

        Destroy(subjectTemplate); // Entfernt überflüssiges Template-Objekt
    }

    // Wird beim Anklicken eines Themenbereichs aufgerufen. Dabei wird der zugehörige subjectIndex zur Zuordnung mitgegeben.
    public void LoadLessonPath(int subjectIndex)
    {
        pathPanel.SetActive(false); // Deaktiviere den Lernpfad mit den Themenbereichen
        subjectPathPanel.SetActive(true); // Aktiviere den Lektionspfad

        header.text = subjects[subjectIndex].name; // Setze den Header mit dem Namen des gewählten Themenbereichs

        var lessons = subjects[subjectIndex].lessons;

        for (int i = 0; i < lessons.Count; i++)
        {
            var lessonItem = Instantiate(lessonTemplate, subjectPathPanel.transform); // Für jede Lesson wird eine neue Instanz des lessonTemplate Objekts erstellt
            var lessonGO = lessonItem.GetComponent<LessonGO>(); // Enthält die Komponenten, die befüllt werden müssen
            var lessonButton = lessonItem.GetComponent<Button>();

            // Zuweisung der Werte aus der Liste
            lessonGO.header.text = lessons[i].name;
            lessonGO.image.sprite = subjects[subjectIndex].lessonSprite;
            lessonGO.counter.text = "{ " + lessons[i].number  + " }";

            var index = i;
            lessonButton.onClick.AddListener(() => {LoadLesson(index);}); // beim Anklicken einer Lesson wird die zugehörigen Lektion in der LessonScene geladen
        }

        Destroy(lessonTemplate); // Entfernt überflüssiges Template-Objekt
    }

    public void LoadLesson(int lessonIndex)
    {
        SceneManager.LoadScene("LessonScene");
    }
}
