using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;


namespace CommonCoreScripts.OutOfBoundsChecker
{

    /// <summary>
    /// A cube-shaped bounding area.
    /// </summary>
    public class BoundingAreaBox : SerializedMonoBehaviour, IBoundingArea
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

            if (localPosition.x >= -0.5f && localPosition.x <= 0.5f
                && localPosition.y >= -0.5f && localPosition.y <= 0.5f
                && localPosition.z >= -0.5f && localPosition.z <= 0.5f)
                return true;

            return false;
        }

        public Vector3 ClosestPoint(Vector3 position, float offset = 0f)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);

            Vector3 closestPoint = new Vector3(
                Mathf.Clamp(localPosition.x, -0.5f + offset / transform.lossyScale.x, 0.5f - offset / transform.lossyScale.x),
                Mathf.Clamp(localPosition.y, -0.5f + offset / transform.lossyScale.y, 0.5f - offset / transform.lossyScale.y),
                Mathf.Clamp(localPosition.z, -0.5f + offset / transform.lossyScale.z, 0.5f - offset / transform.lossyScale.z)
            );

            return transform.TransformPoint(closestPoint);
        }

        #region Editor
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Cube.fbx"), transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Cube.fbx"), transform.position, transform.rotation, -transform.lossyScale);
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
