using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{

    [SerializeField]
    float time;


    private void OnEnable()
    {
        StartCoroutine(StartDisable());
    }

    private IEnumerator StartDisable()
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }

}
