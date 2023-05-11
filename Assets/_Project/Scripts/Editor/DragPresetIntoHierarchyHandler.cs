using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using LM.Management;
using CommonCoreScripts.SceneManagement;


[InitializeOnLoad]
public class DragPresetIntoHierarchyHandler : Editor
{
    static DragPresetIntoHierarchyHandler()
    {
        // Adds a callback for when the hierarchy window processes GUI events
        // for every GameObject in the heirarchy.
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
    }

    private static void HierarchyWindowItemCallback(int pID, Rect pRect)
    {
        // handle cursor change when dragging preset asset
        if (Event.current.type == EventType.DragUpdated)
        {
            // run through each object that was dragged in.
            foreach (var objectRef in DragAndDrop.objectReferences)
            {
                if (objectRef is Preset)
                {
                    // begin processing drag event
                    DragAndDrop.AcceptDrag();

                    var sceneResolver = FindObjectOfType<SceneResolver>();
        
                    if (sceneResolver != null)
                    {
                        // set cursor type
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        // make sure this call is the only one that processes the event.
                        Event.current.Use();
                    }
        
                    break;
                }
            }
        }

        // handle drop preset asset
        if (Event.current.type == EventType.DragExited)
        {
            // run through each object that was dragged in.
            foreach (var objectRef in DragAndDrop.objectReferences)
            {
                if (objectRef is Preset)
                {
                    // begin processing drag event
                    DragAndDrop.AcceptDrag();

                    var sceneResolver = FindObjectOfType<SceneResolver>();

                    if (sceneResolver != null)
                    {
                        var preset = objectRef as Preset;

                        Level level = sceneResolver.ResolveConfiguration(preset.Configuration);

                        EditorSceneManager.OpenScene(level.activeScene.path);

                        foreach (SceneRef scene in level.scenes)
                            EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                    }

                    // make sure this call is the only one that processes the event.
                    Event.current.Use();

                    break;
                }
            }

        }
    }
}
