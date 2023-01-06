using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(SimpleCable))]
public class LinkSegment : MonoBehaviour
{

    [SerializeField]
    List<Hand> _hands;

    Rigidbody _body;
    ConfigurableJoint _joint;
    [SerializeField]
    SimpleCable _simpcab;

    float breakAt=1000;

    [SerializeField]
    Transform _bottomTransform, _topTransform;

    //public Transform topPoint, bottomPoint;

    private void Awake()
    {
        _body= GetComponent<Rigidbody>();
        _joint= GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        DisplayHighestValue.Instance.AddValue((int) _joint.currentForce.magnitude);

    }

    private void FixedUpdate()
    {
        if (_joint.currentForce.magnitude >= breakAt)
        {

            foreach(Hand h in _hands) { h.ForceReleaseGrab(); }


        }

    }

    public void SetUpVisuals(Transform other)
    {

        _simpcab.SetSettings(other, this._bottomTransform, this.transform, this._bottomTransform);

    }

    /*
     *
    public void SetupSegment(float size, Rigidbody connectedBody)
    {

        transform.localScale = new Vector3(transform.localScale.x, size, transform.localScale.z);

        if (connectedBody)
        {
            _joint.connectedBody = connectedBody;

            
            _joint.angularXMotion = ConfigurableJointMotion.Locked;
            _joint.angularYMotion = ConfigurableJointMotion.Locked;
            _joint.angularZMotion = ConfigurableJointMotion.Locked;

            StartCoroutine(UnlockJoints());
            
        }
    }

    public void SetupAsFirstSegment(float size, Rigidbody connectedBody)
    {

        transform.localScale = new Vector3(transform.localScale.x, size, transform.localScale.z);

        if (connectedBody)
        {
            _joint.connectedBody = connectedBody;
            _joint.connectedAnchor = Vector3.zero;
            _joint.angularXMotion = ConfigurableJointMotion.Locked;
            _joint.angularYMotion = ConfigurableJointMotion.Locked;
            _joint.angularZMotion = ConfigurableJointMotion.Locked;

            //connectedBody.transform.localRotation = Quaternion.Euler(Vector3.up * -90);

            //StartCoroutine(UnlockJoints());
        }
    }

    private IEnumerator UnlockJoints()
    {

        yield return new WaitForSeconds(0.5f);

        _joint.angularXMotion = ConfigurableJointMotion.Limited;
        _joint.angularYMotion = ConfigurableJointMotion.Limited;
        _joint.angularZMotion = ConfigurableJointMotion.Limited;
    }
    */

}
