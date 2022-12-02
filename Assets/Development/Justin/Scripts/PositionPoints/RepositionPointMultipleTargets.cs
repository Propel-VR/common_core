using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RepositionPointMultipleTargets : MonoBehaviour
{
    [SerializeField]
    Rigidbody[] targetBodies;

    public UnityEvent OnRepositionComplete;

    FixedJoint joint;

    bool repoComplete = false;
    private void Awake()
    {
        joint = GetComponent<FixedJoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!repoComplete)
        {
            GameObject otherGO = other.gameObject;

            if (IsATarget(otherGO))
            {
                //position Correctly
                otherGO.transform.position = transform.position;
                otherGO.transform.rotation = transform.rotation;

                joint.connectedBody = other.GetComponent<Rigidbody>();
                repoComplete = true;

                OnRepositionComplete?.Invoke();
            }
        }
    }


    private bool IsATarget(GameObject _otherGO)
    {
        foreach(Rigidbody _body in targetBodies)
        {
            if(_otherGO.Equals(_body.gameObject))
                return true;
        }

        return false;
    }
}
