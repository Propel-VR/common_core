using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// 
/// This is a class that grew too large. The panel-switching logic
/// and the Risk Assesment activity UI flow should be separated further,
/// and the panel should be opened with something other than meaningless
/// numbers (I'm not a big fan of enum but anything is better than int at 
/// this point)
/// 
/// The interput system that interupts the current UI for a certain panel
/// is universal bettween activities, it should probably be kept.
/// The feedback system can also be kept.
/// 
/// Any logic that modifies other UI element in preparation for an even
/// (E.g. updating title of notice when ending activity) should be moved
/// to the Risk Assesment-Specific system.
/// 
public class MainPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerReferences targetPlayer;
    [SerializeField] Transform parent;
    [SerializeField] CanvasGroup parentGroup;
    [SerializeField] RectTransform[] panels;
    [SerializeField] TextMeshProUGUI interuptTitle;

    [Header("Parameters")]
    [SerializeField] int firstPanel = 6;
    [SerializeField] float fadeSpeed = 4f;
    [SerializeField] float minSize = 0.8f;

    [Header("Panel References")]
    [SerializeField] SlideSelectUI hazardSelect;
    [SerializeField] SlideSelectUI responseSelect;
    // REMOVE ME IN CASE DROPDOWNS ARE VOTED OUT (Check line 154 too, and don't forget to switch which panels are shown)
    [SerializeField] TMP_Dropdown hazardDropdown;
    [SerializeField] TMP_Dropdown responseDropdown;
    // End of remove me
    [SerializeField] ReportUI reportUI;
    [SerializeField] NudgeUI nudgeUI;
    [SerializeField] TextMeshProUGUI noticeText;

    [Header("Feeback References")]
    [SerializeField] Image feebackIcon;
    [SerializeField] Sprite feebackSpriteGood;
    [SerializeField] Sprite feebackSpriteBad;
    [SerializeField] Sprite feebackSpriteHelp;
    [SerializeField] Sprite feebackSpriteInfo;
    [SerializeField] TextMeshProUGUI feedbackText;
    [SerializeField] TextMeshProUGUI feedbackTitleText;

    [Header("Player References")]
    [SerializeField] UIFollowUser uiRoot;
    [SerializeField] Transform endOfActivityPoint;
    [SerializeField] Transform endOfActivityReportPoint;


    [Header("Default Text")]
    [SerializeField] string titleGood = "Good job!";
    [SerializeField] string titleBad = "Ooops...";
    [SerializeField] string titleMistake = "One small mistake...";
    [SerializeField, TextArea] string decoyRight = "You are right, there are no risks with this object.";
    [SerializeField, TextArea] string riskUnidentified = "This object is actually a risk. Try again.";
    [SerializeField, TextArea] string noticeNoBonusFound = "Have you noticed?";
    [SerializeField, TextArea] string noticeBounusFound = "Nice work finding the exterior hazard!";

    public float opacityMul = 1f;
    float fade;
    bool isUsed;
    int nextPanel = -1;
    bool fadeIn, fadeOut;
    bool nudgeAlreadyShown = false;
    private RiskObject currentObject;

    public static bool IsUsed
    {
        get { return inst.isUsed; }
    }



    #region Monobehaviour Callbacks
    static MainPanelUI inst;
    private void Awake ()
    {
        if (inst != null)
        {
            Debug.LogError("There is already a RiskAssesmentUI in the scene");
            return;
        }
        inst = this;

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].gameObject.SetActive(false);
        }

        if (firstPanel >= 0 && firstPanel < panels.Length)
        {
            OpenPanel(firstPanel);
        }
    }

    private void OnDestroy ()
    {
        inst = null;
    }

    private void Update ()
    {
        parentGroup.interactable = !fadeOut && !fadeIn;
        parentGroup.blocksRaycasts = parentGroup.alpha >= 0.1f;
        if (fadeOut)
        {
            parentGroup.alpha = (1f - fade);
            parent.localScale = Vector3.one * Mathf.Lerp(1f, minSize, fade);

            if (fade == 1f)
            {
                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i].gameObject.SetActive(false);
                }
                fadeOut = false;

                if (nextPanel != -1)
                {
                    fadeIn = true;
                    panels[nextPanel].gameObject.SetActive(true);
                }
                else
                {
                    // Show nudge on close panel
                    if (ActivityManager.CurrentActivityType == ActivityType.RiskAssesment && !nudgeAlreadyShown)
                    {
                        nudgeUI.FadeIn();
                        nudgeAlreadyShown = true;
                    }
                }
            }

            fade = Mathf.Clamp01(fade + Time.deltaTime * fadeSpeed);
        }
        else
        if (fadeIn)
        {
            parentGroup.alpha = 1f - fade;
            parent.localScale = Vector3.one * Mathf.Lerp(1f, minSize, fade);

            if (fade == 0f)
            {
                fadeIn = false;
            }

            fade = Mathf.Clamp01(fade - Time.deltaTime * fadeSpeed);
        }else
        {
            parentGroup.alpha = 1f;
        }
        parentGroup.alpha *= opacityMul;
    }
    #endregion



    #region Risk Asses Events
    List<RiskType> hazardAnswers = new List<RiskType>();
    List<string> hazardOptions = new();
    public void OpenRiskAssesWindow (RiskObject riskObject)
    {
        currentObject = riskObject;
        fade = 1f;

        // Rebuild options
        hazardDropdown.ClearOptions();
        hazardAnswers.Clear();
        hazardOptions.Clear();
        hazardOptions.Add("Select an option");
        hazardAnswers.Add(RiskType.NotHazard);
        foreach (RiskType value in (RiskType[])Enum.GetValues(typeof(RiskType)))
        {
            if (value == 0) continue;
            if(!riskObject.Asset.TypeToExcludeInChoice.HasFlag((RiskTypeExclusion)(1 << (int)value)))
            {
                hazardOptions.Add(RiskAsset.riskTypeToString[value]);
                hazardAnswers.Add(value);
            }
        }
        hazardDropdown.AddOptions(hazardOptions);

        OpenPanel(0);
        nudgeUI.FadeOut();
    }

    public void IsHazardButton (bool isYes)
    {
        // We are identified a decoy
        if (currentObject.Asset.Type == RiskType.NotHazard)
        {
            RiskAssesmentManager.MarkRiskAsAssesed(currentObject);
            Debug.Log(RiskAssesmentManager.CanInteractWithObject(currentObject));

            // We incorrectly identified a decoy as being a hazard
            if (isYes) OpenFeeback(currentObject.Asset.FeedbackIncorrectType, titleBad, FeedbackType.Bad);

            // We correclty identified a decoy as being a hazard
            else OpenFeeback(decoyRight, titleGood, FeedbackType.Good);

            currentObject = null;
            return;
        }

        //We are identifying an object
        else
        {
            // We marked an actual risk as not one
            if (!isYes)
            {
                OpenFeeback(riskUnidentified, titleBad, FeedbackType.Bad);
                currentObject = null;
                return;
            }
        }
        OpenPanel(1);
    }

    public void ConfirmHazard ()
    {
        RiskType inputType = hazardAnswers[hazardDropdown.value];
        bool hazardCorrect = inputType == currentObject.Asset.Type;

        if (hazardCorrect) OpenFeeback(currentObject.Asset.FeedbackCorrectType, titleGood, FeedbackType.Good);
        else OpenFeeback(currentObject.Asset.FeedbackIncorrectType, titleBad, FeedbackType.Bad);
    }

    public void ConfirmResponse ()
    {
        // SWITCH COMMENTED PAIR IN CASE DROPDOWNS ARE VOTED BACK IN
        RiskType inputType = hazardAnswers[hazardDropdown.value];
        RiskResponse inputResponse = (RiskResponse)responseDropdown.value;
        hazardDropdown.SetValueWithoutNotify(0);
        responseDropdown.SetValueWithoutNotify(0);

        //RiskType inputType = (RiskType)hazardSelect.GetValue();
        //RiskResponse inputResponse = (RiskResponse)responseSelect.GetValue();
        bool responseCorrect = inputResponse == currentObject.Asset.Response;


        // We correctly identified the type of risk and its response
        if (responseCorrect) OpenFeeback(currentObject.Asset.FeedbackCorrectResponse, titleGood, FeedbackType.Good);
        else OpenFeeback(currentObject.Asset.FeedbackIncorrectResponse, titleBad, FeedbackType.Bad);
        RiskAssesmentManager.MarkRiskAsAssesed(currentObject, inputType, inputResponse);
        currentObject = null;
    }

    public void CloseFeeback ()
    {
        // We are still dealing with an object
        if (currentObject != null)
        {
            // It's a bonus, end it after the first feedback
            if (currentObject.Asset.IsBonus)
            {
                RiskAssesmentManager.MarkRiskAsAssesed(currentObject, RiskType.NotHazard, RiskResponse.None);
                currentObject = null;
                hazardDropdown.SetValueWithoutNotify(0);
                responseDropdown.SetValueWithoutNotify(0);
            }

            // Open response select
            else
            {
                OpenPanel(2);
                return;
            }
        }

        if (RiskAssesmentManager.AllRisksAssesed())
        {
            EndRiskAssesActivity();
        }
        else
        {
            ClosePanel();
        }
    }

    private void EndRiskAssesActivity ()
    {
        ActivityManager.EndActivity();
        targetPlayer.Controller.TeleportTo(endOfActivityPoint.position, true, postTeleport: () =>
        {
            targetPlayer.Controller.RotateTowards(endOfActivityReportPoint.position);
            noticeText.text = RiskAssesmentManager.AnyBonusFound() ? noticeBounusFound : noticeNoBonusFound;
            OpenPanel(5);
            uiRoot.FocusAnchor(1);
        });
    }

    public void CloseDidYouNotice ()
    {
        OpenPanel(4);
        reportUI.Open();
    }

    public void CloseReport ()
    {
        // Do something
        ClosePanel();
    }

    public void GoBackToReport ()
    {
        targetPlayer.Context.CloseContext(endOfActivityReportPoint);
        uiRoot.FocusAnchor(1);
        OpenPanel(4);
    }
    #endregion



    #region Interupts Event
    private int interuptPanel;
    private System.Action onConfirm;
    private bool isInteruptOpen;

    public void OpenSettings ()
    {
        if (isInteruptOpen) return;

        nudgeUI.FadeOut();
        isInteruptOpen = true;
        interuptPanel = nextPanel;
        OpenPanel(8);
        uiRoot.RequireWidePanel();
    }

    public void OpenInterupt (string title, System.Action onConfirm)
    {
        if (isInteruptOpen) return;

        nudgeUI.FadeOut();
        interuptTitle.text = title;
        isInteruptOpen = true;
        this.onConfirm = onConfirm;
        interuptPanel = nextPanel;
        OpenPanel(7);
    }
    public void InteruptYes ()
    {
        isInteruptOpen = false;
        if (interuptPanel == -1) ClosePanel();
        else OpenPanel(interuptPanel);

        onConfirm?.Invoke();
    }

    public void InteruptNo ()
    {
        isInteruptOpen = false;
        if (interuptPanel == -1) ClosePanel();
        else OpenPanel(interuptPanel);
    }

    public void CloseSettings () { InteruptNo(); uiRoot.UnrequireWidePanel();}


    public void OpenEndActivity ()
    {
        OpenInterupt("Do you want to end the activity?", () => EndRiskAssesActivity());
    }

    public void OpenGotoMenu ()
    {
        OpenInterupt("Do you want to go back to menu?", () =>
        {
            // Goto menu
            targetPlayer.Controller.TeleportTo(targetPlayer.Controller.GetPosition(), doFade: true, () => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
            });
        });
    }

    public void OpenQuit ()
    {
        OpenInterupt("Do you want to quit the application?", () =>
        {
            // Goto menu
            targetPlayer.Controller.TeleportTo(targetPlayer.Controller.GetPosition(), doFade: true, () => {
                Application.Quit();
            });
        });
    }

    public void OpenRestartActivity ()
    {
        OpenInterupt("Do you want to restart the activity?", () =>
        {
            // Restart activity
        });
    }
    #endregion



    #region Panel Events
    // Todo: make enum based? (But enum can't be arrange in the editor :[
    public void OpenPanel (int panelIndex)
    {
        fadeOut = true;
        nextPanel = panelIndex;
        isUsed = true;
    }

    public void ClosePanel ()
    {
        fadeOut = true;
        nextPanel = -1;
        isUsed = false;
    }

    enum FeedbackType
    {
        Good,
        Bad,
        Help,
        Info
    }

    private void OpenFeeback (string feedback, string feedbackTitle, FeedbackType feedbackType)
    {
        OpenPanel(3);
        feedbackText.text = feedback;

        // Fixing a stupid UI bug
        if(feedbackText.transform.parent.gameObject.activeSelf)
        {
            feedbackText.transform.parent.gameObject.SetActive(false);
            feedbackText.transform.parent.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(feedbackText.transform.parent.GetComponent<RectTransform>());
        } 
        else
        {
            feedbackText.transform.parent.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(feedbackText.transform.parent.GetComponent<RectTransform>());
            feedbackText.transform.parent.gameObject.SetActive(false);
        }
        
        feedbackTitleText.text = feedbackTitle;
        feebackIcon.sprite = feedbackType switch
        {
            FeedbackType.Good => feebackSpriteGood,
            FeedbackType.Bad => feebackSpriteBad,
            FeedbackType.Help => feebackSpriteHelp,
            FeedbackType.Info => feebackSpriteInfo,
            _ => null
        };
    }
    #endregion
}