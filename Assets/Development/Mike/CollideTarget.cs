using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollideTarget : MonoBehaviour
{

    public UnityEvent onCollide;
    public GameObject targetObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == targetObject)
        {
            

            onCollide?.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
