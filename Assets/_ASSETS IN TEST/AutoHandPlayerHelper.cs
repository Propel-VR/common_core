using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that allows for easy access of autohand hands through any script
/// </summary>
public class AutoHandPlayerHelper : MonoBehaviour
{
    
    
    public static AutoHandPlayerHelper Instance { get; private set; }

    [SerializeField]
    private Hand _leftHand, _rightHand;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        Instance = this;
    }

    /// <summary>
    /// forces both hands to releaser what they are holding
    /// </summary>
    public void ForceHandsRelease()
    {
        _leftHand.ForceReleaseGrab();
        _rightHand.ForceReleaseGrab();
    }

    /// <summary>
    /// gets the hand holding a specified object
    /// </summary>
    /// <param name="_grabbedObject"> the object being held </param>
    /// <returns></returns>
    public Hand GetHandHoldingObject(Grabbable _grabbedObject)
    {
        if (_leftHand.GetHeldGrabbable() == _grabbedObject)
        {
            return _leftHand;
        }
        else if(_rightHand.GetHeldGrabbable() == _grabbedObject)
        {
            return _rightHand;
        }
        else
        {
            Debug.LogError("NO HAND IS HOLDING THIS OBJECT");
            return null;
        }
    }

    /// <summary>
    /// returns true if the specified object is being grabbed with both hands
    /// </summary>
    /// <param name="_grabbedObject">object being held</param>
    /// <returns></returns>
    public bool isBothHandGrab(Grabbable _grabbedObject)
    {
        if (_leftHand.GetHeldGrabbable() == _grabbedObject && _rightHand.GetHeldGrabbable() == _grabbedObject)
            return true;

        return false;
    }

    public Hand GetLeftHand()
    {
        return _leftHand;

    }

    public Hand GetRightHand()
    {
        return _rightHand;

    }
}
