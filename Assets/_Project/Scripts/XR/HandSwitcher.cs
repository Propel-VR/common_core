using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandPair
{
    public Transform leftHand;
    public Transform rightHand;
}

public class HandSwitcher : MonoBehaviour
{
    [SerializeField] private int defaultPair;
    [SerializeField] List<HandPair> handPairs = new();

    private void Start ()
    {
        SetPair(defaultPair);
    }

    public void SetPair (int index)
    {
        for (int i = 0; i < handPairs.Count; i++)
        {
            handPairs[i].leftHand.gameObject.SetActive(i == index);
            handPairs[i].rightHand.gameObject.SetActive(i == index);
        }
    }
}
