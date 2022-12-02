using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class ActivityManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] UIFollowUser ui;
    [SerializeField] Activity[] activities;
    [SerializeField] TrainingSystem trainingSystem;

    public static event System.Action<ActivityType> onActivityStart;
    public static event System.Action<ActivityType> onActivityEnd;
    private ActivityType currentActivity = ActivityType.None;
    private Dictionary<ActivityType, Activity> activityMap;

    static ActivityManager inst;
    private void Awake () { 
        inst = this; 
        activityMap = new Dictionary<ActivityType, Activity> ();
        foreach(Activity activity in activities)
        {
            activityMap.Add(activity.type, activity);
        }
    }

    private void Start ()
    {
        ui.FocusAnchor(0);
    }

    public static Activity[] GetActivities ()
    {
        if (inst == null) return null;
        return inst.activities;
    }

    public static void EndActivity ()
    {
        onActivityEnd?.Invoke(inst.activityMap[inst.currentActivity].type);
    }

    public static void SetActivity (ActivityType type, System.Action onTeleported)
    {
        if(inst == null) return;
        inst.trainingSystem.CloseTraining();
        inst.currentActivity = type;
        var activity = inst.activityMap[type];

        // Trigger activity start events
        onActivityStart?.Invoke(type);

        // Teleport player to activity start, facing the right direction
        inst.player.TeleportTo(activity.startPosition, true, () => {
            inst.player.RotateTowards(activity.startPosition + Quaternion.AngleAxis(activity.startAngleY, Vector3.up) * Vector3.forward);
            // Unfocus anchor to follow player
            inst.ui.UnfocusAnchor();
            onTeleported?.Invoke();

            // Teleport to a different scene if needed
            string targetScene = activity.targetSceneName;
            Debug.Log($"Teleporting player to scene \"{targetScene}\".");
            if (targetScene != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
            }
        });
    }

    public static ActivityType CurrentActivityType
    {
        get 
        {
            if (inst == null) return ActivityType.None;
            return inst.currentActivity;
        }
    }
}
