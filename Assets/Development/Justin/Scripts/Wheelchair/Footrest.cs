using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footrest : MonoBehaviour
{

    HingeJoint joint;
    Lever lever;
    JointHelper jointHelper;
    bool isAttatched = true;
    [SerializeField]
    bool readyToAttatch = false;

    Rigidbody rb;

    [SerializeField]
    private GameObject reattatchedFootrest;

    private void Awake()
    {
        lever = GetComponent<Lever>();
        jointHelper = GetComponent<JointHelper>();
        joint = GetComponent<HingeJoint>();
        rb=GetComponent<Rigidbody>();
    }


    public void Unlock()
    {
        Debug.Log("UNLOCKED");

        Destroy(joint);
        Destroy(lever);
        Destroy(jointHelper);
        rb.useGravity = true;

    }

    public void OnEnterTrigger()
    {
        if (!isAttatched && readyToAttatch)
        {
            isAttatched = true;
            reattatchedFootrest.SetActive(true);
            StepController.Instance.CompleteSubStep();

            gameObject.SetActive(false);

        }
    }

    public void OnExitTrigger()
    {
        if(isAttatched)
            isAttatched = false;
    }

    public void PrepForReattatch()
    {
        readyToAttatch = true;
    }
    
}
