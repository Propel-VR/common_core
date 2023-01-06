using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHandPlayerHelper : MonoBehaviour
{
    
    
    public static AutoHandPlayerHelper Instance { get; private set; }

    [SerializeField]
    private Hand leftHand, rightHand;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        Instance = this;
    }


    public void ForceHandsRelease()
    {
        leftHand.ForceReleaseGrab();
        rightHand.ForceReleaseGrab();
    }

    public Hand GetHandHoldingObject(Grabbable _grabbedObject)
    {
        if (leftHand.GetHeldGrabbable() == _grabbedObject)
        {
            return leftHand;
        }
        else if(rightHand.GetHeldGrabbable() == _grabbedObject)
        {
            return rightHand;
        }
        else
        {
            Debug.LogError("NO HAND IS HOLDING THIS OBJECT");
            return null;
        }
    }

    public bool isBothHandGrab(Grabbable _grabbedObject)
    {
        if (leftHand.GetHeldGrabbable() == _grabbedObject && rightHand.GetHeldGrabbable() == _grabbedObject)
            return true;

        return false;
    }

    public Hand GetLeftHand()
    {
        return leftHand;

    }

    public Hand GetRightHand()
    {
        return rightHand;

    }
}
