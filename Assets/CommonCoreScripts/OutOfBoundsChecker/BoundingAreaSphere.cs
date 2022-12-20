using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CommonCoreScripts.OutOfBoundsChecker
{

    /// <summary>
    /// A sphere-shaped bounding area.
    /// </summary>
    public class BoundingAreaSphere : MonoBehaviour, IBoundingArea
    {
        [Header("Debug")]
        [SerializeField]
        [Tooltip("Draw debug gizmos")]
        bool _drawGizmos = false;

        [SerializeField]
        [Tooltip("Draw debug handles")]
        bool _drawHandles = false;

        public bool IsInsideArea(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);

            if (localPosition.magnitude <= 0.5f)
                return true;

            return false;
        }

        public Vector3 ClosestPoint(Vector3 position, float offset = 0f)
        {
            // Note: this does not compute the exact closest point for an ellipse - instead it
            // computes the closest point for a sphere and makes a safe guess for an ellipse
            // based on its the longest axis (the result will be less accurate depnding on how 
            // oblong the ellipse is)
            Vector3 localPosition = transform.InverseTransformPoint(position);
            Vector3 localOffset = new Vector3(offset / transform.lossyScale.x, offset / transform.lossyScale.y, offset / transform.lossyScale.z);
            float maxOffset = Mathf.Max(localOffset.x, localOffset.y, localOffset.z);
            Vector3 closestPoint = Vector3.ClampMagnitude(localPosition, 0.5f - maxOffset);
            return transform.TransformPoint(closestPoint);
        }

        #region Editor
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Sphere.fbx"), transform.position, transform.rotation, transform.lossyScale / 2f);
                Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Sphere.fbx"), transform.position, transform.rotation, -transform.lossyScale / 2f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawHandles)
            {
                Handles.Label(transform.position, "In Bounds", new GUIStyle("NotificationBackground"));
                Handles.Label(transform.TransformPoint(new Vector3(0f, 0f, -1.5f)), "Out of Bounds", new GUIStyle("NotificationBackground"));
                Handles.Label(transform.TransformPoint(new Vector3(0f, 0f, 1.5f)), "Out of Bounds", new GUIStyle("NotificationBackground"));
                Handles.Label(transform.TransformPoint(new Vector3(0f, -1.5f, 0f)), "Out of Bounds", new GUIStyle("NotificationBackground"));
                Handles.Label(transform.TransformPoint(new Vector3(0f, 1.5f, 0f)), "Out of Bounds", new GUIStyle("NotificationBackground"));
                Handles.Label(transform.TransformPoint(new Vector3(-1.5f, 0f, 0f)), "Out of Bounds", new GUIStyle("NotificationBackground"));
                Handles.Label(transform.TransformPoint(new Vector3(1.5f, 0f, 0f)), "Out of Bounds", new GUIStyle("NotificationBackground"));
            }
        }

#endif
#endregion
    }

}
