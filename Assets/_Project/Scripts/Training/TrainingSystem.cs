using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bad system made for a single purpose, the control training.
/// </summary>
public class TrainingSystem : MonoBehaviour
{
    [SerializeField] PlayerReferences targetPlayer;
    [SerializeField] TrainingTriggerZone zoneA;
    [SerializeField] TrainingTriggerZone zoneB;
    [SerializeField] TrainingTriggerZone zoneC;
    GameObject tooltipA;
    GameObject tooltipB;
    GameObject tooltipC;
    GameObject tooltipD;

    static bool hasBeenTrained = false;
    private void Awake ()
    {
        tooltipA = targetPlayer.Tooltips[0];
        tooltipB = targetPlayer.Tooltips[1];
        tooltipC = targetPlayer.Tooltips[2];
        tooltipD = targetPlayer.Tooltips[3];

        zoneA.gameObject.SetActive(false);
        zoneB.gameObject.SetActive(false);
        zoneC.gameObject.SetActive(false);

        zoneA.SetOpen(true);
        zoneB.SetOpen(false);
        zoneC.SetOpen(false);

        tooltipA.SetActive(true);
        tooltipB.SetActive(false);
        tooltipC.SetActive(false);
        tooltipD.SetActive(false);

        if (!hasBeenTrained)
        {
            hasBeenTrained = true;
            return;
        }
        CloseTraining();
    }

    public void CloseTraining ()
    {
        zoneA.SetOpen(false);
        zoneB.SetOpen(false);
        zoneC.SetOpen(false);
        tooltipA.SetActive(false);
        tooltipB.SetActive(false);
        tooltipC.SetActive(false);
        tooltipD.SetActive(true);
    }

    public void OnZoneTrigger (int zoneId)
    {
        switch(zoneId)
        {
            case 0:
                zoneA.SetOpen(false);
                zoneB.SetOpen(true);
                zoneC.SetOpen(false);
                tooltipA.SetActive(false);
                tooltipB.SetActive(true);
                tooltipC.SetActive(false);
                break;
            case 1:
                zoneA.SetOpen(false);
                zoneB.SetOpen(false);
                zoneC.SetOpen(true);
                tooltipA.SetActive(false);
                tooltipB.SetActive(false);
                tooltipC.SetActive(true);
                break;
            case 2:
                CloseTraining();
                break;
        }
    }
}
