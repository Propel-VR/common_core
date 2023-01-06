using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemSocket : MonoBehaviour
{
    [SerializeField]
    Rigidbody _targetItem;

    [SerializeField]
    bool _snapInPlace;

    public UnityEvent OnItemPlaced;


    private void OnTriggerEnter(Collider other)
    {

        Rigidbody otherRB = GetComponentInParent<Rigidbody>();

        if (otherRB && otherRB.Equals(_targetItem))
        {
            OnItemPlaced?.Invoke();

            if (_snapInPlace)
            {
                _targetItem.isKinematic = true;

                try
                {
                    _targetItem.GetComponentInParent<Grabbable>().onGrab.AddListener((Hand h, Grabbable g) => OnGrabbedObject());
                }
                catch { /*NO GRABBABLE*/}
            }
        }
    }

    public void OnGrabbedObject()
    {
        _targetItem.isKinematic = false;
        _targetItem.GetComponentInParent<Grabbable>().onGrab.RemoveListener((Hand h, Grabbable g) => OnGrabbedObject());

    }

}
