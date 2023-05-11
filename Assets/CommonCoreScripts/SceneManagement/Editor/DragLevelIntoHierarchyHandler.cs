using UnityEditor;
using UnityEngine;
using CommonCoreScripts.SceneManagement;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class DragLevelIntoHierarchyHandler : Editor
{
    static DragLevelIntoHierarchyHandler()
    {
        // Adds a callback for when the hierarchy window processes GUI events
        // for every GameObject in the heirarchy.
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
    }

    private static void HierarchyWindowItemCallback(int pID, Rect pRect)
    {
        // handle cursor change when dragging level asset
        if (Event.current.type == EventType.DragUpdated)
        {
            // begin processing drag event
            DragAndDrop.AcceptDrag();

            // run through each object that was dragged in.
            foreach (var objectRef in DragAndDrop.objectReferences)
            {
                if (objectRef is Level)
                {
                    // set cursor type
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    // make sure this call is the only one that processes the event.
                    Event.current.Use();

                    break;
                }
            }
        }

        // handle drop level asset
        if (Event.current.type == EventType.DragExited)
        {

            // run through each object that was dragged in.
            foreach (var objectRef in DragAndDrop.objectReferences)
            {
                if (objectRef is Level)
                {
                    // begin processing drag event
                    DragAndDrop.AcceptDrag();

                    var level = objectRef as Level;

                    EditorSceneManager.OpenScene(level.activeScene.path);

                    foreach (SceneRef scene in level.scenes)
                        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);

                    // make sure this call is the only one that processes the event.
                    Event.current.Use();

                    break;
                }
            }
        }
    }
}
