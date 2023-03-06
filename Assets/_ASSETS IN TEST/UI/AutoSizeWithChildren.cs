using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoSizeWithChildren : MonoBehaviour
{
    int lastChildCount;
    RectTransform rt;

    private void Awake()
    {
        rt= GetComponent<RectTransform>();
    }

    private void Start()
    {
        lastChildCount=transform.childCount;
    }



    private void Update()
    {
        if(lastChildCount!=transform.childCount)
        {
            float newHeight=0;

            foreach(Transform child in transform.GetChild(lastChildCount))
            {
                if (child.gameObject.activeSelf)
                {
                    newHeight += child.GetComponent<RectTransform>().rect.height;
                }
            }

            rt.rect.SetHeight(newHeight);

            lastChildCount=transform.childCount;
        }
    }
}
