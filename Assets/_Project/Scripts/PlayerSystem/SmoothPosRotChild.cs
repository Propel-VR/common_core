using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Smooths the positions and rotation of all childs. 
/// Keeps in mind their original rotational and positional difference.
/// </summary>
[DefaultExecutionOrder(-300000)]
public class SmoothPosRotChild : MonoBehaviour
{
    [SerializeField] bool autoUpdate = true;

    [SerializeField] float rotationSmoothness = 0.1f;
    [SerializeField] float positionSmoothness = 0.1f;
    [SerializeField] Transform parent;
    [SerializeField] Transform[] targets;
    private Vector3[] rotVels;
    private Vector3[] posVels;
    private Quaternion[] rots;
    private Vector3[] positions;
    private Quaternion[] initRots;
    private Vector3[] initPositions;

    private void Start ()
    {
        posVels = new Vector3[targets.Length];
        positions = new Vector3[targets.Length];
        initPositions = new Vector3[targets.Length];
        posVels = new Vector3[targets.Length];
        rotVels = new Vector3[targets.Length];
        rots = new Quaternion[targets.Length];
        initRots = new Quaternion[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i];
            rots[i] = transform.rotation;
            initRots[i] = Quaternion.FromToRotation(transform.forward, target.forward);
            positions[i] = target.position - parent.position;
            initPositions[i] = target.localPosition;
        }
    }

    private void Update ()
    {
        if (autoUpdate) OnUpdate();
    }

    public void OnUpdate()
    {
        if (!gameObject.activeInHierarchy || !enabled) return;
        for(int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i];
            rots[i] = SmoothDampQuaternion(rots[i], transform.rotation * initRots[i], ref rotVels[i], rotationSmoothness);
            target.rotation = rots[i];
            positions[i] = Vector3.SmoothDamp(positions[i], transform.TransformPoint(initPositions[i]) - parent.position, ref posVels[i], positionSmoothness);
            target.position = positions[i] + parent.position;
        }
    }

    public void ResetPositions ()
    {
        if(rotVels == null) return;

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i];
            rotVels[i] = Vector3.zero;
            rots[i] = transform.rotation * initRots[i];
            target.rotation = rots[i];
            positions[i] = transform.TransformPoint(initPositions[i]) - parent.position;
            posVels[i] = Vector3.zero;
            target.position = positions[i] + parent.position;
        }
    }

    static Quaternion SmoothDampQuaternion (Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
          Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
          Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
          Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }
}
