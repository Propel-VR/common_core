using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class BoneLinkSegment : MonoBehaviour
{


    Rigidbody _body;
    ConfigurableJoint _joint;
    [SerializeField]
    SimpleCable _simpcab;

    float breakAt = 1000;

    [SerializeField]
    Transform _bottomTransform, _topTransform;

    //public Transform topPoint, bottomPoint;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _joint = GetComponent<ConfigurableJoint>();



    }

    private void FixedUpdate()
    {
        if (_joint.currentForce.magnitude >= breakAt)
        {
            //break grab if too much tension occurs
            AutoHandPlayerHelper.Instance.ForceHandsRelease();


        }

    }

}
