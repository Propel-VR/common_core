using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace CommonCoreScripts.OutOfBoundsChecker
{

    /// <summary>
    /// Checks to ensure that the player is within a predefined set of bounds and
    /// teleports the player inside the bounds if found outside of them.
    /// 
    /// <see cref="IBoundingArea="/>
    /// </summary>
    public abstract class OutOfBoundsChecker : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// How often to check if the player is out of bounds
        /// </summary>
        [SerializeField]
        [Tooltip("How often to check if the player is out of bounds")]
        int _checkBoundsEveryNFrames = 1;

        /// <summary>
        /// If not checking every frame, will offset the frames checked by this amount
        /// </summary>
        [SerializeField]
        [Tooltip("If not checking every frame, will offset the frames checked by this amount")]
        int _checkFrameOffset = 0;

        /// <summary>
        /// Distance the player will be teleported inside the bounds when found outside of them (set to the player's max radius to ensure no overlap with bounds on teleport)
        /// </summary>
        [SerializeField]
        [Tooltip("Distance the player will be teleported inside the bounds when found outside of them (set to the player's max radius to ensure no overlap with bounds on teleport)")]
        float _playerTeleportOffset = 0f;

        /// <summary>
        /// Called when player is found out-of-bounds (can add teleport events here)
        /// </summary>
        [Header("Events")]
        [SerializeField]
        [Tooltip("Called when player is found out-of-bounds (can add teleport events here)")]
        public UnityEvent _onPlayerOutOfBounds;

        [Header("Debug")]

        /// <summary>
        /// Will draw a sphere at this point that is red if inside the bounds or blue if outside the bounds according to this checker
        /// </summary>
        [SerializeField]
        [Tooltip("Will draw a sphere at this point that is red if inside the bounds or blue if outside the bounds according to this checker")]
        Transform _testPoint;

        /// <summary>
        /// Draw debug gizmos
        /// </summary>
        [SerializeField]
        [Tooltip("Draw debug gizmos")]
        bool _drawGizmos;

        [SerializeField]
        [Tooltip("Log when player is found out of bounds")]
        bool _logOutOfBounds = false;

        #endregion

        #region Private Fields

        List<IBoundingArea> _bounds;

        #endregion

        /// <summary>
        /// Used to get the player's position (to be implemented per project)
        /// </summary>
        abstract public Transform GetPlayer();

        /// <summary>
        /// Checks to see if the given position is out of bounds
        /// </summary>
        public bool IsOutOfBounds(Vector3 position)
        {
            foreach (IBoundingArea boundingArea in _bounds)
            {
                if (boundingArea.IsInsideArea(position))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the given transform is out of bounds
        /// </summary>
        public bool IsOutOfBounds(Transform transform)
        {
            return IsOutOfBounds(transform.position);
        }

        /// <summary>
        /// Shorthand for <c>IsOutOfBounds(GetPlayer())</c>
        /// </summary>
        public bool IsPlayerOutOfBounds()
        {
            return IsOutOfBounds(GetPlayer());
        }

        /// <summary>
        /// Resolves an out-of-bounds transform by moving it inside the bounds, with
        /// an offset to ensure it is far enough inside the bounds.
        /// </summary>
        public void ResolveOutOfBounds(Transform transform, float offset)
        {
            if (_bounds.Count == 0)
                return;

            Vector3 outOfBoundsOffset = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (IBoundingArea boundingArea in _bounds)
            {
                Vector3 closestPoint = boundingArea.ClosestPoint(transform.position, offset);
                Vector3 closestPointOffset = transform.position - closestPoint;
                outOfBoundsOffset = closestPointOffset.sqrMagnitude < outOfBoundsOffset.sqrMagnitude ? closestPointOffset : outOfBoundsOffset;
            }

            transform.position -= outOfBoundsOffset;
        }

        #region MonoBehaviour Methods

        private void OnValidate()
        {
            _bounds = FindObjectsOfType<MonoBehaviour>().OfType<IBoundingArea>().ToList();
        }

        private void Awake()
        {
            _bounds = FindObjectsOfType<MonoBehaviour>().OfType<IBoundingArea>().ToList();
        }

        private void Update()
        {
            if ((Time.frameCount + _checkFrameOffset) % _checkBoundsEveryNFrames == 0)
            {
                Debug.Log("frame " + Time.frameCount);
                Transform player = GetPlayer();

                if (IsOutOfBounds(player))
                {
                    if (_logOutOfBounds)
                        Debug.Log($"Player out of bounds at {player.position}!", player);

                    ResolveOutOfBounds(player, _playerTeleportOffset);
                    _onPlayerOutOfBounds?.Invoke();
                }
            }
        }

        #region Editor

        private void OnDrawGizmos()
        {
            if (_drawGizmos && _testPoint)
            {
                float gizmoRadius = Mathf.Max(_playerTeleportOffset, 0.1f);

                if (IsOutOfBounds(_testPoint))
                {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawSphere(_testPoint.position, gizmoRadius);

                    Vector3 minOffset = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                    foreach (IBoundingArea boundingArea in _bounds)
                    {
                        Vector3 offset = _testPoint.position - boundingArea.ClosestPoint(_testPoint.position, _playerTeleportOffset);
                        minOffset = offset.sqrMagnitude < minOffset.sqrMagnitude ? offset : minOffset;
                    }

                    Vector3 closestPoint = _testPoint.position - minOffset;

                    Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
                    DrawDashedLine(_testPoint.position, closestPoint, minOffset.magnitude / 20f, minOffset.magnitude / 20f);

                    Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
                    Gizmos.DrawSphere(closestPoint, gizmoRadius);
                }
                else
                {
                    Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                    Gizmos.DrawSphere(_testPoint.position, gizmoRadius);
                }

            }

            void DrawDashedLine(Vector3 from, Vector3 to, float step, float gap)
            {
                Vector3 dir = (to - from).normalized;
                float len = (to - from).magnitude;
                float cur = 0f;

                while (cur < len - step)
                {
                    Gizmos.DrawLine(from + dir * cur, from + dir * (cur + step));
                    cur += step + gap;
                }
            }
        }

        #endregion

        #endregion
    }

}
