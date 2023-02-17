using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for setting up chapter details for tablet
/// </summary>
public class ChapterTaskHelper : MonoBehaviour
{

    int chapterID;

    Task task;
    [SerializeField]
    Sprite chapterSprite, backgroundSprite;

    private void Awake()
    {
        task = GetComponent<Task>();

        task.OnTaskCompleted.AddListener(OnComplete);
    }

    /// <summary>
    /// called from checklist task manager to setup tablet
    /// </summary>
    public void SetUp()
    {

        Debug.Log("SET UP CHAPTER WITH ID: " + chapterID);

        chapterID = Tablet.Instance.AddChapter(gameObject.name, chapterSprite, backgroundSprite);

        AddTaskToTablet(transform);

    }

    /// <summary>
    /// called to update tablet when chapter is complete
    /// </summary>
    public void OnComplete()
    {

        Debug.Log("CHAPTER COMPLETE FOR \"" + gameObject.name + "\"");
        Tablet.Instance.CompleteChapterItem(chapterID);
    }

    /// <summary>
    /// Adds all tasks from chapter to tablet
    /// </summary>
    /// <param name="obj"></param>
    private void AddTaskToTablet(Transform obj)
    {

        ChecklistTaskHelper cth = obj.GetComponent<ChecklistTaskHelper>();
        if (cth != null)
            cth.SetUp(chapterID);


        for (int i = 0; i < obj.childCount; i++)
            AddTaskToTablet(obj.GetChild(i));
    }
}
