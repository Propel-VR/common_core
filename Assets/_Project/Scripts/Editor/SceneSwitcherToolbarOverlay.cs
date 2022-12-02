using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class EditorSceneSwitcher
{
    public static bool AutoEnterPlaymode = false;
    public static readonly List<string> ScenePaths = new();

    public static void OpenScene (string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (AutoEnterPlaymode) EditorApplication.EnterPlaymode();
    }

    public static void LoadScenes ()
    {
        // clear scenes 
        ScenePaths.Clear();

        // find all scenes in the Assets folder
        var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

        foreach (var sceneGuid in sceneGuids)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            var sceneAsset = AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset));
            ScenePaths.Add(scenePath);
        }
    }
}

[Icon("d_SceneAsset Icon")]
[Overlay(typeof(SceneView), OverlayID, "Scene Switcher Creator Overlay")]
public class SceneSwitcherToolbarOverlay : ToolbarOverlay
{
    public const string OverlayID = "scene-switcher-overlay";

    private SceneSwitcherToolbarOverlay () : base(
        SceneDropdown.ID,
        AutoEnterPlayModeToggle.ID
    )
    {
    }

    public override void OnCreated ()
    {
        base.OnCreated();
        EditorSceneSwitcher.LoadScenes();
        EditorApplication.projectChanged += OnProjectChanged;
    }

    private void OnProjectChanged ()
    {
        // reload the scenes whenever the project has changed
        EditorSceneSwitcher.LoadScenes();
    }
}

[EditorToolbarElement(ID, typeof(SceneView))]
public class SceneDropdown : EditorToolbarDropdown
{
    public const string ID = SceneSwitcherToolbarOverlay.OverlayID + "/scene-dropdown";

    private const string Tooltip = "Switch scene.";

    public SceneDropdown ()
    {
        var content =
            EditorGUIUtility.TrTextContentWithIcon(SceneManager.GetActiveScene().name, Tooltip,
                "d_SceneAsset Icon");
        text = content.text;
        tooltip = content.tooltip;
        icon = content.image as Texture2D;

        // hacky: the text element is the second one here so we can set the padding
        //        but this is not really robust I think
        ElementAt(1).style.paddingLeft = 5;
        ElementAt(1).style.paddingRight = 5;

        clicked += ToggleDropdown;

        // keep track of playmode state changes
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
        EditorApplication.projectChanged += OnProjectChanged;
    }

    private void OnProjectChanged ()
    {
        // update the dropdown label in case that the scene was renamed
        text = SceneManager.GetActiveScene().name;
    }

    private void PlayModeStateChanged (PlayModeStateChange stateChange)
    {
        switch (stateChange)
        {
            case PlayModeStateChange.EnteredEditMode:
                SetEnabled(true);
                break;
            case PlayModeStateChange.EnteredPlayMode:
                // don't allow switching scenes while in play mode
                SetEnabled(false);
                break;
        }
    }

    private void ToggleDropdown ()
    {
        var menu = new GenericMenu();
        foreach (var scenePath in EditorSceneSwitcher.ScenePaths)
        {
            var sceneName = Path.GetFileNameWithoutExtension(scenePath);
            menu.AddItem(new GUIContent(sceneName), text == sceneName,
                () => OnDropdownItemSelected(sceneName, scenePath));
        }

        menu.DropDown(worldBound);
    }

    private void OnDropdownItemSelected (string sceneName, string scenePath)
    {
        text = sceneName;
        EditorSceneSwitcher.OpenScene(scenePath);
    }
}

[EditorToolbarElement(ID, typeof(SceneView))]
public class ReloadButton : EditorToolbarButton
{
    public const string ID = SceneSwitcherToolbarOverlay.OverlayID + "/reload-button";

    private const string Tooltip = "Reload scenes.";

    public ReloadButton ()
    {
        var content = EditorGUIUtility.TrTextContentWithIcon("", Tooltip, "d_Refresh");
        text = content.text;
        tooltip = content.tooltip;
        icon = content.image as Texture2D;

        clicked += OnClicked;
    }

    void OnClicked ()
    {
        EditorSceneSwitcher.LoadScenes();
    }
}

[EditorToolbarElement(ID, typeof(SceneView))]
public class AutoEnterPlayModeToggle : EditorToolbarToggle
{
    public const string ID = SceneSwitcherToolbarOverlay.OverlayID + "/auto-enter-playmode-toggle";

    private const string Tooltip = "Auto enter playmode.";

    public AutoEnterPlayModeToggle ()
    {
        var content = EditorGUIUtility.TrTextContentWithIcon("", Tooltip, "d_PlayButton On");
        text = content.text;
        tooltip = content.tooltip;
        icon = content.image as Texture2D;

        value = EditorSceneSwitcher.AutoEnterPlaymode;
        this.RegisterValueChangedCallback(Toggle);
    }

    void Toggle (ChangeEvent<bool> evt)
    {
        EditorSceneSwitcher.AutoEnterPlaymode = evt.newValue;
    }
}