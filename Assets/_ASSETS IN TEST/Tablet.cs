using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;

public class Tablet : MonoBehaviour
{

    [SerializeField]
    TabletScreen[] tabletScreens= null;

    [SerializeField]
    List<TOCChapterData> tocChapterData= new List<TOCChapterData>();

    [SerializeField]
    ChecklistItem[] checklistItems;

    [SerializeField]
    TOCChapterItem[] tocChapterItems;
    [SerializeField]
    TOCCurrentChapter currentChapter;

    [SerializeField]
    TextMeshProUGUI progressText;

    int checklistPageNum = 0;
    int currentChapterID;
    int pageSize;

    public static Tablet Instance { get; private set; }

    private void Awake()
    {
        pageSize=checklistItems.Length;
        Instance= this;
    }

    public void StartChapter(int chapterNum)
    {
        currentChapterID= chapterNum;

        TOCChapterData data = tocChapterData[chapterNum];
        data.Current = true;
        currentChapter.SetChapter(data.ChapterName, data.Image);
        tocChapterData[chapterNum] = data;

        UpdateChapters();
        UpdateChecklist(chapterNum);

    }

    public void StartTasks(int chapterID)
    {
        ChecklistItemData data = tocChapterData[chapterID].ChecklistItemData[0];
        data.Current = true;
        tocChapterData[chapterID].ChecklistItemData[0] = data;

        UpdateChecklist(chapterID);
    }


    public void CompleteItem(int id, int chapterID)
    {
        if (id >= tocChapterData[chapterID].ChecklistItemData.Count)
        {
            Debug.Log("OUT OF BOUND ID");
                return;
        }

        ChecklistItemData item = tocChapterData[chapterID].ChecklistItemData[id];

        item.Complete = true;
        item.Current = false;

        tocChapterData[chapterID].ChecklistItemData[id] = item;

        if(id< tocChapterData[chapterID].ChecklistItemData.Count-2) 
        {
        
            item = tocChapterData[chapterID].ChecklistItemData[id+1];
            item.Current= true;
            tocChapterData[chapterID].ChecklistItemData[id + 1] = item;
        }

        UpdateChecklist(chapterID);
    }

    public void CompleteChapterItem(int id)
    {
        if (id >= tocChapterData.Count)
        {
            Debug.Log("OUT OF BOUND ID");
            return;
        }

        TOCChapterData item = tocChapterData[id];

        item.Complete = true;
        item.Current = false;
     
        UpdateChapters();
    }

    public int AddCheckListItem(string itemText, int chapterID, bool complete, bool warning, bool caution)
    {

        ChecklistItemData item;

        item.ItemText = itemText;
        item.ID =tocChapterData[chapterID].ChecklistItemData.Count;
        item.Complete= complete;
        item.Fail = false;
        item.Current = false;
        item.HasWarning= warning;
        item.HasCaution = caution;
        tocChapterData[chapterID].ChecklistItemData.Add(item);

        return item.ID;

    }


    public int AddChapter(string chapterName, Sprite chapterImg, Sprite background)
    {
        TOCChapterData chapter;

        chapter.ChapterName = chapterName;
        chapter.ID = tocChapterData.Count;
        chapter.Complete = false;
        chapter.Fail = false;
        chapter.Current = false;
        chapter.Image = chapterImg;
        chapter.ChecklistItemData = new List<ChecklistItemData>();
        chapter.BackgroundSprite = background;
        tocChapterData.Add(chapter);


        return chapter.ID;
    }

    public void NextChecklistPage()
    {
        if (checklistPageNum+1 * pageSize >= tocChapterData[currentChapterID].ChecklistItemData.Count)
            return;

        checklistPageNum++;

        UpdateChecklist(currentChapterID);

    }

    public void PreviousChecklistPage()
    {
        if (checklistPageNum == 0)
            return;

        checklistPageNum--;

        UpdateChecklist(currentChapterID);
    }

    public void UpdateChecklist(int chapterID)
    {
        foreach(ChecklistItem item in checklistItems)
        {
            item.UnComplete();
        }

        for(int i=0; i<pageSize; i++)
        {
            int id = pageSize * checklistPageNum + i;
            if (id >= tocChapterData[chapterID].ChecklistItemData.Count || id < 0)
            {
                Debug.Log("failed to set item, out of bounds ID #" + id);
                checklistItems[i].SetText("");
            }
            else
            {
                Debug.Log("Setting item #" + i + ", using ID #" + id);
                checklistItems[i].SetText(tocChapterData[chapterID].ChecklistItemData[id].ItemText);
                if (tocChapterData[chapterID].ChecklistItemData[id].Complete)
                    checklistItems[i].Complete();
                if (tocChapterData[chapterID].ChecklistItemData[id].Fail)
                    checklistItems[i].Fail();
                if (tocChapterData[chapterID].ChecklistItemData[id].Current)
                    checklistItems[i].Current();
                checklistItems[i].SetWarningCaution(tocChapterData[chapterID].ChecklistItemData[id].HasWarning, tocChapterData[chapterID].ChecklistItemData[id].HasCaution);
                

            }

        }

        progressText.text = GetNumTaskComplete(chapterID) + "/" + (tocChapterData[chapterID].ChecklistItemData.Count - 1) + " complete";
    }

    public void UpdateChapters()
    {
        foreach (TOCChapterItem item in tocChapterItems)
        {
            item.UnComplete();
        }

        for (int i = 0; i < tocChapterItems.Count(); i++)
        {
            int id = i;
            if (id >= tocChapterData.Count || id < 0)
            {
                Debug.Log("failed to set chapter item, out of bounds ID #" + id);
                tocChapterItems[i].SetTextAndImage("",null);
            }
            else
            {
                Debug.Log("Setting chapter item #" + i + ", using ID #" + id);
                tocChapterItems[i].SetTextAndImage(tocChapterData[id].ChapterName, tocChapterData[id].BackgroundSprite);
                tocChapterItems[i].SetProgress(GetNumTaskComplete(id), tocChapterData[id].ChecklistItemData.Count);
                if (tocChapterData[id].Complete)
                    tocChapterItems[i].Complete();
                if (tocChapterData[id].Fail)
                    tocChapterItems[i].Fail();
                if (tocChapterData[id].Current)
                    tocChapterItems[i].Current();

            }

        }
    }


    public void ChangeToScreen(int screenID)
    {
        DisableScreens();
        EnableScreen(screenID);
    }

    private void EnableScreen(int screenID)
    {
        foreach (GameObject go in tabletScreens[screenID].gameObjects)
            go.SetActive(true);

    }

    private void DisableScreens()
    {
        foreach(TabletScreen screen in tabletScreens)
        {
            foreach(GameObject go in screen.gameObjects)
                go.SetActive(false);
        }
    }

    private int GetNumTaskComplete(int chapterNum)
    {
        int numTaskComplete = 0;
        foreach(ChecklistItemData data in tocChapterData[chapterNum].ChecklistItemData)
            if(data.Complete) numTaskComplete++;

        return numTaskComplete;
    }


    [System.Serializable]
    struct TabletScreen
    {
        public string screenName;
        public List<GameObject> gameObjects;
    }

    [System.Serializable]
    public struct ChecklistItemData
    {
        public int ID;
        public string ItemText;
        public bool Current;
        public bool Complete;
        public bool Fail;

        public bool HasWarning, HasCaution;
    }

    [System.Serializable]
    public struct TOCChapterData
    {
        public int ID;
        public string ChapterName;
        public bool Current;
        public bool Complete;
        public bool Fail;
        public Sprite Image;
        public Sprite BackgroundSprite;

        public List<ChecklistItemData> ChecklistItemData;
    }

}
