using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoxManager : MonoBehaviour
{
    [SerializeField]
    TaskPool taskManager;

    private void Start()
    {
        StartCoroutine(StartPool());
    }

    private IEnumerator StartPool()
    {
        yield return new WaitForSeconds(1);
        taskManager.StartPool();

    }

}
