using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that manages the task checklists
/// </summary>
public class ChecklistTaskManager : MonoBehaviour
{
    [SerializeField]
    int _forceStartChapter=3;

    private void Start()
    {
        AddChapterToTablet(transform);

        Tablet.Instance.UpdateChapters();

        Tablet.Instance.StartChapter(_forceStartChapter);
    }



   
    /// <summary>
    /// Cycles through all objects in tasks and adds chapters to the tablet
    /// </summary>
    /// <param name="obj"></param>
    private void AddChapterToTablet(Transform obj)
    {
        ChapterTaskHelper cth = obj.GetComponent<ChapterTaskHelper>();
        if (cth != null)
        cth.SetUp();


        for (int i = 0; i < obj.childCount; i++)
            AddChapterToTablet(obj.GetChild(i));
    }
}
