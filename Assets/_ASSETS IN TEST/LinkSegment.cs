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
        _hands = new List<Hand>();
        
        //add all hands in scene here (should use a helper script so as not to have so many finds on awake)
        Hand[] handsInScene = FindObjectsOfType<Hand>();
        foreach(Hand h in handsInScene)
            _hands.Add(h);
    }

  

    private void FixedUpdate()
    {
        if (_joint.currentForce.magnitude >= breakAt)
        {
            //break grab if too much tension occurs
            foreach(Hand h in _hands) { h.ForceReleaseGrab(); }


        }

    }

    /// <summary>
    /// called on instantiation to setup the visuals bebtween two vables
    /// </summary>
    /// <param name="other"> previous cable </param>
    public void SetUpVisuals(Transform other)
    {
        
        _simpcab.SetSettings(other, this._bottomTransform, this.transform, this._bottomTransform);

    }

}
