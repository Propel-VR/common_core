using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    GameObject _spotlightObj;

    public void OnClick()
    {
        _spotlightObj.SetActive(!_spotlightObj.activeSelf);
    }
}
