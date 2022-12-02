using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpringJoint))]
public class SpringGrabPoint : MonoBehaviour
{
    [SerializeField]
    Material basicMaterial, warningMaterial;

    [SerializeField][Tooltip("The distance before the spring will begin ramping up its force")]
    float distanceTolerance;

    [SerializeField]
    [Tooltip("The rate at which the spring will ramp up its force")]
    float springForceAccelleration;

    [SerializeField]
    [Tooltip("The distance before it is considered bad practice")]
    float distanceHazard;

    //grab point
    Renderer renderer;
    SpringJoint spring;

    //limb
    Rigidbody connectedBody;
    Transform connectedTransform;
    float offsetDist;

    float dist;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        spring = GetComponent<SpringJoint>();
        connectedBody = spring.connectedBody;
        connectedTransform=connectedBody.transform;
        offsetDist = (transform.position - connectedTransform.position).magnitude;
    }

    private void Start()
    {
        renderer.material = basicMaterial;
    }

    private void Update()
    {
        dist = (connectedTransform.position-transform.position+(-1*offsetDist*connectedTransform.right)).magnitude;
        Debug.Log(dist);
        //keep spring and limb connected
        if(dist > distanceTolerance)
            spring.spring+=springForceAccelleration*Time.deltaTime;
        else
            spring.spring -= springForceAccelleration * Time.deltaTime;

        //hazard
        if(dist > distanceHazard)
        {
            renderer.material = warningMaterial;
        }
        else
        {
            renderer.material = basicMaterial;
        }
    }
}
