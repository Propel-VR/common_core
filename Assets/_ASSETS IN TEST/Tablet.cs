using RootMotion;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

/// <summary>
/// Main script to control functionalitry and visuals of tablet
/// </summary>
public class Tablet : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI _tabletTitle;

    [SerializeField]
    string _titleText;

    [SerializeField]
    TabletScreen[] _tabletScreens= null;

    [SerializeField]
    List<TOCChapterData> _tocChapterData= new List<TOCChapterData>();

    [SerializeField]
    List<ChecklistItem> _checklistItems;

    [SerializeField]
    TOCChapterItem[] _tocChapterItems;
    [SerializeField]
    TOCCurrentChapter _currentChapter;

    [SerializeField]
    TextMeshProUGUI _progressText;

    [SerializeField]
    DetailedPage _detailedPage;

    [SerializeField]
    GameObject _checklistItem;
    [SerializeField]
    RectTransform _scrollContent;

    [SerializeField]
    TMP_FontAsset _headerFont;

    [SerializeField]
    AnalogStickSlider _analogSlider;
    //int checklistPageNum = 0;
    int _currentChapterID;
    //int pageSize;


    bool _blockerActive = false;

    [SerializeField]
    ScrollRect _scrollRect;
    [SerializeField]
    GameObject _scrollClickBlocker;


    public static Tablet Instance { get; private set; }

    private void Awake()
    {
        //pageSize=checklistItems.Count;
        Instance= this;
    }

    private void Start()
    {
        _tabletTitle.text = _titleText;
    }


    public void IsHovered(bool hovering)
    {
        _analogSlider.EnableScroll = hovering;
    }

    /// <summary>
    /// Called to start a chapter from the tablet
    /// </summary>
    /// <param name="chapterNum">chapter ID to start</param>
    public void StartChapter(int chapterNum)
    {
        _currentChapterID= chapterNum;

        TOCChapterData data = _tocChapterData[chapterNum];
        data.Current = true;
        _currentChapter.SetChapter(data.ChapterName, data.Image);
        _tocChapterData[chapterNum] = data;

        UpdateChapters();
        UpdateChecklist(chapterNum);

        StartTasks();

    }

    /// <summary>
    /// called to start the first checklist task in a chapter
    /// sets task to current and updates checklist visuals
    /// </summary>
    public void StartTasks()
    {
        for (int i = 0; i < _tocChapterData[_currentChapterID].ChecklistItemData.Count; i++)
        {
            if (!_tocChapterData[_currentChapterID].ChecklistItemData[i].IsHeader)
            {
                ChecklistItemData data = _tocChapterData[_currentChapterID].ChecklistItemData[i];
                data.Current = true;
                _tocChapterData[_currentChapterID].ChecklistItemData[i] = data;

                foreach (ChecklistInteractable item in data.Interactables)
                    item.TaskStarted();

                UpdateChecklist(_currentChapterID);
                break;
            }
        }
    }

    /// <summary>
    /// called when a checklist item is complete
    /// </summary>
    /// <param name="id">checklist item ID</param>
    /// <param name="chapterID">chapter ID of the chapter the task is in</param>
    public void CompleteItem(int id, int chapterID)
    {
        if (id >= _tocChapterData[chapterID].ChecklistItemData.Count)
        {
            Debug.Log("OUT OF BOUND ID");
                return;
        }


        ChecklistItemData item = _tocChapterData[chapterID].ChecklistItemData[id];

        item.CurrentQuantity = item.CurrentQuantity+1;
        Debug.Log("NEW QTY"+item.CurrentQuantity);

        _tocChapterData[chapterID].ChecklistItemData[id] = item;

        if (item.CurrentQuantity>=item.Quantity)
        {
            if (item.Current)
            {
                _analogSlider.AutoScroll(1.0f / _tocChapterData[chapterID].ChecklistItemData.Count);
                item.Complete = true;
                item.Current = false;

                _tocChapterData[chapterID].ChecklistItemData[id] = item;


                if (id <= _tocChapterData[chapterID].ChecklistItemData.Count - 2)
                {

                    item = _tocChapterData[chapterID].ChecklistItemData[id + 1];
                    if (!item.Complete)
                    {
                        item.Current = true;

                        for (int i = 0; i < item.Interactables.Count; i++)
                        {
                            if (!item.Interactables[i].IsComplete)
                            {
                                item.Interactables[i].TaskStarted();

                            }
                        }
                        _tocChapterData[chapterID].ChecklistItemData[id + 1] = item;
                    }
                }
            }



        }
        else
        {
            /*
            item.Complete = true;
            item.Current = false;

            _tocChapterData[chapterID].ChecklistItemData[id] = item;
            */

            
            for (int i=0; i < item.Interactables.Count; i++)
            {

                if (!item.Interactables[i].IsComplete)
                {

                    item.Interactables[i].TaskStarted();
                    break;
                }

            }
        }

        UpdateChecklist(chapterID);
    }

    /// <summary>
    /// called when a chapter is complete
    /// </summary>
    /// <param name="id"></param>
    public void CompleteChapterItem(int id)
    {
        if (id >= _tocChapterData.Count)
        {
            Debug.Log("OUT OF BOUND ID");
            return;
        }

        TOCChapterData item = _tocChapterData[id];

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
   public int AddCheckListItem(bool isHeader, string itemText, string itemDesc, int chapterID, bool complete, bool warning, bool caution, int quantity, string smin, bool hasDetails, ChecklistTaskHelper.DetailsPageData details, List<ChecklistInteractable> interactables)
    {

        GameObject newGO = GameObject.Instantiate(_checklistItem, _scrollContent);
        _checklistItems.Add(newGO.GetComponent<ChecklistItem>());

        _checklistItems[_checklistItems.Count - 1].SetButtonValue(_checklistItems.Count - 1, isHeader);
        newGO.SetActive(true);

        ChecklistItemData item;

        item.IsHeader = isHeader;
        item.ItemText = itemText;
        item.ItemDesc = itemDesc;
        item.ID = _tocChapterData[chapterID].ChecklistItemData.Count;
        item.Complete = complete;
        item.Fail = false;
        item.Current = false;
        item.HasWarning = warning;
        item.HasCaution = caution;
        item.CurrentQuantity = 0;
        item.Quantity = quantity;
        item.SMIN = smin;
        item.HasDetails = hasDetails;
        item.detailsPageData = details;
        item.Interactables = interactables;
        _tocChapterData[chapterID].ChecklistItemData.Add(item);

        if (item.IsHeader)
            _checklistItems[_checklistItems.Count - 1].SetTitleFont(_headerFont);

        return item.ID;

    }
    /// <summary>
    /// Called when a checklist item is clicked to show details page
    /// </summary>
    /// <param name="checkBtn">ID of the button pressed</param>
    public void ClickChecklist(int checkBtn)
    {
        if (_tocChapterData[_currentChapterID].ChecklistItemData[checkBtn].HasDetails)
        {
            ChangeToScreen(2);
            UpdateDetailsPage(_tocChapterData[_currentChapterID].ChecklistItemData[checkBtn].detailsPageData);
            
        }
    }

    /// <summary>
    /// updates details page using details data
    /// </summary>
    /// <param name="details"></param>
    private void UpdateDetailsPage(ChecklistTaskHelper.DetailsPageData details)
    {
        _detailedPage.SetDetails(details);
    }

    public void UpdateDetails(int chapterID, int checkID, string rectification)
    {
        
        ChecklistItemData item;

        item.IsHeader = _tocChapterData[chapterID].ChecklistItemData[checkID].IsHeader;
        item.ItemText = _tocChapterData[chapterID].ChecklistItemData[checkID].ItemText;
        item.ItemDesc = _tocChapterData[chapterID].ChecklistItemData[checkID].ItemDesc;
        item.ID = _tocChapterData[chapterID].ChecklistItemData[checkID].ID;
        item.Complete = _tocChapterData[chapterID].ChecklistItemData[checkID].Complete;
        item.Fail = _tocChapterData[chapterID].ChecklistItemData[checkID].Fail;
        item.Current = _tocChapterData[chapterID].ChecklistItemData[checkID].Current;
        item.HasWarning = _tocChapterData[chapterID].ChecklistItemData[checkID].HasWarning;
        item.HasCaution = _tocChapterData[chapterID].ChecklistItemData[checkID].HasCaution;
        item.CurrentQuantity = _tocChapterData[chapterID].ChecklistItemData[checkID].CurrentQuantity;
        item.Quantity = _tocChapterData[chapterID].ChecklistItemData[checkID].Quantity;
        item.SMIN = _tocChapterData[chapterID].ChecklistItemData[checkID].SMIN;
        item.HasDetails = _tocChapterData[chapterID].ChecklistItemData[checkID].HasDetails;
        item.detailsPageData = _tocChapterData[chapterID].ChecklistItemData[checkID].detailsPageData;
        item.detailsPageData.rectification = rectification;
        item.Interactables = _tocChapterData[chapterID].ChecklistItemData[checkID].Interactables;

        _tocChapterData[chapterID].ChecklistItemData[checkID]=item;
        
    }

    private void Update()
    {
        if (_blockerActive == false && Mathf.Abs(_scrollRect.velocity.y) > 40f)
        {
            _blockerActive = true;
            _scrollClickBlocker.SetActive(true);
        }
        else if (
            _blockerActive == true && Mathf.Abs(_scrollRect.velocity.y) <= 40f
        )
        {
            _blockerActive = false;
            _scrollClickBlocker.SetActive(false);
        }
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
        chapter.ID = _tocChapterData.Count;
        chapter.Complete = false;
        chapter.Fail = false;
        chapter.Current = false;
        chapter.Image = chapterImg;
        chapter.ChecklistItemData = new List<ChecklistItemData>();
        chapter.BackgroundSprite = background;
        _tocChapterData.Add(chapter);


        return chapter.ID;
    }

    /// <summary>
    /// flips to the next page in checklist
    /// </summary>
    /*public void NextChecklistPage()
    {
        if (checklistPageNum+1 * pageSize >= tocChapterData[currentChapterID].ChecklistItemData.Count)
            return;

        checklistPageNum++;

        UpdateChecklist(currentChapterID);

    }
    */


    /// <summary>
    /// flips to previous checklist page
    /// </summary>
    /*public void PreviousChecklistPage()
    {
        if (checklistPageNum == 0)
            return;

        checklistPageNum--;

        UpdateChecklist(currentChapterID);
    }
    */


    /// <summary>
    /// updates the checklist data
    /// </summary>
    /// <param name="chapterID">chapter to pull checklist items from</param>
    public void UpdateChecklist(int chapterID)
    {
        foreach(ChecklistItem item in _checklistItems)
        {
            item.UnComplete();
        }

        for(int i=0; i<_checklistItems.Count; i++)
        {
            int id = i;
            if (id >= _tocChapterData[chapterID].ChecklistItemData.Count || id < 0)
            {
                Debug.Log("failed to set item, out of bounds ID #" + id);
                _checklistItems[i].SetText("","");
            }
            else
            {
                Debug.Log("Setting item #" + i + ", using ID #" + id);
                _checklistItems[i].SetText(_tocChapterData[chapterID].ChecklistItemData[id].ItemText, _tocChapterData[chapterID].ChecklistItemData[id].ItemDesc);
                if (_tocChapterData[chapterID].ChecklistItemData[id].Complete)
                    _checklistItems[i].Complete();
                if (_tocChapterData[chapterID].ChecklistItemData[id].Fail)
                    _checklistItems[i].Fail();
                if (_tocChapterData[chapterID].ChecklistItemData[id].Current)
                    _checklistItems[i].Current();
                _checklistItems[i].SetQuantity(_tocChapterData[chapterID].ChecklistItemData[id].CurrentQuantity, _tocChapterData[chapterID].ChecklistItemData[id].Quantity);
                _checklistItems[i].SetSmin(_tocChapterData[chapterID].ChecklistItemData[id].SMIN);
                _checklistItems[i].SetWarningCaution(_tocChapterData[chapterID].ChecklistItemData[id].HasWarning, _tocChapterData[chapterID].ChecklistItemData[id].HasCaution);
                

            }

        }

        //update content size
        float h = 0;

        foreach(RectTransform rt in _scrollContent.GetComponentsInChildren<RectTransform>())
        {
            if(rt.gameObject.activeSelf)
                h += rt.rect.height;
        }

        _scrollContent.rect.SetHeight(h);

        _progressText.text = GetNumTaskComplete(chapterID) + " of " + (_tocChapterData[chapterID].ChecklistItemData.Count) + " complete";
    }

    public int GetNextComplete()
    {
        for(int i=0; i < _tocChapterData[_currentChapterID].ChecklistItemData.Count; i++)
        {
            if (!_tocChapterData[_currentChapterID].ChecklistItemData[i].Complete)
                return i;
        }

        return-1;
    }

    /// <summary>
    /// updates chapter page with newest chapter data
    /// </summary>
    public void UpdateChapters()
    {
        foreach (TOCChapterItem item in _tocChapterItems)
        {
            item.UnComplete();
        }

        for (int i = 0; i < _tocChapterItems.Count(); i++)
        {
            int id = i;
            if (id >= _tocChapterData.Count || id < 0)
            {
                Debug.Log("failed to set chapter item, out of bounds ID #" + id);
                _tocChapterItems[i].SetTextAndImage("",null);
            }
            else
            {
                Debug.Log("Setting chapter item #" + i + ", using ID #" + id);
                _tocChapterItems[i].SetTextAndImage(_tocChapterData[id].ChapterName, _tocChapterData[id].BackgroundSprite);
                _tocChapterItems[i].SetProgress(GetNumTaskComplete(id), _tocChapterData[id].ChecklistItemData.Count);
                if (_tocChapterData[id].Complete)
                    _tocChapterItems[i].Complete();
                if (_tocChapterData[id].Fail)
                    _tocChapterItems[i].Fail();
                if (_tocChapterData[id].Current)
                    _tocChapterItems[i].Current();

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
        foreach (GameObject go in _tabletScreens[screenID].gameObjects)
            go.SetActive(true);

    }

    /// <summary>
    /// disables all screens
    /// </summary>
    private void DisableScreens()
    {
        foreach(TabletScreen screen in _tabletScreens)
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
        foreach(ChecklistItemData data in _tocChapterData[chapterNum].ChecklistItemData)
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
        public bool IsHeader;
        public string ItemText;
        public string ItemDesc;
        public bool Current;
        public bool Complete;
        public bool Fail;
        public int CurrentQuantity;
        public int Quantity;
        public string SMIN;
        public bool HasWarning, HasCaution;
        public bool HasDetails;
        public List<ChecklistInteractable> Interactables;
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
