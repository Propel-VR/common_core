using NaughtyAttributes;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreflightReportUI : ReportUI
{
    public static PreflightReportUI Instance { get; private set; }


    [SerializeField]
    private List<PreflightReportData> data;

    [SerializeField]
    ReportSummary _summary;

    protected void Awake()
    {
        Instance = this;
    }

    [Sirenix.OdinInspector.Button]
    public override void Open()
    {
        Debug.LogWarning("OPENING");
        template.gameObject.SetActive(false);

        // Cleanup
        foreach (var formEntry in formEntries)
        {
            //Destroy(formEntry.gameObject);
            formEntry.gameObject.SetActive(false);
        }
        formEntries.Clear();

        // Create panel
        foreach (PreflightReportData rd in data)
        {
            
            GameObject formEntry = Instantiate(template, parent).gameObject;

            formEntry.GetComponent<PreflightReportItem>().Init(rd);

            formEntries.Add(formEntry.GetComponent<RectTransform>());

            formEntry.gameObject.SetActive(true);
            

        }


        //SET UP SUMMARY
        _summary.SetChecks(GetNumChecksComplete(), GetTotalChecks());
        _summary.SetRectifications(GetRectComplete(), GetRectTotal());
        _summary.SetSnags(GetSnagsCorrect(), GetSnagsTotal());

        FilterList();

        transform.GetChild(0).gameObject.SetActive(true);


    }

    public override void FilterList()
    {
        Debug.LogWarning("FILTERING");

        for (int i=0; i < formEntries.Count; i++)
        {
            if (isFilterSatisfied(data[i]))
            {

                formEntries[i].gameObject.SetActive(true);

            }
            else
            {
                formEntries[i].gameObject.SetActive(false);

            }

        }
    }



    private int GetNumChecksComplete()
    {
        return data.Count;
    }

    private int GetTotalChecks()
    {
        //CHANGE LATER
        return data.Count;
    }

    private int GetSnagsCorrect()
    {
        int count=0;
        foreach(PreflightReportData d in data)
        {
            if(d.CheckReq.Equals("Snag Found") && d.CheckDone.Equals("Snag Found"))
                count++;
        }

        return count;
    }

    private int GetSnagsTotal()
    {
        int count = 0;
        foreach (PreflightReportData d in data)
        {
            if (d.CheckReq.Equals("Snag Found"))
                count++;
        }

        return count;
    }

    private int GetRectComplete()
    {
        int count = 0;
        foreach (PreflightReportData d in data)
        {
            if (!d.RectificationReq.Equals("N/A") && d.RectificationDone.Equals(d.RectificationReq))
                count++;
        }

        return count;
    }

    private int GetRectTotal()
    {
        int count = 0;
        foreach (PreflightReportData d in data)
        {
            if (!d.RectificationReq.Equals("N/A"))
                count++;
        }

        return count;
    }


    private bool isFilterSatisfied(PreflightReportData rd)
    {
        switch (_filterIndex)
        {
            case 0:
                return true;
            case 1:

                if (rd.CheckReq.Equals("Snag Found"))
                    return true;
                return false;

            case 2:
                if (!rd.Evaluation)
                    return true;
                return false;

                default: return false;
        }
    }

    public void AddItemData(PreflightReportData d)
    {

        bool found = false;

        for(int i=0; i<data.Count;i++)
        {
            if (data[i].TaskName.Equals(d.TaskName))
            {
                found = true;
                
                if(data[i].Evaluation && !d.Evaluation)
                {
                    data[i].SetInfo(d);
                }

                data[i].numCmplt++;

                break;
            }
        }

        if(!found)
            data.Add(d);
    }

}



[System.Serializable]
public class PreflightReportData : ReportData
{

    public string CheckDone, CheckReq;
    public string RectificationDone, RectificationReq;
    public int numCmplt;

    public PreFlightInteractable Interactable;

    public Dictionary<string, object> kvp = new Dictionary<string, object>();

    public PreflightReportData()
    {

    }

    public PreflightReportData(string taskName, string checkDone, string checkReq, string recDone, string recReq, PreFlightInteractable interactable)
    {
        TaskName = taskName;
        CheckDone = checkDone;
        CheckReq = checkReq;
        RectificationDone = recDone;
        RectificationReq = recReq;
        numCmplt = 1;

        Interactable = interactable;


        Evaluation = CheckReq.Equals(CheckDone) && RectificationReq.Equals(RectificationDone);
    }


    public PreflightReportData(string taskName, string checkDone, string checkReq, string recDone, string recReq, bool eval, PreFlightInteractable interactable, Dictionary<string, object> d)
    {
        TaskName = taskName;
        CheckDone = checkDone;
        CheckReq = checkReq;
        RectificationDone = recDone;
        RectificationReq = recReq;
        kvp = d;

        Interactable = interactable;

        Evaluation = CheckReq.Equals(CheckDone) && RectificationReq.Equals(RectificationDone);
    }

    public void SetInfo(PreflightReportData d)
    {
        CheckDone = d.CheckDone;
        CheckReq = d.CheckReq;
        RectificationDone = d.RectificationDone;
        RectificationReq = d.RectificationReq;

        Interactable= d.Interactable;


        Evaluation = CheckReq.Equals(CheckDone) && RectificationReq.Equals(RectificationDone);

    }
}