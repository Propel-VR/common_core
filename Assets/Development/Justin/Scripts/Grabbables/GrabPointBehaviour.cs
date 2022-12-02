using Autohand;
using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrabPointBehaviour : MonoBehaviour
{
    //private PuppetMaster puppetMaster;
    //private Rigidbody rb;
    protected Grabbable grabScript;

    FixedJoint grabJoint;

    [SerializeField]
    protected Color neutralColour, focusedColour, goodColour, badColour;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    protected float breakforce;
    [Range(0f, 1f)][SerializeField]
    protected float hazardStart;
    [SerializeField]
    bool startFocused;


    protected bool focus = false;
    protected bool grabbed = false;

    protected virtual void Awake()
    {
        //puppetMaster = GetComponentInParent<PuppetMaster>();
        //rb = GetComponent<Rigidbody>();
        grabScript = GetComponent<Grabbable>();
        grabScript.jointBreakForce = breakforce;

        sprite.color = neutralColour;
    }

    private void Start()
    {
        if (startFocused)
            SetFocus();
    }

    public void HideSprite()
    {
        sprite.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleGrabForce();
    }

    protected virtual void HandleGrabForce()
    {
        if (grabbed)
        {

            if (focus)
            {
                if (grabJoint.currentForce.magnitude > (breakforce * hazardStart))
                {
                    sprite.color = badColour;
                }
                else
                {
                    sprite.color = goodColour;
                }
            }
            else
            {
                sprite.color = badColour;
            }

        }
    }

    public virtual void SetFocus()
    {
        focus = true;
        sprite.color = focusedColour;
    }

    public virtual void Unfocus()
    {
        
        focus=false;
        sprite.color = neutralColour;
    }

    public virtual void OnGrab()
    {
        grabbed = true;

        //get the hand holding the object and set its new joint
        grabJoint=AutoHandPlayerHelper.Instance.GetHandHoldingObject(grabScript).GetComponent<FixedJoint>();
    }

    public virtual void OnRelease()
    {
        grabbed = false;

        if(focus)
            sprite.color = focusedColour;
        else
            sprite.color = neutralColour;
    }



}
