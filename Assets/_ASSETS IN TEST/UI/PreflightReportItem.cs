using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreflightReportItem : ReportItem
{

    [SerializeField] List<Button> buttons;
    [SerializeField] Sprite rightSprite;
    [SerializeField] Sprite wrongSprite;
    [SerializeField] Color rightColor = Color.white;
    [SerializeField] Color wrongColor = Color.white;

    public void Init(PreflightReportData rd)
    {

        labels[0].SetText(rd.TaskName);
        labels[1].SetText(rd.CheckReq);
        labels[2].SetText(rd.RectificationReq);

        labels[1].color = rd.CheckReq.Equals("Complete") ? rightColor : wrongColor;
        labels[2].color = Color.white;

        images[0].sprite = rd.CheckReq.Equals(rd.CheckDone) ? rightSprite : wrongSprite;
        images[1].sprite = rd.RectificationReq.Equals(rd.RectificationDone) ? rightSprite : wrongSprite;
        images[2].sprite = rd.Evaluation ? passSprite : failSprite;

        if (rd.CheckReq.Equals(rd.CheckDone))
        {
            buttons[1].onClick.AddListener(() =>
            {
                PreflightReportUI.Instance.HideInfo();
            });
        }
        else
        {
            buttons[1].onClick.AddListener(() =>
            {

                string msg = "you initially selected " + rd.CheckDone;
                PreflightReportUI.Instance.ShowInfo(images[0].transform, msg);
            });
        }

        if (rd.RectificationReq.Equals(rd.RectificationDone))
        {

            buttons[2].onClick.AddListener(() => {

                PreflightReportUI.Instance.HideInfo();
            });

        }
        else
        {
            buttons[2].onClick.AddListener(() => {

                string msg = "you initially selected " + rd.RectificationDone;
                PreflightReportUI.Instance.ShowInfo(images[1].transform, msg);
            });
        }
        

        buttons[3].onClick.AddListener(() =>
        {

            ContextSystem.Instance.OpenContext(rd.Interactable);
        });


        buttons[0].onClick.AddListener(() =>
        {

            ContextSystem.Instance.OpenContext(rd.Interactable);
        });

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
