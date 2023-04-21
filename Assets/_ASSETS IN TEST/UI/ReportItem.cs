using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportItem : MonoBehaviour
{

    [SerializeField]
    protected List<TextMeshProUGUI> labels;

    [SerializeField]
    protected List<Image> images;

    [SerializeField]
    protected Sprite passSprite, failSprite;

    public virtual void Init(ReportData rd)
    {

        labels[0].SetText(rd.TaskName);
        images[0].sprite = rd.Evaluation ? passSprite : failSprite;
        /*
        TextMeshProUGUI label0 = formEntry.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI label1 = formEntry.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI label2 = formEntry.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        Image icon1 = formEntry.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>();
        Image icon2 = formEntry.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>();
        Image icon3 = formEntry.GetChild(0).GetChild(3).GetChild(0).GetChild(1).GetComponent<Image>();
        Button button0 = formEntry.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>();
        Button button1 = formEntry.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>();
        Button button2 = formEntry.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>();
        Button button3 = formEntry.GetChild(0).GetChild(3).GetChild(0).GetComponent<Button>();

        label0.SetText(rd.TaskName);
        label1.SetText(rd.CheckReq);
        label2.SetText(rd.RectificationReq);

        label1.color = rd.CheckReq.Equals(rd.CheckDone) ? rightColor : wrongColor;
        label2.color = rd.RectificationReq.Equals(rd.RectificationDone) ? rightColor : wrongColor;


        icon1.sprite = rd.CheckReq.Equals(rd.CheckDone) ? rightSprite : wrongSprite;
        icon2.sprite = rd.RectificationReq.Equals(rd.RectificationDone) ? rightSprite : wrongSprite;
        icon3.sprite = rd.Evaluation ? passSprite : failSprite;
        */
    }

}