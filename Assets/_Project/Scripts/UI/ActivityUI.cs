using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityUI : MonoBehaviour
{
    [SerializeField] PlayerReferences targetPlayer;

    [SerializeField] MainPanelUI mainPanelUI;
    [SerializeField] GameObject sidePanelObject;
    [SerializeField] SidePanelUI sidePanelUI;

    [Header("References")]
    [SerializeField] RectTransform activityPanel;
    [SerializeField] Button activityTemplate;
    [SerializeField] RawImage patientPicture;
    [SerializeField] AspectRatioFitter patientPictureRatio;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI ageGenderText;
    [SerializeField] TextMeshProUGUI detailText;
    [SerializeField] TextMeshProUGUI beginButtonText;
    [SerializeField] Button beginButton;
    [SerializeField] GameObject scrollIndicator;

    private List<Image> selectionImages;
    private ActivityType selectedType;
    private Activity[] activities;

    private void Start ()
    {
        activities = ActivityManager.GetActivities();
        selectionImages = new List<Image>();

        int index = 0;
        foreach(Activity activity in activities)
        {
            Button newActivityButton = Instantiate(activityTemplate, activityPanel);
            newActivityButton.gameObject.SetActive(true);
            int i = index;
            newActivityButton.onClick.AddListener(() =>
            {
                SelectActivity(i);
            });
            index++;
            newActivityButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(activity.name);
            selectionImages.Add(newActivityButton.transform.GetChild(0).GetComponentInChildren<Image>());
        }

        SelectActivity(0);
        SetBeginButtonActive(false);
    }


    public void SelectActivity (int index)
    {
        var activity = activities[index];
        selectedType = activity.type;
        for (int i = 0; i < selectionImages.Count; i++)
        {
            selectionImages[i].enabled = i == index;
        }

        descriptionText.SetText(activity.description);
        nameText.SetText(activity.patientName);
        ageGenderText.SetText(activity.patientAgeAndGender);
        StringBuilder sb = new();
        int detailIndex = 0;
        foreach(PatientDetail detail in activity.patientDetails)
        {
            sb.Append("<b>");
            sb.Append(detail.title);
            sb.Append("</b>: ");
            sb.Append(detail.description);
            if(detailIndex != activities.Length - 1)
            {
                sb.Append("\n\n\n");
            }
            detailIndex++;
        }
        if(!string.IsNullOrWhiteSpace(activity.footnote))
        {
            sb.Append("\n\n\n");
            sb.Append(activity.footnote);
        }
        patientPicture.texture = activity.patientPicture;
        patientPictureRatio.aspectRatio = activity.patientPictureRatio;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void OnBegin ()
    {
        mainPanelUI.ClosePanel();
        ActivityManager.SetActivity(selectedType, onTeleported: () =>
        {
            sidePanelObject.SetActive(true);
            sidePanelUI.StartOfActivitySidePanel();
            targetPlayer.HandSwitcher.SetPair(1);
        });
    }


    public void OnScrollUpdate (float value)
    {
        if(value <= 0.1f)
        {
            SetBeginButtonActive(true);
            scrollIndicator.SetActive(false);
            targetPlayer.Tooltips[3].SetActive(false);
        }
    }

    public void SetBeginButtonActive (bool isActive)
    {
        beginButtonText.color = isActive ? Color.white : new Color(0.5562281f, 0.3199092f, 0.745283f, 1f);
        beginButton.interactable = isActive;
    }
}
