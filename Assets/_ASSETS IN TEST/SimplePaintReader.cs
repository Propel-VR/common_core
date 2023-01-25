using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using TMPro;

[RequireComponent(typeof(P3dColorCounterText))]
public class SimplePaintReader : MonoBehaviour
{


    P3dColorCounterText colorCounter;
    [SerializeField] TextMeshProUGUI text;

    private void Awake()
    {
        colorCounter= GetComponent<P3dColorCounterText>();
    }

    private void Update()
    {
        text.text=colorCounter.ToString();
    }

}
