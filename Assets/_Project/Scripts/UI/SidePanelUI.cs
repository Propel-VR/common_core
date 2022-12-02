using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(1)]
public class SidePanelUI : MonoBehaviour
{
    [SerializeField] CanvasGroup parentGroup;
    [SerializeField] CanvasGroup expendGroup;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI elapsedTimeText;
    [SerializeField] Button leaveButton;
    [SerializeField] Transform expandArrow;

    [SerializeField] RectTransform toggleTransform;
    [SerializeField] RectTransform expendTransform;
    [SerializeField] float expendSpeed = 1f;
    [SerializeField] AnimationCurve expendCurve;
    [SerializeField] float lockedXPos = -3.5f;
    [SerializeField] float unlockedXPos = -2f;
    [SerializeField] float startOfActivityTimeWithPanelOpen = 1f;

    public float opacityMul = 1f;
    private Vector2 minSize;
    private Vector2 maxSize;
    private bool isExpended = false;
    private float expendValue = 0;
    private float fastExpendValue = 0;
    private int totalRiskCount;
    private Stopwatch stopwatch;
    private StringBuilder sb;
    private float wideValue;

    // 0 Is normal position, >= 1 is set aside.
    // If the panel gets locked, it get +1, if the settings are opened +1
    // If the settings are then closed -1, but it's still going to stay
    // wide until the lock disapears.
    private int wideIndent = 0; 

    private void Start ()
    {
        minSize = new Vector2(expendTransform.sizeDelta.x, toggleTransform.sizeDelta.y);
        maxSize = expendTransform.sizeDelta;
        totalRiskCount = RiskAssesmentManager.GetTotalRiskCount();

        sb = new StringBuilder();
        stopwatch = new Stopwatch();
        stopwatch.Start();

        UpdateRiskCounter(0);
    }

    private void OnEnable ()
    {
        RiskAssesmentManager.onRiskAssesed += UpdateRiskCounter;
        ActivityManager.onActivityEnd += OnActivityEnd;
    }

    private void OnDisable ()
    {
        RiskAssesmentManager.onRiskAssesed -= UpdateRiskCounter;
        ActivityManager.onActivityEnd -= OnActivityEnd;
    }

    private void Update ()
    {
        bool isVRHeadsetOn = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).isValid;
        if(stopwatch.IsRunning && !isVRHeadsetOn) stopwatch.Start();
        if(!stopwatch.IsRunning && isVRHeadsetOn) stopwatch.Stop();

        expendValue = Mathf.Clamp01(expendValue + Time.deltaTime * expendSpeed * (isExpended ? 1 : -1));
        expendGroup.alpha = (expendValue == 0) ? 0 : 1;
        fastExpendValue = Mathf.Clamp01(fastExpendValue + Time.deltaTime * expendSpeed * 4 * (isExpended ? 1 : -1));
        expendTransform.sizeDelta = Vector2.Lerp(minSize, maxSize, expendCurve.Evaluate(expendValue));

        wideValue = Mathf.Clamp01(wideValue + Time.deltaTime * expendSpeed * (wideIndent > 0 ? 1 : -1));
        transform.localPosition = new Vector3(Mathf.Lerp(unlockedXPos, lockedXPos, wideValue), 0f, transform.localPosition.z);

        expandArrow.localScale = new Vector3 (1f, Mathf.Lerp(1, -1, fastExpendValue), 1f);

        int minutes = (int)stopwatch.Elapsed.TotalMinutes;
        int seconds = (int)stopwatch.Elapsed.Seconds;
        sb.Clear();
        // Dumb but quick padding/no gc
        if(minutes < 10)
        {
            sb.Append('0');
        }
        sb.Append(minutes);
        sb.Append(':');
        if (seconds < 10)
        {
            sb.Append('0');
        }
        sb.Append(seconds);
        elapsedTimeText.SetText(sb);

        parentGroup.alpha = opacityMul;
    }

    void OnActivityEnd (ActivityType type)
    {
        if(stopwatch.IsRunning) stopwatch.Stop();
        leaveButton.interactable = false;
    }
    void UpdateRiskCounter (int currentRiskCount)
    {
        scoreText.SetText($"{currentRiskCount}/{totalRiskCount}");
    }
    
    public void ToggleExpension ()
    {
        isExpended = !isExpended;
    }

    public void RequireWidePanel ()
    {
        wideIndent++;
    }

    public void UnrequireWidePanel ()
    {
        wideIndent--;
    }

    public void StartOfActivitySidePanel ()
    {
        isExpended = true;
        StartCoroutine(StartOfActivityCoroutine());
    }

    IEnumerator StartOfActivityCoroutine ()
    {
        yield return new WaitForSeconds(startOfActivityTimeWithPanelOpen);
        isExpended = false;
    }
}
