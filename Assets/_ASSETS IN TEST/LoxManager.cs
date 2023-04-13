using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoxManager : MonoBehaviour
{
    [SerializeField]
    TaskPool _taskManager;

    public ModuleMode _moduleMode;

    private void Start()
    {
        StartCoroutine(StartPool());
    }

    private IEnumerator StartPool()
    {
        yield return new WaitForSeconds(1);
        _taskManager.StartPool();

    }

    public enum ModuleMode
    {

        Guided,
        UnGuided
    }

}
