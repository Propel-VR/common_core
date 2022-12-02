using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandedGrabPoint : GrabPointBehaviour
{

    [SerializeField]
    private SpriteRenderer leftSprite, rightSprite;

    FixedJoint grabJointLeft, grabJointRight;

    private bool completeSubstepOnGrab = false;
    public void CompleteSubStepOnGrab() { completeSubstepOnGrab = true; }

    bool oneHandGrab=false;

    protected override void Awake()
    {
        //puppetMaster = GetComponentInParent<PuppetMaster>();
        //rb = GetComponent<Rigidbody>();
        grabScript = GetComponent<Grabbable>();
        grabScript.jointBreakForce = breakforce;

        leftSprite.color = neutralColour;
        rightSprite.color = neutralColour;
    }

    public override void OnGrab()
    {
        if (AutoHandPlayerHelper.Instance.isBothHandGrab(grabScript))
        {
            grabbed = true;
            grabJointRight = AutoHandPlayerHelper.Instance.GetRightHand().GetComponent<FixedJoint>();
            grabJointLeft = AutoHandPlayerHelper.Instance.GetLeftHand().GetComponent<FixedJoint>();

            if (completeSubstepOnGrab)
            {
                StepController.Instance.CompleteSubStep();
                completeSubstepOnGrab=false;
            }
        }
        else
        {
            oneHandGrab = true;
        }
    }

    public override void SetFocus()
    {
        focus = true;
        leftSprite.color = focusedColour;
        rightSprite.color = focusedColour;
    }

    public override void Unfocus()
    {

        focus = false;
        leftSprite.color = neutralColour;
        rightSprite.color = neutralColour;
    }

    public override void OnRelease()
    {
        if(grabbed)
            grabbed = false;
        else
            oneHandGrab=false;

        if (focus)
        {
            leftSprite.color = focusedColour;
            rightSprite.color = focusedColour;
        }
        else
        {
            leftSprite.color = neutralColour;
            rightSprite.color = neutralColour;
        }
    }
    protected override void HandleGrabForce()
    {
        if (grabbed)
        {

            if (focus)
            {
                if (grabJointLeft.currentForce.magnitude > (breakforce * hazardStart))
                {
                    leftSprite.color = badColour;
                }
                else
                {
                    leftSprite.color = goodColour;
                }

                if (grabJointRight.currentForce.magnitude > (breakforce * hazardStart))
                {
                    rightSprite.color = badColour;
                }
                else
                {
                    rightSprite.color = goodColour;
                }
            }
            else
            {
                leftSprite.color = badColour;
                rightSprite.color = badColour;
            }

        }
        else if (oneHandGrab)
        {
            leftSprite.color = badColour;
            rightSprite.color = badColour;
        }

    }
}
