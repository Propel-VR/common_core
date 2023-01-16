using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{


    #region public events
    public UnityEvent OnSwitchedOn, OnSwitchedOff;
    #endregion

    #region private fields
    bool _switchState =true;
    bool _left;

    Hand _currentHand= null;
    #endregion

    
    private void OnTriggerEnter(Collider other)
    {
        //Checkj for the hand componant on trigger enter
        Hand hand = other.GetComponentInParent<Hand>();

        if(hand != null )
        {
            //if found set as the current hand and store if it is the left hand
            _currentHand= hand;
            _left = _currentHand.left;
            Debug.Log("HAND FOUND. Left hand: " + _left);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        //remove a hand from current hand if it leaves
        if(_currentHand!=null && other.gameObject.Equals(_currentHand.gameObject) )
            {

            _currentHand = null;
            }
    }

    /// <summary>
    /// Called when the player squeezes the trigger
    /// </summary>
    /// <param name="left">if it was the left hand</param>
    public void Interact(bool left)
    {

        if (_currentHand!=null && left== _left)
        {
            FlipSwitch();
        }
        
    }
    /// <summary>
    /// Called when the player tries to flip the switch. determines if it can be flipped before invoking events
    /// </summary>
    public void FlipSwitch()
    {
        if (_switchState)
        {
            //turn off the switch
            OnSwitchedOff?.Invoke();
            _switchState= false;
        }
        else
        {
            //turn on the switch
            OnSwitchedOn?.Invoke();
            _switchState= true;
        }
    }

}
