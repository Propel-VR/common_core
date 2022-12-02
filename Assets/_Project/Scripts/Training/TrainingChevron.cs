using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingChevron : MonoBehaviour
{
    const float floatFreq = 1f;
    const float floatMaxHeight = 0.25f;
    private Transform target;
    private Vector3 initLocalPos;

    void Start()
    {
        target = Camera.main.transform;
        initLocalPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = initLocalPos + Vector3.up * Mathf.Sin(Time.time * floatFreq * Mathf.PI) * floatMaxHeight;
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}
