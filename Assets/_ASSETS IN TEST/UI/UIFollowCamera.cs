using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    [SerializeField] Camera _targetCamera;


    [SerializeField] bool _autoUpdate=true;
    [SerializeField] float _maxAngleBeforeSnap = 45f;
    [SerializeField] float _minAngleAfterSnap = 10f;
    [SerializeField] float _maxDistBeforeSnap = 0.25f;
    [SerializeField] float _minDistAfterSnap = 0.1f;
    [SerializeField] float _angleSmoothTime = 0.2f;
    [SerializeField] float _posSmoothTime = 0.2f;
    [SerializeField] float _hardSnapDistance = 5f;
    [SerializeField] float _castRadius = 2f;
    [SerializeField] float _castDistance = 5f;
    [SerializeField] float _fadeDistance = 0.5f;
    [SerializeField] LayerMask _collidingLayer;

    Transform _target = null;
    bool _angleSnap;
    bool _posSnap;
    float _angleVel;
    Vector3 _posVel;



    private void Start()
    {
        SnapToTarget();
    }

    private void Update()
    {
        if (_autoUpdate) OnUpdate();
    }

    public void OnUpdate()
    {
        if (!isActiveAndEnabled) return;
        if (!_target) return;
        Transform newTarget =  _target;


        // Trying to figure out how far should the UI be placed from a wall using a sphere cast
        Vector3 initPos = newTarget.position;
        Vector3 pos = initPos;
        float rotY = newTarget.eulerAngles.y;
        Vector3 dir = Quaternion.Euler(0f, rotY, 0f) * Vector3.forward;
        float opacityMul = 1f;
        if (newTarget)
        {
            if (Physics.SphereCast(new Ray(initPos, dir), _castRadius, out RaycastHit hitInfo, _castDistance, _collidingLayer.value, QueryTriggerInteraction.Ignore))
            {
                // We might be too close, fade UI out if that's the case.
                opacityMul = Mathf.Clamp01((hitInfo.distance - _castRadius) / _fadeDistance);
                pos = initPos + dir * _castDistance;
            }
            else
            {
                opacityMul = 1f;
                pos = initPos + dir * _castDistance;
            }
        }
      


        // --- Snapping rotation ---
        // Snapping is not always instantanious.
        // It will occure over multiple range until it is within a small radius
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, rotY));
        if (angleDifference > _maxAngleBeforeSnap) _angleSnap = true;
        if (_angleSnap)
        {
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotY, ref _angleVel, _angleSmoothTime);
            if (angleDifference < _minAngleAfterSnap) _angleSnap = false;
        }

        // --- Snapping position ---
        // Snapping is not always instantanious.
        // It will occure over multiple range until it is within a small radius
        float dist = Vector3.Distance(transform.position, pos);
        if (dist > _hardSnapDistance)
        {
            // Snap instantly because we are too far.
            dist = 0f;
            transform.position = initPos + dir * _castDistance;
            _posVel = Vector3.zero;
            _posSnap = false;
        }
        if (dist > _maxDistBeforeSnap) _posSnap = true;
        if (_posSnap)
        {
            // Gradually go toward target until min dist is reached
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref _posVel, _posSmoothTime);
            if (dist < _minDistAfterSnap) _posSnap = false;
        }
    }


    public void SnapToTarget()
    {
        if (!_target)
        {
            _target = _targetCamera.transform;
        }

        Transform newTarget = _target;

        transform.eulerAngles = Vector3.up * newTarget.eulerAngles.y;
        transform.position = newTarget.position;
    }


}
