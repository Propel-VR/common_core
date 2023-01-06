using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ConfigurableJoint))]
public class ConfigurableJointEvents : MonoBehaviour
{
    [SerializeField]
    private float jointEventLimit;
    private ConfigurableJoint joint;

    public UnityEvent OnJointEventLimit;


    private void Awake()
    {
        joint= GetComponent<ConfigurableJoint>();
    }


    private void Update()
    {
        if(jointEventLimit > 0 && joint.currentForce.magnitude > jointEventLimit)
        {
            OnJointEventLimit?.Invoke();
        }
    }

}
