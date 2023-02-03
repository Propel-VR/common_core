using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterTaskHelper : MonoBehaviour
{

    int chapterID;

    Task task;
    [SerializeField]
    Sprite chapterSprite;

    private void Awake()
    {
        task = GetComponent<Task>();

        task.OnTaskCompleted.AddListener(OnComplete);
    }

    public void SetUp()
    {

        Debug.Log("SET UP CHAPTER WITH ID: " + chapterID);

        chapterID = Tablet.Instance.AddChapter(gameObject.name, chapterSprite);

        AddTaskToTablet(transform);

    }

    public void OnComplete()
    {

        Debug.Log("CHAPTER COMPLETE FOR \"" + gameObject.name + "\"");
        Tablet.Instance.CompleteChapterItem(chapterID);
    }

    private void AddTaskToTablet(Transform obj)
    {

        ChecklistTaskHelper cth = obj.GetComponent<ChecklistTaskHelper>();
        if (cth != null)
            cth.SetUp(chapterID);


        for (int i = 0; i < obj.childCount; i++)
            AddTaskToTablet(obj.GetChild(i));
    }
}
