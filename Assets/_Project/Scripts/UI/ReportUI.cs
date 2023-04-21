using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ReportUI : MonoBehaviour
{

    public static ReportUI BaseInstance { get; private set; }

   
    [Header("References")]
    [SerializeField] protected RectTransform parent;
    [SerializeField] protected RectTransform template;
    [SerializeField] RectTransform infoTranform;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] Color FilterSelected, FilterUnselected;
    [SerializeField] TextMeshProUGUI[] filterTexts;

    protected int _filterIndex=0;

    protected Transform infoTarget;

    protected List<RectTransform> formEntries = new();


    private List<ReportData> reportData = new List<ReportData>();


    //List<RiskObjectAssesment> objects;

    protected void Awake()
    {
        BaseInstance= this;
    }

    private void Start ()
    {
        infoTranform.gameObject.SetActive(false);
    }



    public void SelectFilter(int filterIndex)
    {
        _filterIndex=filterIndex;

        foreach(TextMeshProUGUI t in filterTexts)
        {
            t.color = FilterUnselected;
            t.fontStyle = FontStyles.Normal;
        }

        filterTexts[filterIndex].color = FilterSelected;
        filterTexts[filterIndex].fontStyle = FontStyles.Underline;

        FilterList();
    }


    public virtual void FilterList()
    {

    }


    public virtual void Open ()
    {
        //objects = RiskAssesmentManager.GetRisksAssesed();
        //int total = RiskAssesmentManager.GetTotalNonBonusRiskCount();

        Debug.Log("PARENT OPEN");

        template.gameObject.SetActive(false);

        // Cleanup
        foreach (var formEntry in formEntries)
        {
            Destroy(formEntry.gameObject);
        }
        formEntries.Clear();

        // Create panel
        int typeCorrect = 0;
        int responseCorrect = 0;
        int typeAndResponseCorrect = 0;

        foreach(ReportData rd in reportData)
        {
            GameObject formEntry = Instantiate(template, parent).gameObject;

            formEntry.GetComponent<ReportItem>().Init(rd);

           

            formEntry.gameObject.SetActive(true);

           

            //label1.fontStyle = obj.IsTypeCorrect ? FontStyles.Normal : FontStyles.Strikethrough;
            //label2.fontStyle = obj.IsResponseCorrect ? FontStyles.Normal : FontStyles.Strikethrough;
            
            //icon1.color = label1.color;
            //icon2.color = label2.color;
            

            /*
            var risk = obj;

            button0.onClick.AddListener(() =>
            {
                targetPlayer.Context.OpenContext(risk.riskObject);
                mainPanel.OpenPanel(9);
            });
            button1.onClick.AddListener(() => {
                ShowType(label1.transform, risk);
            });
            button2.onClick.AddListener(() => {
                ShowResponse(label2.transform, risk);
            });

            if (obj.IsTypeCorrect) typeCorrect++;
            if(obj.IsResponseCorrect) responseCorrect++;
            if(obj.IsTypeCorrect && obj.IsResponseCorrect) typeAndResponseCorrect++;
            formEntries.Add(formEntry);
            */
        }


        transform.GetChild(0).gameObject.SetActive(true);

        // Set score
        /*
        scoresBoth.SetText($"{objects.Count}/{total}");
        scoresType.SetText($"{typeCorrect}/{total}");
        scoresResponse.SetText($"{responseCorrect}/{total}");
        */
    }

    private void Update()
    {
        if (infoTranform.gameObject.activeInHierarchy && infoTarget)
        {
            infoTranform.position = infoTarget.position + Vector3.up * 0.01f;
        }
    }



    public void AddItem(ReportData data)
    {

        reportData.Add(data);


    }

    /*
    public void ShowType (Transform label, RiskObjectAssesment assesment)
    {
    infoTranform.gameObject.SetActive(true);
    infoTranform.position = label.transform.position + Vector3.up * 0.01f;
    if(assesment.IsTypeCorrect)
    {
        infoText.SetText(RiskAsset.correctRiskType[assesment.riskObject.Asset.Type]);
    }
    else
    {
        infoText.SetText(RiskAsset.incorrectRiskType[assesment.riskObject.Asset.Type]);
    }
    LayoutRebuilder.ForceRebuildLayoutImmediate(infoText.transform.parent.GetComponent<RectTransform>()); // Fixes an awful unity bug of container not fitting content
}

public void ShowResponse (Transform label, RiskObjectAssesment assesment)
{
    infoTranform.gameObject.SetActive(true);
    infoTranform.position = label.transform.position + Vector3.up * 0.01f;
    if (assesment.IsResponseCorrect)
    {
        infoText.SetText(RiskAsset.correctResponseType[assesment.riskObject.Asset.Response]);
    }
    else
    {
        infoText.SetText(RiskAsset.incorrectResponseType[assesment.riskObject.Asset.Response]);
    }
    LayoutRebuilder.ForceRebuildLayoutImmediate(infoText.transform.parent.GetComponent<RectTransform>()); // Fixes an awful unity bug of container not fitting content
}
*/


    public void ShowInfo(Transform obj, string text)
    {
        infoTranform.gameObject.SetActive(true);
        infoTarget = obj;
        
        infoText.SetText(text);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(infoText.transform.parent.GetComponent<RectTransform>()); // Fixes an awful unity bug of container not fitting content
    }

    public void HideInfo ()
    {
        infoTranform.gameObject.SetActive(false);
    }

}

[System.Serializable]
public class ReportData
{

    public string TaskName;
    public bool Evaluation;

    public Dictionary<string, object> kvp = new Dictionary<string, object>();

    public ReportData()
    {

    }

    public ReportData(string taskName, bool eval)
    {
        TaskName = taskName;
        

        Evaluation = eval;
    }


    public ReportData(string taskName, bool eval, Dictionary<string, object> d)
    {
        TaskName = taskName;
        kvp = d;

        Evaluation = eval;
    }
}
