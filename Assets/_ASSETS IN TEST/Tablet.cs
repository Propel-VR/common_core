using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;

public class Tablet : MonoBehaviour
{

    [SerializeField]
    TabletScreen[] tabletScreens= null;

    [SerializeField]
    List<ChecklistItemData> checklistItemData=new List<ChecklistItemData>();

    [SerializeField]
    ChecklistItem[] checklistItems;

    int checklistPageNum = 0;
    int pageSize;

    public static Tablet Instance { get; private set; }

    private void Awake()
    {
        pageSize=checklistItems.Length;
        Instance= this;
    }

    public void StartTasks()
    {
        ChecklistItemData data = checklistItemData[0];
        data.Current = true;
        checklistItemData[0] = data;

        UpdateChecklist();
    }

    public void CompleteItem(int id)
    {
        if (id >= checklistItemData.Count)
        {
            Debug.Log("OUT OF BOUND ID");
                return;
        }

        ChecklistItemData item = checklistItemData[id];

        item.Complete = true;

        checklistItemData[id] = item;

        if(id<checklistItemData.Count-2) 
        {
        
            item = checklistItemData[id+1];
            item.Current= true;
        }

        UpdateChecklist();
    }

    public int AddCheckListItem(string itemText)
    {

        ChecklistItemData item;

        item.ItemText = itemText;
        item.ID = checklistItemData.Count;
        item.Complete= false;
        item.Fail = false;
        item.Current = false;
        checklistItemData.Add(item);

        return item.ID;

    }

    public void NextChecklistPage()
    {
        if (checklistPageNum+1 * pageSize >= checklistItemData.Count)
            return;

        checklistPageNum++;

        UpdateChecklist();

    }

    public void PreviousChecklistPage()
    {
        if (checklistPageNum == 0)
            return;

        checklistPageNum--;

        UpdateChecklist();
    }

    public void UpdateChecklist()
    {
        foreach(ChecklistItem item in checklistItems)
        {
            item.UnComplete();
        }

        for(int i=0; i<pageSize; i++)
        {
            int id = pageSize * checklistPageNum + i;
            if (id >= checklistItemData.Count && id >= 0)
            {
                Debug.Log("failed to set item, out of bounds ID #" + id);
                checklistItems[i].SetText("");
            }
            else
            {
                Debug.Log("Setting item #" + i + ", using ID #" + id);
                checklistItems[i].SetText(checklistItemData[id].ItemText);
                if (checklistItemData[id].Complete)
                    checklistItems[i].Complete();
                if (checklistItemData[id].Fail)
                    checklistItems[i].Fail();
                if (checklistItemData[id].Current)
                    checklistItems[i].Current();

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
    }
}
