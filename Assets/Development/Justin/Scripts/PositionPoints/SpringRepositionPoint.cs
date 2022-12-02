using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpringJoint))]
public class SpringRepositionPoint : MonoBehaviour
{
    [SerializeField]
    Rigidbody targetBody;

    public UnityEvent OnRepositionComplete;

    SpringJoint joint;

    private void Awake()
    {
        joint = GetComponent<SpringJoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = targetBody.gameObject;

        if (other.gameObject.Equals(otherGO))
        {
            //position Correctly
            otherGO.transform.position = transform.position;
            otherGO.transform.rotation = transform.rotation;

            joint.connectedBody = targetBody;

            OnRepositionComplete?.Invoke();
        }
    }
}
