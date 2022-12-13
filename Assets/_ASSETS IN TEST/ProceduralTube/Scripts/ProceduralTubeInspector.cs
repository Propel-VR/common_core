#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MassiveProceduralTube {
    // Visualize and control the handles in the Unity Editor
    [CustomEditor(typeof(ProceduralTube))]
    public class ProceduralTubeInspector :Editor {

        private ProceduralTube tube;

        // To get the tube object's transfom
        private Transform tubeTransform;
        private Quaternion tubeRotation;

        private const float HANDLE_SIZE = 0.04f;
        private const float PICK_SIZE = 0.06f;

        // To control selected handle(index)
        private int selectedIndex = -1;

        private void OnSceneGUI() {
            tube = target as ProceduralTube;
            tubeTransform = tube.transform;
            tubeRotation = Tools.pivotRotation == PivotRotation.Local ?
                tubeTransform.rotation : Quaternion.identity;

            Handles.color = Color.white;
            Vector3 p0 = HandleControl(0);
            Vector3 p1 = HandleControl(1);
            Vector3 p2 = HandleControl(2);
            Vector3 p3 = HandleControl(3);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            // Unity's API (curveStart, curveEnd, handle1, handle2, color, ?, ?)
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
        }

        #region Accessories
        private Vector3 GetHandlePositionWithIndex(int idx) {
            Vector3 position = Vector3.zero;
            switch(idx) {
                case 0:
                    position = tube.handle.p0;
                    break;
                case 1:
                    position = tube.handle.p1;
                    break;
                case 2:
                    position = tube.handle.p2;
                    break;
                case 3:
                    position = tube.handle.p3;
                    break;
            }
            return position;
        }
        private void SetHandlePositionWithIndex(int idx, Vector3 _pos) {
            switch(idx) {
                case 0:
                    tube.handle.p0 = _pos;
                    break;
                case 1:
                    tube.handle.p1 = _pos;
                    break;
                case 2:
                    tube.handle.p2 = _pos;
                    break;
                case 3:
                    tube.handle.p3 = _pos;
                    break;
            }
        }
        #endregion

        private Vector3 HandleControl(int idx) {
            Vector3 handle = tubeTransform.TransformPoint(GetHandlePositionWithIndex(idx));

            // Show Handle
            float size = HandleUtility.GetHandleSize(handle); // to fix to the screen size
            if(Handles.Button(handle, tubeRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                selectedIndex = idx;
            }

            // Record Undo and Update tubePosition into the handle[i]
            if(selectedIndex == idx) {
                EditorGUI.BeginChangeCheck();
                handle = Handles.DoPositionHandle(handle, tubeRotation);
                if(EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(tube, "Move Handle");
                    EditorUtility.SetDirty(tube);
                    SetHandlePositionWithIndex(idx, tubeTransform.InverseTransformPoint(handle));
                    tube.HandleChanged(idx);
                }
            }
            return handle;
        }
        
    } // class
}  // namespace

#endif
