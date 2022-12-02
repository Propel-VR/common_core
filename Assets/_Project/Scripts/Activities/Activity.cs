using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivityType
{
    None,
    RiskAssesment,
    WheelchairTransfer
}

public enum SidebarProgressType
{
    ScoreOverTotal,
    ProgressPercentage
}

[System.Serializable]
public class PatientDetail
{
    public string title = "Living Situation";
    [TextArea] public string description = "Living Alone";
}



[CreateAssetMenu(fileName = "Activity", menuName = "Custom/Activity")]
public class Activity : ScriptableObject
{
    [Header("Activity UI Config")]
    new public string name;
    public ActivityType type;
    [TextArea] public string description;
    public string sidebarLabel = "Hazards identified";
    public SidebarProgressType progressType;

    [Header("Activity Config")]
    public string targetSceneName = "Main";
    public Vector3 startPosition;
    public float startAngleY;

    [Header("Patient Details")]
    public string patientName;
    public string patientAgeAndGender = "Age xx, Female/Male";
    public Texture2D patientPicture;
    public float patientPictureRatio = 1f;
    public List<PatientDetail> patientDetails;

    [TextArea] public string footnote;
}
