using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EasyTransformFunctions : MonoBehaviour
{
    
    public void DoLocalTranslation(Vector3 translationAmount)
    {

        transform.localPosition += translationAmount;

    }

    public void OverideLocalZ(float newPosition)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newPosition);

    }


    public void OverrideLocalXRot(float newPosition)
    {

        Vector3 currentRot = transform.localRotation.eulerAngles;
        currentRot.x = newPosition;
        transform.localRotation = (Quaternion.Euler(currentRot));
    }

}
