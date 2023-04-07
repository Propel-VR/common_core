using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbox : MonoBehaviour
{

    [SerializeField]
    Transform lid;


    [SerializeField]
    float openSpeed = 15;

    [SerializeField]
    float openPos;
    float closedPos, GYR,GZR;


    private void Start()
    {
        closedPos = lid.rotation.eulerAngles.x;
        openPos = closedPos - openPos;
        GYR = lid.transform.rotation.y;
        GZR = lid.transform.rotation.z;
    }

    public void Open()
    {

        StartCoroutine(StartOpenLid());
    }

    private IEnumerator StartOpenLid()
    {

        float currentRot = closedPos;

        while (currentRot > openPos)
        {
            currentRot -= openSpeed * Time.deltaTime;
            
            if(currentRot<openPos)
                currentRot= openPos;

            lid.transform.rotation= Quaternion.Euler(currentRot, GYR, GZR);

            yield return new WaitForEndOfFrame();
        }


    }
}
