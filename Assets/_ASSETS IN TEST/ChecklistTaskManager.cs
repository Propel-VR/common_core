using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecklistTaskManager : MonoBehaviour
{

    private void Start()
    {
        AddChapterToTablet(transform);

        Tablet.Instance.UpdateChapters();
    }



   

    private void AddChapterToTablet(Transform obj)
    {
        ChapterTaskHelper cth = obj.GetComponent<ChapterTaskHelper>();
        if (cth != null)
        cth.SetUp();


        for (int i = 0; i < obj.childCount; i++)
            AddChapterToTablet(obj.GetChild(i));
    }
}
