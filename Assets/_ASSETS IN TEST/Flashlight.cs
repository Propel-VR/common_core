using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    GameObject spotlightObj;

    public void OnClick()
    {
        spotlightObj.SetActive(!spotlightObj.active);
    }
}
