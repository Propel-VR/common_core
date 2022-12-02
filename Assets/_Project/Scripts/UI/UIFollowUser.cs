using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowUser : MonoBehaviour
{
    [SerializeField] PlayerReferences targetPlayer;

    [SerializeField] Canvas[] canvases;
    [SerializeField] Transform[] anchors;
    [SerializeField] MainPanelUI mainPanelUI;
    [SerializeField] SidePanelUI sidePanelUI;
    [SerializeField] bool autoUpdate;
    [SerializeField] float maxAngleBeforeSnap = 45f;
    [SerializeField] float minAngleAfterSnap = 10f;
    [SerializeField] float maxDistBeforeSnap = 0.25f;
    [SerializeField] float minDistAfterSnap = 0.1f;
    [SerializeField] float angleSmoothTime = 0.2f;
    [SerializeField] float posSmoothTime = 0.2f;
    [SerializeField] float hardSnapDistance = 5f;
    [SerializeField] float castRadius = 2f;
    [SerializeField] float castDistance = 5f;
    [SerializeField] float fadeDistance = 0.5f;
    [SerializeField] LayerMask collidingLayer;

    Transform target = null;
    bool focusOnAnchor;
    int anchor;
    bool angleSnap;
    bool posSnap;
    float angleVel;
    Vector3 posVel;
    


    private void OnEnable () => targetPlayer.Context.OnEnterContextMode += EnterContextMode;
    private void OnDisable () => targetPlayer.Context.OnEnterContextMode -= EnterContextMode;
    private void Start () { 
        foreach(var canvas in canvases)
        {
            canvas.worldCamera = targetPlayer.Camera;
        }
        SnapToTarget(); 
    }

    private void Update ()
    {
        if (autoUpdate) OnUpdate();
    }

    public void OnUpdate ()
    {
        if (!isActiveAndEnabled) return;
        if (!target) return;
        Transform newTarget = focusOnAnchor ? anchors[anchor] : target;


        // Trying to figure out how far should the UI be placed from a wall using a sphere cast
        Vector3 initPos = newTarget.position;
        Vector3 pos = initPos;
        float rotY = newTarget.eulerAngles.y;
        Vector3 dir = Quaternion.Euler(0f, rotY, 0f) * Vector3.forward;
        float opacityMul = 1f;
        if (newTarget && !focusOnAnchor)
        {
            if (Physics.SphereCast(new Ray(initPos, dir), castRadius, out RaycastHit hitInfo, castDistance, collidingLayer.value, QueryTriggerInteraction.Ignore))
            {
                // We might be too close, fade UI out if that's the case.
                opacityMul = Mathf.Clamp01((hitInfo.distance - castRadius) / fadeDistance);
                pos = initPos + dir * castDistance;
            }
            else
            {
                opacityMul = 1f;
                pos = initPos + dir * castDistance;
            }
        }
        mainPanelUI.opacityMul = opacityMul;
        sidePanelUI.opacityMul = opacityMul;


        // --- Snapping rotation ---
        // Snapping is not always instantanious.
        // It will occure over multiple range until it is within a small radius
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, rotY));
        if (angleDifference > maxAngleBeforeSnap) angleSnap = true;
        if (angleSnap)
        {
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotY, ref angleVel, angleSmoothTime);
            if (angleDifference < minAngleAfterSnap) angleSnap = false;
        }

        // --- Snapping position ---
        // Snapping is not always instantanious.
        // It will occure over multiple range until it is within a small radius
        float dist = Vector3.Distance(transform.position, pos);
        if (dist > hardSnapDistance)
        {
            // Snap instantly because we are too far.
            dist = 0f;
            transform.position = initPos + dir * castDistance;
            posVel = Vector3.zero;
            posSnap = false;
        }
        if (dist > maxDistBeforeSnap) posSnap = true;
        if (posSnap)
        {
            // Gradually go toward target until min dist is reached
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref posVel, posSmoothTime);
            if (dist < minDistAfterSnap) posSnap = false;
        }
    }


    public void RequireWidePanel () => sidePanelUI.RequireWidePanel();
    public void UnrequireWidePanel () => sidePanelUI.UnrequireWidePanel();

    public void SnapToTarget ()
    {
        if(target == null)
        {
            target = targetPlayer.Camera.transform;
        }

        Transform newTarget = focusOnAnchor ? anchors[anchor] : target;

        transform.eulerAngles = Vector3.up * newTarget.eulerAngles.y;
        transform.position = newTarget.position;
    }

    public void FocusAnchor (int index)
    {
        focusOnAnchor = true;
        RequireWidePanel();
        anchor = index;
        SnapToTarget();
    }

    public void UnfocusAnchor ()
    {
        UnrequireWidePanel();
        focusOnAnchor = false;
    }

    void EnterContextMode ()
    {
        UnfocusAnchor();
        SnapToTarget();
        RequireWidePanel();
    }

    void ExitContextMode ()
    {
        SnapToTarget();
        UnrequireWidePanel();
    }
}
