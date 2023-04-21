using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportSummary : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI module, subMod, mode, duration;


    [SerializeField] Slider[] sliders;
    [SerializeField] TextMeshProUGUI[] completions;


    public void SetChecks(int completed, int total)
    {
        float val = completed;
        val /= total;
        sliders[0].value=val;

        completions[0].text = completed.ToString() + "/" + total.ToString();
    }

    public void SetSnags(int completed, int total)
    {
        float val = completed;
        val /= total;
        sliders[1].value = val;

        completions[1].text = completed.ToString() + "/" + total.ToString();
    }

    public void SetRectifications(int completed, int total)
    {
        float val = completed;
        val /= total;
        sliders[2].value = val;

        completions[2].text = completed.ToString() + "/" + total.ToString();
    }




}
