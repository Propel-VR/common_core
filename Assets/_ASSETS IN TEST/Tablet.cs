using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;

/// <summary>
/// Main script to control functionalitry and visuals of tablet
/// </summary>
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

    [SerializeField]
    DetailedPage detailedPage;

    int checklistPageNum = 0;
    int currentChapterID;
    int pageSize;

    public static Tablet Instance { get; private set; }

    private void Awake()
    {
        pageSize=checklistItems.Length;
        Instance= this;
    }

    /// <summary>
    /// Called to start a chapter from the tablet
    /// </summary>
    /// <param name="chapterNum">chapter ID to start</param>
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

    /// <summary>
    /// called to start the first checklist task in a chapter
    /// sets task to current and updates checklist visuals
    /// </summary>
    /// <param name="chapterID"></param>
    public void StartTasks(int chapterID)
    {
        ChecklistItemData data = tocChapterData[chapterID].ChecklistItemData[0];
        data.Current = true;
        tocChapterData[chapterID].ChecklistItemData[0] = data;

        UpdateChecklist(chapterID);
    }

    /// <summary>
    /// called when a checklist item is complete
    /// </summary>
    /// <param name="id">checklist item ID</param>
    /// <param name="chapterID">chapter ID of the chapter the task is in</param>
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

    /// <summary>
    /// called when a chapter is complete
    /// </summary>
    /// <param name="id"></param>
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

    /// <summary>
    /// adds a new checklist item to the tablet then updates the checklist
    /// </summary>
    /// <param name="itemText">name of item</param>
    /// <param name="chapterID">id of chapter it belongs to</param>
    /// <param name="complete">if the task has been complete</param>
    /// <param name="warning">if the item has a warning</param>
    /// <param name="caution">if the item has a caution</param>
    /// <param name="quantity">quantity of the check</param>
    /// <param name="smin">SMIN of the item (empty if none)</param>
    /// <returns></returns>
    public int AddCheckListItem(string itemText, int chapterID, bool complete, bool warning, bool caution, int quantity, string smin, bool hasDetails, ChecklistTaskHelper.DetailsPageData details)
    {

        ChecklistItemData item;

        item.ItemText = itemText;
        item.ID =tocChapterData[chapterID].ChecklistItemData.Count;
        item.Complete= complete;
        item.Fail = false;
        item.Current = false;
        item.HasWarning= warning;
        item.HasCaution = caution;
        item.Quantity= quantity;
        item.SMIN = smin;
        item.HasDetails = hasDetails;
        item.detailsPageData= details;
        tocChapterData[chapterID].ChecklistItemData.Add(item);

        return item.ID;

    }

    public void ClickChecklist(int checkBtn)
    {
        if (tocChapterData[currentChapterID].ChecklistItemData[checklistPageNum * pageSize + checkBtn].HasDetails)
        {
            UpdateDetailsPage(tocChapterData[currentChapterID].ChecklistItemData[checklistPageNum * pageSize + checkBtn].detailsPageData);
            EnableScreen(2);
        }
    }

    private void UpdateDetailsPage(ChecklistTaskHelper.DetailsPageData details)
    {
        detailedPage.SetDetails(details);
    }

    /// <summary>
    /// adds a new chapter to the tablet
    /// </summary>
    /// <param name="chapterName">name of chapter</param>
    /// <param name="chapterImg">splash image for the chapter</param>
    /// <param name="background">background image of chapter</param>
    /// <returns></returns>
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

    /// <summary>
    /// flips to the next page in checklist
    /// </summary>
    public void NextChecklistPage()
    {
        if (checklistPageNum+1 * pageSize >= tocChapterData[currentChapterID].ChecklistItemData.Count)
            return;

        checklistPageNum++;

        UpdateChecklist(currentChapterID);

    }

    /// <summary>
    /// flips to previous checklist page
    /// </summary>
    public void PreviousChecklistPage()
    {
        if (checklistPageNum == 0)
            return;

        checklistPageNum--;

        UpdateChecklist(currentChapterID);
    }

    /// <summary>
    /// updates the checklist data
    /// </summary>
    /// <param name="chapterID">chapter to pull checklist items from</param>
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
                checklistItems[i].SetQuantity(0, tocChapterData[chapterID].ChecklistItemData[id].Quantity);
                checklistItems[i].SetSmin(tocChapterData[chapterID].ChecklistItemData[id].SMIN);
                checklistItems[i].SetWarningCaution(tocChapterData[chapterID].ChecklistItemData[id].HasWarning, tocChapterData[chapterID].ChecklistItemData[id].HasCaution);
                

            }

        }

        progressText.text = GetNumTaskComplete(chapterID) + "/" + (tocChapterData[chapterID].ChecklistItemData.Count - 1) + " complete";
    }

    /// <summary>
    /// updates chapter page with newest chapter data
    /// </summary>
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

    /// <summary>
    /// changes to a screen
    /// </summary>
    /// <param name="screenID">ID of screen to change to</param>
    public void ChangeToScreen(int screenID)
    {
        DisableScreens();
        EnableScreen(screenID);
    }

    /// <summary>
    /// enables a screen
    /// </summary>
    /// <param name="screenID"></param>
    private void EnableScreen(int screenID)
    {
        foreach (GameObject go in tabletScreens[screenID].gameObjects)
            go.SetActive(true);

    }

    /// <summary>
    /// disables all screens
    /// </summary>
    private void DisableScreens()
    {
        foreach(TabletScreen screen in tabletScreens)
        {
            foreach(GameObject go in screen.gameObjects)
                go.SetActive(false);
        }
    }

    /// <summary>
    /// returns number of tasks complete
    /// </summary>
    /// <param name="chapterNum">chapter to check</param>
    /// <returns>number of complete tasks</returns>
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
        public int Quantity;
        public string SMIN;
        public bool HasWarning, HasCaution;
        public bool HasDetails;
        public ChecklistTaskHelper.DetailsPageData detailsPageData;
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
