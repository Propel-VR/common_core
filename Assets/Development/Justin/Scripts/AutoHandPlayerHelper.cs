using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHandPlayerHelper : MonoBehaviour
{
    
    private static AutoHandPlayerHelper instance;
    public static AutoHandPlayerHelper Instance { get { return instance; } }

    [SerializeField]
    private Hand leftHand, rightHand;


    private void Awake()
    {
        instance = this;
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
