using CommonCoreScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for setting up chapter details for tablet
/// </summary>
public class ChapterTaskHelper : MonoBehaviour
{

    int _chapterID;

    Task _task;
    [SerializeField]
    Sprite _chapterSprite, _backgroundSprite;

    private void Awake()
    {
        _task = GetComponent<Task>();

        _task.OnTaskCompleted.AddListener(OnComplete);
    }

    /// <summary>
    /// called from checklist task manager to setup tablet
    /// </summary>
    public void SetUp()
    {

        Debug.Log("SET UP CHAPTER WITH ID: " + _chapterID);

        _chapterID = Tablet.Instance.AddChapter(gameObject.name, _chapterSprite, _backgroundSprite);

        AddTaskToTablet(transform);

    }

    /// <summary>
    /// called to update tablet when chapter is complete
    /// </summary>
    public void OnComplete()
    {

        Debug.Log("CHAPTER COMPLETE FOR \"" + gameObject.name + "\"");
        Tablet.Instance.CompleteChapterItem(_chapterID);
    }

    /// <summary>
    /// Adds all tasks from chapter to tablet
    /// </summary>
    /// <param name="obj"></param>
    private void AddTaskToTablet(Transform obj)
    {

        ChecklistTaskHelper cth = obj.GetComponent<ChecklistTaskHelper>();
        if (cth != null)
            cth.SetUp(_chapterID);


        for (int i = 0; i < obj.childCount; i++)
            AddTaskToTablet(obj.GetChild(i));
    }
}
