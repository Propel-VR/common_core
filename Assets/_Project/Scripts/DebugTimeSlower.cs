using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimeSlower : MonoBehaviour
{
    public float timeScale = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScale;
    }
}
