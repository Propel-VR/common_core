using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEditor;
using CommonCoreScripts;

namespace Josh.Dev
{

    /// <summary>
    /// Follow a specified target and rotate along with it, with optional delay.
    /// 
    /// Runs in LateUpdate, after everything else so that if we're following the
    /// camera, we update after the camera.
    /// </summary>
    [DefaultExecutionOrder(999)]
    public class FollowTarget : MonoBehaviour
    {
        #region Serialized Fields

        #region General Settings 

        [SerializeField]
        [TitleGroup("General")]
        bool _followPlayer;

        [SerializeField]
        [TitleGroup("General"), HideIf("_followPlayer")]
        Transform _target;

        [Space]
        [SerializeField]
        [Tooltip("Lock position")]
        [TitleGroup("General")]
        bool _lockPosition = false;

        [SerializeField]
        [Tooltip("Lock all rotation")]
        [TitleGroup("General")]
        bool _lockRotation = false;

        [Space]
        [SerializeField]
        [TitleGroup("General")]
        bool _smoothPosition = false;

        [SerializeField]
        [TitleGroup("General"), ShowIf("_smoothPosition"), Indent]
        float _repositionSpeed = 1f;

        [SerializeField]
        [TitleGroup("General")]
        bool _smoothRotation = false;

        [SerializeField]
        [TitleGroup("General"), ShowIf("_smoothRotation"), Indent]
        float _rotationSpeed = 1f;

        [Space]
        [SerializeField]
        [Tooltip("Add delay so that object waits for target to reach a certain distance before following.")]
        [TitleGroup("General")]
        bool _delayPosition = false;

        [SerializeField]
        [Tooltip("Distance at which to start moving toward target")]
        [TitleGroup("General"), OnValueChanged("OnValueChangedDistanceStartCutoff"), ShowIf("_delayPosition"), Indent]
        float _distanceStartCutoff = 0f;

        [SerializeField]
        [Tooltip("Distance at which to stop moving toward target")]
        [TitleGroup("General"), OnValueChanged("OnValueChangedDistanceStopCutoff"), ShowIf("_delayPosition"), Indent]
        float _distanceStopCutoff = 0f;

        [SerializeField]
        [Tooltip("Add delay so that object waits for target to reach a certain degree before following.")]
        [TitleGroup("General")]
        bool _delayRotation = false;

        [SerializeField]
        [Tooltip("Angle at which to start turning toward target")]
        [TitleGroup("General"), OnValueChanged("OnValueChangedAngleStartCutoff"), ShowIf("_delayRotation"), Indent]
        float _angleStartCutoff = 0f;

        [SerializeField]
        [Tooltip("Angle at which to stop turning toward target")]
        [TitleGroup("General"), OnValueChanged("OnValueChangedAngleStopCutoff"), ShowIf("_delayRotation"), Indent]
        float _angleStopCutoff = 0f;

        #endregion

        #region Position Settings

        [TitleGroup("Advanced")]

        [TabGroup("Advanced/Tabs", "Position Settings")]

        [SerializeField]
        [Tooltip("Lock the X axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "X Axis", Order = 1, AnimateVisibility = false)]
        bool _lockXPosition = false;

        [SerializeField]
        [Tooltip("Lock the Y axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Y Axis", Order = 2, AnimateVisibility = false)]
        bool _lockYPosition = false;

        [SerializeField]
        [Tooltip("Lock the Z axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Z Axis", Order = 3, AnimateVisibility = false)]
        bool _lockZPosition = false;

        [SerializeField]
        [Tooltip("Limit the X axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "X Axis")]
        bool _limitXPosition = false;

        [SerializeField]
        [Tooltip("Limit the Y axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Y Axis")]
        bool _limitYPosition = false;

        [SerializeField]
        [Tooltip("Limit the Z axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Z Axis")]
        bool _limitZPosition = false;

        [SerializeField]
        [Tooltip("Minimum X axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "X Axis"), ShowIf("_limitXPosition"), Indent]
        float _minXPosition;

        [SerializeField]
        [Tooltip("Maximum X axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "X Axis"), ShowIf("_limitXPosition"), Indent]
        float _maxXPosition;

        [SerializeField]
        [Tooltip("Minimum Y axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Y Axis"), ShowIf("_limitYPosition"), Indent]
        float _minYPosition;

        [SerializeField]
        [Tooltip("Maximum Y axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Y Axis"), ShowIf("_limitYPosition"), Indent]
        float _maxYPosition;

        [SerializeField]
        [Tooltip("Minimum Z axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Z Axis"), ShowIf("_limitZPosition"), Indent]
        float _minZPosition;

        [SerializeField]
        [Tooltip("Maximum Z axis position")]
        [TabGroup("Advanced/Tabs/Position Settings/Axis", "Z Axis"), ShowIf("_limitZPosition"), Indent]
        float _maxZPosition;

        [SerializeField]
        [Tooltip("Whether the position settings should refer to the local or global coordinate system")]
        [TabGroup("Advanced/Tabs", "Position Settings"), PropertyOrder(2), LabelText("Use local Axes")]
        bool _useLocalPosition = false;

        #endregion

        #region Rotation Settings

        [TabGroup("Advanced/Tabs", "Rotation Settings")]

        [SerializeField]
        [Tooltip("Lock rotation around the X axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "X Axis", Order = 1, AnimateVisibility = false)]
        bool _lockXRotation = false;

        [SerializeField]
        [Tooltip("Lock rotation around the Y axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Y Axis", Order = 2, AnimateVisibility = false)]
        bool _lockYRotation = false;

        [SerializeField]
        [Tooltip("Lock rotation around the Z axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Z Axis", Order = 3, AnimateVisibility = false)]
        bool _lockZRotation = false;

        [SerializeField]
        [Tooltip("Limit the rotation around the X axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "X Axis")]
        bool _limitXRotation = false;

        [SerializeField]
        [Tooltip("Limit the rotation around the Y axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Y Axis")]
        bool _limitYRotation = false;

        [SerializeField]
        [Tooltip("Limit the rotation around the Z axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Z Axis")]
        bool _limitZRotation = false;

        [SerializeField]
        [Tooltip("Minimum rotation around the X axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "X Axis"), ShowIf("_limitXRotation"), Indent]
        float _minXRotation;

        [SerializeField]
        [Tooltip("Maximum rotation around the X axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "X Axis"), ShowIf("_limitXRotation"), Indent]
        float _maxXRotation;

        [SerializeField]
        [Tooltip("Minimum rotation around the Y axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Y Axis"), ShowIf("_limitYRotation"), Indent]
        float _minYRotation;

        [SerializeField]
        [Tooltip("Maximum rotation around the Y axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Y Axis"), ShowIf("_limitYRotation"), Indent]
        float _maxYRotation;

        [SerializeField]
        [Tooltip("Minimum rotation around the Z axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Z Axis"), ShowIf("_limitZRotation"), Indent]
        float _minZRotation;

        [SerializeField]
        [Tooltip("Maximum rotation around the Z axis")]
        [TabGroup("Advanced/Tabs/Rotation Settings/Axis", "Z Axis"), ShowIf("_limitZRotation"), Indent]
        float _maxZRotation;

        [SerializeField]
        [Tooltip("Whether the rotation settings should refer to the local or global coordinate system")]
        [TabGroup("Advanced/Tabs", "Rotation Settings"), PropertyOrder(2), LabelText("Use local Axes")]
        bool _useLocalRotation = false;

        #endregion

        #region Other Settings

        [SerializeField]
        [Tooltip("Set to true if the turntable should already be at its destination when enabled")]
        [TitleGroup("Other Settings")]
        bool _resetOnEnable = true;

        [SerializeField]
        [Tooltip("Set to true if the turntable should only start transitioning after it has detected movement from its target")]
        [TitleGroup("Other Settings")]
        bool _resetOnFirstMovement = true;

        #endregion

        #endregion

        bool _shouldMove = false;
        bool _shouldRotate = false;

        public Transform Target { get => _target; set => _target = value; }

        void Start()
        {
            if (_followPlayer)
                StartCoroutine(FindPlayer());
        }

        private void OnEnable()
        {
            if (_followPlayer)
                StartCoroutine(FindPlayer());

            if (_resetOnEnable)
                ResetTarget();

            if (_resetOnFirstMovement)
                StartCoroutine(WaitAndReset());

            if (_followPlayer && Player.Get())
            {
                Player.Get().OnTeleport -= ResetTarget;
                Player.Get().OnTeleport += ResetTarget;
            }
        }

        private void OnDisable()
        {
            if (_followPlayer && Player.Get())
                Player.Get().OnTeleport -= ResetTarget;
        }

        void LateUpdate()
        {
            if (_target == null)
                return;

            //--- Position 
            if (!_lockPosition)
            {
                Vector3 targetPosition = GetTargetPosition();

                if (_delayPosition)
                    targetPosition = DelayPosition(targetPosition);

                if (_smoothPosition)
                    targetPosition = SmoothPosition(targetPosition);

                if (_useLocalPosition)
                    transform.localPosition = targetPosition;
                else
                    transform.position = targetPosition;
            }

            //--- Rotation
            if (!_lockRotation)
            {
                Quaternion targetRotation = GetTargetRotation();

                if (_delayRotation)
                    targetRotation = DelayRotation(targetRotation);

                if (_smoothRotation)
                    targetRotation = SmoothRotation(targetRotation);

                if (_useLocalRotation)
                    transform.localRotation = targetRotation;
                else
                    transform.rotation = targetRotation;
            }
        }

        Vector3 DelayPosition(Vector3 position)
        {
            float distance = Vector3.Distance(transform.position, position);

            if (!_shouldMove && distance >= _distanceStartCutoff)
            {
                _shouldMove = true;
            }
            else if (_shouldMove && distance <= _distanceStopCutoff)
            {
                _shouldMove = false;
            }

            // return the position that is at the "stop" cutoff value so that we stop smoothly
            if (_shouldMove)
                return Vector3.MoveTowards(position, transform.position, _distanceStopCutoff);
            else
                return transform.position;
        }

        Quaternion DelayRotation(Quaternion rotation)
        {
            float angle = Quaternion.Angle(transform.rotation, rotation);

            if (!_shouldRotate && angle >= _angleStartCutoff)
            {
                _shouldRotate = true;
            }
            else if (_shouldRotate && angle <= _angleStopCutoff)
            {
                _shouldRotate = false;
            }

            // return the rotation that is at the "stop" cutoff value so that we stop smoothly
            if (_shouldRotate)
                return Quaternion.RotateTowards(rotation, transform.rotation, _angleStopCutoff);
            else
                return transform.rotation;
        }

        Vector3 SmoothPosition(Vector3 position)
        {
            return Vector3.Lerp(transform.position, position, _repositionSpeed * Time.unscaledDeltaTime);
        }

        Quaternion SmoothRotation(Quaternion rotation)
        {
            return Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.unscaledDeltaTime);
        }

        Vector3 GetTargetPosition()
        {
            // handle null case
            if (_target == null)
            {
                if (_useLocalPosition)
                    return transform.localPosition;
                else
                    return transform.position;
            }

            // get target and current position in correct coordinate space
            Vector3 targetPos, curPosition;

            // (if parent is null and we're working in local space, then "local" space is actually world space)
            if (_useLocalPosition && transform.parent != null)
            {
                targetPos = transform.parent.InverseTransformPoint(_target.position);
                curPosition = transform.localPosition;
            }
            else
            {
                targetPos = _target.position;
                curPosition = transform.position;
            }

            // lock/limit individual axes
            if (_lockXPosition)
                targetPos.x = curPosition.x;
            else if (_limitXPosition)
                targetPos.x = Mathf.Clamp(targetPos.x, _minXPosition, _maxXPosition);

            if (_lockYPosition)
                targetPos.y = curPosition.y;
            else if (_limitYPosition)
                targetPos.y = Mathf.Clamp(targetPos.y, _minYPosition, _maxYPosition);

            if (_lockZPosition)
                targetPos.z = curPosition.z;
            else if (_limitZPosition)
                targetPos.z = Mathf.Clamp(targetPos.z, _minZPosition, _maxZPosition);

            return targetPos;
        }

        Quaternion GetTargetRotation()
        {
            // handle null case
            if (_target == null)
            {
                if (_useLocalRotation)
                    return transform.localRotation;
                else
                    return transform.rotation;
            }

            // get target and current rotatiob in correct coordinate space
            Vector3 targetEulers, curEulers;

            // (if parent is null and we're working in local space, then "local" space is actually world space)
            if (_useLocalRotation && transform.parent)
            {
                targetEulers = transform.parent.InverseTransformDirection(_target.eulerAngles);
                curEulers = transform.localEulerAngles;
            }
            else
            {
                targetEulers = _target.eulerAngles;
                curEulers = transform.eulerAngles;
            }

            // lock/limit individual axes
            if (_lockXRotation)
                targetEulers.x = curEulers.x;
            else if (_limitXRotation)
                targetEulers.x = ClampAngle(targetEulers.x, _minXRotation, _maxXRotation);

            if (_lockYRotation)
                targetEulers.y = curEulers.y;
            else if (_limitYRotation)
                targetEulers.y = ClampAngle(targetEulers.y, _minYRotation, _maxYRotation);

            if (_lockZRotation)
                targetEulers.z = curEulers.z;
            else if (_limitZRotation)
                targetEulers.z = ClampAngle(targetEulers.z, _minZRotation, _maxZRotation);

            // OLD
            //if (_allowVerticalRotation)
            //{
            //    targetRotation = Quaternion.LookRotation(_target.forward, Vector3.up);
            //}
            //else
            //{
            //    Vector3 projectedCameraForward = Vector3.ProjectOnPlane(_target.forward, Vector3.up);
            //
            //    //var angle = Vector3.Angle(projectedCameraForward, targetRotationHelper.forward);
            //
            //    targetRotation = Quaternion.LookRotation(projectedCameraForward, Vector3.up);
            //}

            return Quaternion.Euler(targetEulers);
        }

        public void ResetTarget()
        {
            if (!isActiveAndEnabled || !_target)
                return;

            if (!_lockPosition)
                transform.position = GetTargetPosition();

            if (!_lockRotation)
                transform.rotation = GetTargetRotation();
        }

        float ClampAngle(float val, float min, float max)
        {
            while (val < min)
                val += 360f;

            while (val > max)
                val -= 360f;

            if (val < min)
            {
                if (min - val < val + 360f - max)
                    return min;
                else
                    return max;
            }

            return val;
        }

        IEnumerator WaitAndReset()
        {
            while (_target == null)
                yield return null;

            Vector3 initialPosition = GetTargetPosition();
            Quaternion initialRotation = GetTargetRotation();

            while (GetTargetPosition() == initialPosition && GetTargetRotation() == initialRotation)
                yield return new WaitForEndOfFrame();

            ResetTarget();
        }

        IEnumerator FindPlayer()
        {
            if (Player.Get() == null)
                yield return new WaitUntil(() => Player.Get() != null);

            var player = Player.Get();
            _target = player.Camera.transform;
            player.OnTeleport -= ResetTarget;
            player.OnTeleport += ResetTarget;

            if (_resetOnEnable)
                ResetTarget();
        }

        #region Editor

        private void OnDrawGizmosSelected()
        {
            float lineWidth = 0.01f;

            Vector3 minCorner = Vector3.zero;
            Vector3 maxCorner = Vector3.zero;

            Vector3 targetPosition = transform.position;

            if (_useLocalPosition)
                targetPosition = transform.localPosition;

            if (!_lockXPosition && _limitXPosition)
            {
                minCorner.x = _minXPosition;
                maxCorner.x = _maxXPosition;
            }
            else
            {
                minCorner.x = targetPosition.x - lineWidth;
                maxCorner.x = targetPosition.x + lineWidth;
            }


            if (!_lockYPosition && _limitYPosition)
            {
                minCorner.y = _minYPosition;
                maxCorner.y = _maxYPosition;
            }
            else
            {
                minCorner.y = targetPosition.y - lineWidth;
                maxCorner.y = targetPosition.y + lineWidth;
            }


            if (!_lockZPosition && _limitZPosition)
            {
                minCorner.z = _minZPosition;
                maxCorner.z = _maxZPosition;
            }
            else
            {
                minCorner.z = targetPosition.z - lineWidth;
                maxCorner.z = targetPosition.z + lineWidth;
            }

            Vector3 P0, P1, P2, P3, P4, P5, P6, P7;

            if (_useLocalPosition && transform.parent)
            {
                P0 = transform.parent.TransformPoint(minCorner);
                P1 = transform.parent.TransformPoint(new Vector3(minCorner.x, minCorner.y, maxCorner.z));
                P2 = transform.parent.TransformPoint(new Vector3(minCorner.x, maxCorner.y, minCorner.z));
                P3 = transform.parent.TransformPoint(new Vector3(minCorner.x, maxCorner.y, maxCorner.z));
                P4 = transform.parent.TransformPoint(new Vector3(maxCorner.x, minCorner.y, minCorner.z));
                P5 = transform.parent.TransformPoint(new Vector3(maxCorner.x, minCorner.y, maxCorner.z));
                P6 = transform.parent.TransformPoint(new Vector3(maxCorner.x, maxCorner.y, minCorner.z));
                P7 = transform.parent.TransformPoint(maxCorner);
            }
            else
            {
                P0 = minCorner;
                P1 = new Vector3(minCorner.x, minCorner.y, maxCorner.z);
                P2 = new Vector3(minCorner.x, maxCorner.y, minCorner.z);
                P3 = new Vector3(minCorner.x, maxCorner.y, maxCorner.z);
                P4 = new Vector3(maxCorner.x, minCorner.y, minCorner.z);
                P5 = new Vector3(maxCorner.x, minCorner.y, maxCorner.z);
                P6 = new Vector3(maxCorner.x, maxCorner.y, minCorner.z);
                P7 = maxCorner;
            }

            Vector3 right = _useLocalPosition ? transform.right: Vector3.right;
            Vector3 up = _useLocalPosition ? transform.up : Vector3.up;
            Vector3 forward = _useLocalPosition ? transform.forward : Vector3.forward;

            Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            Gizmos.DrawLine(P0, P4);

            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);
            Gizmos.DrawLine(P0, P2);

            Gizmos.color = new Color(0.2f, 0.2f, 0.8f, 0.8f);
            Gizmos.DrawLine(P0, P1);

            //Handles.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            //Handles.DrawLine(P0, P, 10f);

            //Gizmos.color = new Color(0.2f, 0.2f, 0.8f, 0.5f);
            //Gizmos.DrawCube((minCorner + maxCorner) / 2, maxCorner - minCorner);
            //Gizmos.color = new Color(0.2f, 0.2f, 0.8f, 1f);
            //Gizmos.DrawWireCube((minCorner + maxCorner) / 2, maxCorner - minCorner);

            //Vector3[] points = new Vector3[8];
            //points[0] = minCorner;
            //points[1] = new Vector3(minCorner.x, minCorner.y, maxCorner.z);
            //points[2] = new Vector3(minCorner.x, maxCorner.y, maxCorner.z);
            //points[3] = new Vector3(minCorner.x, maxCorner.y, maxCorner.z);
            //points[4] = new Vector3(minCorner.x, maxCorner.y, maxCorner.z);
            //points[5] = new Vector3(minCorner.x, maxCorner.y, maxCorner.z);
            //points[6] = new Vector3(minCorner.x, maxCorner.y, maxCorner.z);
            //points[7] = new Vector3(minCorner.x, maxCorner.y, maxCorner.z);
            //
            //
            //Handles.color = new Color(0.2f, 0.2f, 0.8f, 1f);
            //Handles.DrawAAPolyLine()

        }

        [PropertySpace]
        [Button]
        void ResetToTarget()
        {
            ResetTarget();
        }

        #region Editor Value Changed Callbacks

        void OnValueChangedDistanceStartCutoff(float value)
        {
            _distanceStopCutoff = Mathf.Max(_distanceStopCutoff, value);
        }

        void OnValueChangedDistanceStopCutoff(float value)
        {
            _distanceStartCutoff = Mathf.Min(_distanceStartCutoff, value);
        }

        void OnValueChangedAngleStartCutoff(float value)
        {
            _angleStopCutoff = Mathf.Min(_angleStopCutoff, value);
        }

        void OnValueChangedAngleStopCutoff(float value)
        {
            _angleStartCutoff = Mathf.Max(_angleStartCutoff, value);
        }

        #endregion

        #endregion
    }
}
