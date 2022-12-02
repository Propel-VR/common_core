using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(FixedJoint))]
public class RepositionPoint : MonoBehaviour
{

    [SerializeField]
    Rigidbody targetBody;

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
            GameObject otherGO = targetBody.gameObject;

            if (other.gameObject.Equals(otherGO))
            {
                //position Correctly
                otherGO.transform.position = transform.position;
                otherGO.transform.rotation = transform.rotation;

                joint.connectedBody = targetBody;
                repoComplete = true;

                OnRepositionComplete?.Invoke();
            }
        }
    }

}
