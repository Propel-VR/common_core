using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderPoints : MonoBehaviour
{

    [SerializeField]
    private GameObject[] _chevrons;
    
    public void OnGrabLadder()
    {
        foreach(GameObject go in _chevrons)
            go.SetActive(true);
    }

    public void OnReleaseLadder()
    {
        foreach (GameObject go in _chevrons)
            go.SetActive(false);
    }
}
