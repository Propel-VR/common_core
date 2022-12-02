using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReportUI : MonoBehaviour
{

    [Header("Ressource")]
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color rightColor = Color.white;
    [SerializeField] Color wrongColor = Color.white;
    [SerializeField] Sprite rightSprite;
    [SerializeField] Sprite wrongSprite;

    [Header("References")]
    [SerializeField] PlayerReferences targetPlayer;
    [SerializeField] MainPanelUI mainPanel;
    [SerializeField] RectTransform parent;
    [SerializeField] RectTransform template;
    [SerializeField] TextMeshProUGUI scoresBoth;
    [SerializeField] TextMeshProUGUI scoresType;
    [SerializeField] TextMeshProUGUI scoresResponse;
    [SerializeField] RectTransform infoTranform;
    [SerializeField] TextMeshProUGUI infoText;

    List<RectTransform> formEntries = new();
    List<RiskObjectAssesment> objects;

    private void Start ()
    {
        infoTranform.gameObject.SetActive(false);
    }

    public void Open ()
    {
        objects = RiskAssesmentManager.GetRisksAssesed();
        int total = RiskAssesmentManager.GetTotalNonBonusRiskCount();
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
        foreach(var obj in objects)
        {
            RectTransform formEntry = Instantiate(template, parent);
            TextMeshProUGUI label0 = formEntry.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI label1 = formEntry.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI label2 = formEntry.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            Image icon1 = formEntry.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>();
            Image icon2 = formEntry.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>();
            Button button0 = formEntry.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>();
            Button button1 = formEntry.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>();
            Button button2 = formEntry.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>();

            formEntry.gameObject.SetActive(true);
            label0.SetText(obj.riskObject.Asset.DisplayName);
            label1.SetText(RiskAsset.riskTypeToString[obj.assesedType]);
            label2.SetText(RiskAsset.responseTypeToString[obj.assesedResponse]);
            label1.color = obj.IsTypeCorrect ? rightColor : wrongColor;
            label2.color = obj.IsResponseCorrect ? rightColor : wrongColor;
            label1.fontStyle = obj.IsTypeCorrect ? FontStyles.Normal : FontStyles.Strikethrough;
            label2.fontStyle = obj.IsResponseCorrect ? FontStyles.Normal : FontStyles.Strikethrough;
            icon1.color = label1.color;
            icon2.color = label2.color;
            icon1.sprite = obj.IsTypeCorrect ? rightSprite : wrongSprite;
            icon2.sprite = obj.IsResponseCorrect ? rightSprite : wrongSprite;
            
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
        }

        // Set score
        scoresBoth.SetText($"{objects.Count}/{total}");
        scoresType.SetText($"{typeCorrect}/{total}");
        scoresResponse.SetText($"{responseCorrect}/{total}");
    }

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

    public void HideInfo ()
    {
        infoTranform.gameObject.SetActive(false);
    }
}
