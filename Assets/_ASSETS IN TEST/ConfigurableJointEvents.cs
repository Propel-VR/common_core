using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ConfigurableJoint))]
public class ConfigurableJointEvents : MonoBehaviour
{
    [SerializeField]
    private float _jointEventLimit;
    private ConfigurableJoint _joint;

    public UnityEvent OnJointEventLimit;


    private void Awake()
    {
        _joint= GetComponent<ConfigurableJoint>();
    }


    private void Update()
    {
        if(_jointEventLimit > 0 && _joint.currentForce.magnitude > _jointEventLimit)
        {
            OnJointEventLimit?.Invoke();
        }
    }

}
