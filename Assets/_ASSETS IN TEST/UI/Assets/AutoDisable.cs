using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoDisable : MonoBehaviour
{

    [SerializeField]
    float time;
    [SerializeField] UnityEvent onDisable;


    private void OnEnable()
    {
        StartCoroutine(StartDisable());
    }

    private IEnumerator StartDisable()
    {
        yield return new WaitForSeconds(time);


        gameObject.SetActive(false);
        onDisable?.Invoke(); // mike added events
    }

}
