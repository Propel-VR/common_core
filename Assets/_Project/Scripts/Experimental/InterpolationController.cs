using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DefaultExecutionOrder(1000)]
public class InterpolationController : MonoBehaviour
{
    private float[] _lastFixedUpdateTimes;
    private int _newTimeIndex;
    private List<InterpolatedTransform> registeredTransforms = new();

    static InterpolationController _inst;
    private static InterpolationController Instance
    {
        get
        {
            if (_inst == null)
            {
                _inst = new GameObject() { name = "InterpolationTracker" }.AddComponent<InterpolationController>();
            }
            return _inst;
        }
    }
    private static InterpolationController InstanceOrNull => _inst;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ClearStatics ()
    {
        _inst = null;
    }

    private int OldTimeIndex => (_newTimeIndex == 0 ? 1 : 0);

    public void Start ()
    {
        _inst = this;
        _lastFixedUpdateTimes = new float[2];
        _newTimeIndex = 0;

    }

    public void FixedUpdate ()
    {
        _newTimeIndex = OldTimeIndex;
        _lastFixedUpdateTimes[_newTimeIndex] = Time.fixedTime;
    }


    /// <summary>
    /// Call before culling. Applies interpolation on all transform
    /// </summary>
    public static void ApplyInterpolation () => 
        Instance.ApplyInterpolationInternal();

    /// <summary>
    /// Call after rendering. Applies interpolation on all transform
    /// </summary>
    public static void ClearInterpolation () => 
        Instance.ClearInterpolationInternal();



    private void ApplyInterpolationInternal ()
    {
        if (_lastFixedUpdateTimes == null) return;

        float newerTime = _lastFixedUpdateTimes[_newTimeIndex];
        float olderTime = _lastFixedUpdateTimes[OldTimeIndex];
        float interpFactor = 1f;

        if (newerTime != olderTime)
        {
            interpFactor = (Time.time - newerTime) / (newerTime - olderTime);
        }

        foreach (InterpolatedTransform tr in Instance.registeredTransforms)
        {
            if (tr == null) continue;
            tr.ApplyInterpolation(interpFactor);
        }
    }

    private void ClearInterpolationInternal ()
    {
        foreach (InterpolatedTransform tr in Instance.registeredTransforms)
        {
            if (tr == null) continue;
            tr.ClearInterpolation();
        }
    }
    

    /// <summary>
    /// Add a new interpolated transform to the list of interpolated transforms
    /// </summary>
    public static void RegisterTransform (InterpolatedTransform transform) =>
        Instance.registeredTransforms.Add(transform);

    /// <summary>
    /// Removes a transform from the list of interpolated tranforms
    /// </summary>
    public static void UnregisterTransform (InterpolatedTransform transform)
    {
        if (InstanceOrNull == null) return;
        InstanceOrNull.registeredTransforms.Remove(transform);
    }
}