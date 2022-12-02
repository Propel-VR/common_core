using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RiskObjectAssesment
{
    public RiskObject riskObject { private set; get; }
    public RiskType assesedType { private set; get; }
    public RiskResponse assesedResponse { private set; get; }
    public bool IsTypeCorrect { private set; get; }
    public bool IsResponseCorrect { private set; get; }

    public RiskObjectAssesment (RiskObject riskObject, RiskType assesedType, RiskResponse assesedResponse)
    {
        this.riskObject=riskObject;
        this.assesedType=assesedType;
        this.assesedResponse=assesedResponse;

        IsTypeCorrect = assesedType == riskObject.Asset.Type;
        IsResponseCorrect = assesedResponse == riskObject.Asset.Response;
    }
}


[DefaultExecutionOrder(20)]
public class RiskAssesmentManager : MonoBehaviour
{
    [SerializeField] MainPanelUI riskAssesmentUI;

    List<RiskObjectAssesment> risksAssessed;
    List<RiskObject> decoyAssessed;
    List<RiskObject> riskObjects;
    List<RiskObject> decoyObjects;
    public static event Action<int> onRiskAssesed;
    
    static RiskAssesmentManager inst;
    void Awake()
    {
        if (inst != null)
        {
            Debug.LogError("There is already a RiskAssesmentUI in the scene");
            return;
        }
        inst = this;
        risksAssessed = new List<RiskObjectAssesment>();
        decoyAssessed = new List<RiskObject>();
        riskObjects = new List<RiskObject>();
        decoyObjects = new List<RiskObject>();
    }
    private void OnEnable ()
    {
        ActivityManager.onActivityStart += OnActivityStart;
    }

    private void OnDisable ()
    {
        ActivityManager.onActivityStart -= OnActivityStart;
    }

    private void OnActivityStart (ActivityType type)
    {
    }
    private void OnDestroy ()
    {
        inst = null;
        onRiskAssesed = null;
    }

    public static int GetTotalRiskCount ()
    {
        if (inst == null)
        {
            Debug.LogError("No RiskAssesmentManager present in the scene");
            return 0;
        }
        return inst.riskObjects.Count;
    }

    public static void RegisterObject (RiskObject riskObject)
    {
        if (inst == null)
        {
            Debug.LogError("No RiskAssesmentManager present in the scene");
            return;
        }
        if(riskObject.Asset.Type == RiskType.NotHazard || riskObject.Asset.IsBonus) inst.decoyObjects.Add(riskObject);
        else                                                                        inst.riskObjects.Add(riskObject);
    }

    public static bool CanInteractWithObject (RiskObject riskObject)
    {
        if (inst == null)
        {
            Debug.LogError("No RiskAssesmentManager present in the scene");
            return false;
        }
        return (inst.riskObjects.Contains(riskObject) || inst.decoyObjects.Contains(riskObject)) && !MainPanelUI.IsUsed;
    }

    public static void InteractWithObject (RiskObject riskObject)
    {
        if (inst == null)
        {
            Debug.LogError("No RiskAssesmentManager present in the scene");
            return;
        }
        if(inst.riskObjects.Contains(riskObject) || inst.decoyObjects.Contains(riskObject))
        {
            inst.riskAssesmentUI.OpenRiskAssesWindow(riskObject);
        }
    }

    public static void MarkRiskAsAssesed (RiskObject riskObject, RiskType inputType = RiskType.NotHazard, RiskResponse inputResponse = RiskResponse.None)
    {
        if (inst == null)
        {
            Debug.LogError("No RiskAssesmentManager present in the scene");
            return;
        }
        if(riskObject.Asset.IsBonus || riskObject.Asset.Type == RiskType.NotHazard)
        {
            inst.decoyAssessed.Add(riskObject);
            inst.decoyObjects.Remove(riskObject);
            riskObject.OnAssesRisk();
        } 
        else
        {
            inst.risksAssessed.Add(new RiskObjectAssesment(riskObject, inputType, inputResponse));
            inst.riskObjects.Remove(riskObject);
            riskObject.OnAssesRisk();
        }
        onRiskAssesed?.Invoke(inst.risksAssessed.Count);
    }

    public static bool AllRisksAssesed ()
    {
        if (inst == null)
        {
            return false;
        }
        return inst.riskObjects.Count == 0;
    }

    public static bool AnyBonusFound ()
    {
        if (inst == null)
        {
            return false;
        }
        foreach(var riskObject in inst.decoyAssessed)
        {
            if(riskObject.Asset.IsBonus) return true;
        }
        return false;
    }

    public static List<RiskObjectAssesment> GetRisksAssesed ()
    {
        if (inst == null)
        {
            return new List<RiskObjectAssesment>();
        }
        return inst.risksAssessed;
    }

    public static int GetTotalNonBonusRiskCount ()
    {
        if (inst == null)
        {
            return 0;
        }
        return inst.riskObjects.Count + inst.risksAssessed.Count;
    }
}
