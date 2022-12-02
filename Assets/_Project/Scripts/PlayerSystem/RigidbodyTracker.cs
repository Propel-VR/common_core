using UnityEngine;

public class RigidbodyTracker : MonoBehaviour
{
    public Vector3 velocity { get; private set; }
    public Vector3 angularVelocity { get; private set; }

    Vector3 lastPos;
    Quaternion lastRot;
    Transform tr;


    void Start ()
    {
        tr = transform;
        lastRot = tr.rotation;
        lastPos = tr.position;
    }

    void FixedUpdate ()
    {
        velocity = (tr.position - lastPos) / Time.deltaTime;

        Quaternion deltaRotation = tr.rotation * Quaternion.Inverse(lastRot);
        deltaRotation.ToAngleAxis(out var angle, out var axis);
        angle *= Mathf.Deg2Rad;
        angularVelocity = (1.0f / Time.deltaTime) * angle * axis;

        lastPos = tr.position;
        lastRot = tr.rotation;
    }
}
