using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RiskAsset", menuName = "Custom/Risk Asset")]
public class RiskAsset : ScriptableObject
{
    [field: SerializeField] public string DisplayName { private set; get; }
    [field: SerializeField] public RiskType Type { private set; get; }
    [field: SerializeField] public RiskResponse Response { private set; get; }
    [field: SerializeField, TextArea] public string FeedbackCorrectType { private set; get; }
    [field: SerializeField, TextArea] public string FeedbackIncorrectType { private set; get; }
    [field: SerializeField, TextArea] public string FeedbackCorrectResponse { private set; get; }
    [field: SerializeField, TextArea] public string FeedbackIncorrectResponse { private set; get; }
    [field: SerializeField] public bool IsBonus { private set; get; } = false;
    [field: SerializeField] public RiskTypeExclusion TypeToExcludeInChoice { private set; get; }

    public readonly static Dictionary<RiskType, string> riskTypeToString = new()
    {
        {RiskType.NotHazard, "Not a hazard"},
        {RiskType.SlipHazard, "Slipping hazard"},
        {RiskType.ElectricalHazard, "Electrical hazard"},
        {RiskType.BreakHazard, "Breakage hazard"},
        {RiskType.FireHazard, "Fire hazard"},
        {RiskType.TripHazard, "Tripping hazard"},
    };

    public readonly static Dictionary<RiskResponse, string> responseTypeToString = new()
    {
        {RiskResponse.None, "No response"},
        {RiskResponse.ReportToClientAndSupervisor, "Report to client and supervisor"},
        {RiskResponse.ReportToClientAndTakeCare, "Report to client and take care of it"},
        {RiskResponse.LeaveImmediately, "Leave immediately"}
    };

    public readonly static Dictionary<RiskType, string> correctRiskType = new()
    {
        { RiskType.NotHazard, "This is indeed not a hazard." },
        { RiskType.SlipHazard, "You are correct, this is a slipping hazard." },
        { RiskType.ElectricalHazard, "You are correct, this is a eletrical hazard" },
        { RiskType.BreakHazard, "You are correct, this is a breakage hazard" },
        { RiskType.FireHazard, "You are correct, this is a fire hazard" },
        { RiskType.TripHazard, "You are correct, this is a tripping hazard" },
    };

    public readonly static Dictionary<RiskType, string> incorrectRiskType = new()
    {
        { RiskType.NotHazard, "Hmmm. This is not a hazard." },
        { RiskType.SlipHazard, "Hmmm. This is rather a slipping hazard." },
        { RiskType.ElectricalHazard, "Hmmm. This is rather a eletrical hazard." },
        { RiskType.BreakHazard, "Hmmm. This is rather a breakage hazard." },
        { RiskType.FireHazard, "Hmmm. This is rather a fire hazard." },
        { RiskType.TripHazard, "Hmmm. This is rather a tripping hazard." },
    };

    public readonly static Dictionary<RiskResponse, string> correctResponseType = new()
    {
        { RiskResponse.None, "You are right, you don't have to do anything." },
        { RiskResponse.ReportToClientAndSupervisor, "You are correct, report to client and supervisor." },
        { RiskResponse.ReportToClientAndTakeCare, "You are correct, report to client and take care of it." },
        { RiskResponse.LeaveImmediately, "You are correct, you should leave immediately in that case." }
    };

    public readonly static Dictionary<RiskResponse, string> incorrectResponseType = new()
    {
        { RiskResponse.None, "Hmmm. You don't have to do anything in that case." },
        { RiskResponse.ReportToClientAndSupervisor, "You should rather report to the supervisor." },
        { RiskResponse.ReportToClientAndTakeCare, "You should rather take care of it now." },
        { RiskResponse.LeaveImmediately, "You should rather leave immediately." }
    };
}

public enum RiskType
{
    NotHazard = 0,
    SlipHazard = 1,
    ElectricalHazard = 2,
    BreakHazard = 3,
    FireHazard = 4,
    TripHazard = 5,
}

[System.Flags]
public enum RiskTypeExclusion
{
    SlipHazard =        1<<1,
    ElectricalHazard =  1<<2,
    BreakHazard =       1<<3,
    FireHazard =        1<<4,
    TripHazard =        1<<5,
}

public enum RiskResponse
{
    None = 0,
    ReportToClientAndSupervisor = 1,
    ReportToClientAndTakeCare = 2,
    LeaveImmediately = 3,
}
