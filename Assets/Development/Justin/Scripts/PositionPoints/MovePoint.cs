using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovePoint : MonoBehaviour
{
    [SerializeField]
    GameObject targetObject;

    public UnityEvent OnRepositionComplete;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.Equals(targetObject))
        {

            OnRepositionComplete?.Invoke();
        }
    }
}
