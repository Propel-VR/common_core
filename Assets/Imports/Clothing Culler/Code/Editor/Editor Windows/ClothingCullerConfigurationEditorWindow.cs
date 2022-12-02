using System;
using System.Collections.Generic;
using Salvage.ClothingCuller.Configuration;
using Salvage.ClothingCuller.Editor.TreeViews;
using Salvage.ClothingCuller.ExtensionMethods;
using Salvage.ClothingCuller.Serialization;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Salvage.ClothingCuller.Editor.EditorWindows
{
    public class ClothingCullerConfigurationEditorWindow : EditorWindow
    {
        public ClothingCullerConfiguration Config { get; private set; }
        public ValidationResultByOccludee OccludeeValidationResults { get; private set; }
        public PrefabsTreeView PrefabsTreeView { get; private set; }

        private BaseOccludeeCategoryTreeView occludeesTreeView;
        private BaseOccludeeCategoryTreeView occludersTreeView;
        private SearchField prefabsTreeViewSearchField;
        private SearchField occludeesTreeViewSearchField;
        private SearchField occludersTreeViewSearchField;
        private bool isHelpVisible;
        private bool isViewButtonClicked;

        #region SerializeFields

        [SerializeField]
        private TreeViewState prefabsTreeViewState;

        [SerializeField]
        private TreeViewState occludeesTreeViewState;

        [SerializeField]
        private TreeViewState occludersTreeViewState;

        [SerializeField]
        private MultiColumnHeaderState prefabsMultiColumnHeaderState;

        [SerializeField]
        private MultiColumnHeaderState occludeesMultiColumnHeaderState;

        [SerializeField]
        private MultiColumnHeaderState occludersMultiColumnHeaderState;

        [SerializeField]
        private bool isPrefabValidationEnabled;

        #endregion

        #region EditorWindow Methods

        private void OnEnable()
        {
            Config = ClothingCullerConfiguration.FindOrCreate();
            OccludeeValidationResults = new ValidationResultByOccludee();
            PrefabsTreeView = new PrefabsTreeView(initTreeViewState(ref prefabsTreeViewState), initOccludeeCategoryTreeViewHeader(ref prefabsMultiColumnHeaderState), this);
            occludeesTreeView = new OccludeesTreeView(initTreeViewState(ref occludeesTreeViewState), initOccludeeCategoryTreeViewHeader(ref occludeesMultiColumnHeaderState), this);
            occludersTreeView = new OccludersTreeView(initTreeViewState(ref occludersTreeViewState), initOccludeeCategoryTreeViewHeader(ref occludersMultiColumnHeaderState), this);
            prefabsTreeViewSearchField = new SearchField();
            occludeesTreeViewSearchField = new SearchField();
            occludersTreeViewSearchField = new SearchField();

            PrefabsTreeView.SelectedOccludeeChanged += onPrefabsTreeViewSelectedOccludeeChanged;
            occludeesTreeView.SelectedOccludeeChanged += onOccludeesTreeViewSelectedOccludeeChanged;
            occludersTreeView.SelectedOccludeeChanged += onOccludersTreeViewSelectedOccludeeChanged;
        }

        [MenuItem("Window/Clothing Culler/Edit Configuration")]
        private static void show()
        {
            ClothingCullerConfigurationEditorWindow window = GetWindow<ClothingCullerConfigurationEditorWindow>("Edit Configuration");
        }

        private void OnFocus()
        {
            Reload();
        }

        #endregion

        #region OnGUI

        private void OnGUI()
        {
            drawToolbar();

            EditorGUILayout.BeginVertical();

            if (isHelpVisible)
            {
                drawHelpBox();
            }
            drawHeaderLabels();
            drawSearchFields();
            drawTreeViews();
            drawButtons();

            EditorGUILayout.EndVertical();

            if (isViewButtonClicked)
            {
                onViewButtonClicked();
            }
        }

        private void drawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Create Prefab Category", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                onCreatePrefabCategoryClicked();
            }

            EditorGUI.BeginChangeCheck();
            isPrefabValidationEnabled = GUILayout.Toggle(isPrefabValidationEnabled, "Validate Prefabs", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                onValidatePrefabsToggled();
            }

            isHelpVisible = GUILayout.Toggle(isHelpVisible, "Show Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));

            EditorGUILayout.EndHorizontal();
        }

        private void onCreatePrefabCategoryClicked()
        {
            PrefabsTreeView.CreatePrefabCategory();
        }

        private void onValidatePrefabsToggled()
        {
            if (isPrefabValidationEnabled)
            {
                validatePrefabs();
            }
            else
            {
                OccludeeValidationResults.Clear();
            }

            PrefabsTreeView.Reload();
            occludeesTreeView.Reload();
            occludersTreeView.Reload();
        }

        private void drawHeaderLabels()
        {
            var labelStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Prefabs: {PrefabsTreeView.GetOccludeeConfigurationCount()}", labelStyle);
            EditorGUILayout.LabelField($"Occludees: {occludeesTreeView.GetOccludeeConfigurationCount()}", labelStyle);
            EditorGUILayout.LabelField($"Occluders: {occludersTreeView.GetOccludeeConfigurationCount()}", labelStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void drawHelpBox()
        {
            string helpText = "Step 1: Create a Prefab Category using the create button or from the Prefabs TreeView's context menu.\n\n" +
                              "Step 2: Drag a Prefab from the Project view into a Prefab Category.\n\n" +
                              "Step 3: Select a single Prefab from the Prefabs TreeView, then drag another Prefab or Prefab Category into the Occludees or Occluders TreeView.\n\n" +
                              "Step 4: Select a Prefab from the Occludees or Occluders TreeView in order to view or edit the occlusion data.";

            EditorGUILayout.HelpBox(helpText, MessageType.Info, true);
        }

        private void drawSearchFields()
        {
            EditorGUILayout.BeginHorizontal();
            PrefabsTreeView.searchString = prefabsTreeViewSearchField.OnGUI(EditorGUILayout.GetControlRect(), PrefabsTreeView.searchString);
            occludeesTreeView.searchString = occludeesTreeViewSearchField.OnGUI(EditorGUILayout.GetControlRect(), occludeesTreeView.searchString);
            occludersTreeView.searchString = occludersTreeViewSearchField.OnGUI(EditorGUILayout.GetControlRect(), occludersTreeView.searchString);
            EditorGUILayout.EndHorizontal();
        }

        private void drawTreeViews()
        {
            EditorGUILayout.BeginHorizontal();

            PrefabsTreeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true)));
            occludeesTreeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true)));
            occludersTreeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true)));

            EditorGUILayout.EndHorizontal();
        }

        private void drawButtons()
        {
            var richTextStyle = new GUIStyle(GUI.skin.button) { richText = true };
            Color originalBackgroundColor = GUI.backgroundColor;

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(PrefabsTreeView.SelectedOccludee == null);
            if (PrefabsTreeView.IsSelectionLocked)
            {
                GUI.backgroundColor = Color.yellow;
            }
            if (GUILayout.Button(getLockButtonText(), richTextStyle, GUILayout.Width(PrefabsTreeView.GetWidth() + 2f)))
            {
                onLockButtonClicked();
            }
            GUI.backgroundColor = originalBackgroundColor;
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!shouldEnableViewButton());
            isViewButtonClicked = GUILayout.Button(getViewButtonText(), richTextStyle, GUILayout.Width(occludeesTreeView.GetWidth() + occludersTreeView.GetWidth() + 7f));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

#if !UNITY_2019_1_OR_NEWER
            GUILayout.Space(4f);
#endif
        }

        private string getLockButtonText()
        {
            string text;

            if (PrefabsTreeView.IsSelectionLocked)
            {
                text = "Unlock";
            }
            else
            {
                text = "Lock";
            }

            if (PrefabsTreeView.SelectedOccludee != null && PrefabsTreeView.SelectedOccludee.Prefab != null)
            {
                text += $" <b>{PrefabsTreeView.SelectedOccludee.Prefab.name}</b>";
            }

            return text;
        }

        private bool shouldEnableViewButton()
        {
            if (occludeesTreeView.SelectedOccludee != null && occludeesTreeView.SelectedOccludee.Prefab != null)
            {
                return !OcclusionDataViewer.IsSceneOpen || OcclusionDataViewer.Occludee != occludeesTreeView.SelectedOccludee || OcclusionDataViewer.Occluder != PrefabsTreeView.SelectedOccludee;
            }

            if (occludersTreeView.SelectedOccludee != null && occludersTreeView.SelectedOccludee.Prefab != null)
            {
                return !OcclusionDataViewer.IsSceneOpen || OcclusionDataViewer.Occludee != PrefabsTreeView.SelectedOccludee || OcclusionDataViewer.Occluder != occludersTreeView.SelectedOccludee;
            }

            return false;
        }

        private string getViewButtonText()
        {
            string text = "View";

            if (PrefabsTreeView.SelectedOccludee == null || PrefabsTreeView.SelectedOccludee.Prefab == null)
            {
                return text;
            }

            if (occludeesTreeView.SelectedOccludee != null && occludeesTreeView.SelectedOccludee.Prefab != null)
            {
                text += $" <b>{PrefabsTreeView.SelectedOccludee.Prefab.name}</b> occluding <b>{occludeesTreeView.SelectedOccludee.Prefab.name}</b>";
            }
            else if (occludersTreeView.SelectedOccludee != null && occludersTreeView.SelectedOccludee.Prefab)
            {
                text += $" <b>{PrefabsTreeView.SelectedOccludee.Prefab.name}</b> occluded by <b>{occludersTreeView.SelectedOccludee.Prefab.name}</b>";
            }

            return text;
        }

        private void onLockButtonClicked()
        {
            PrefabsTreeView.IsSelectionLocked = !PrefabsTreeView.IsSelectionLocked;

            if (!PrefabsTreeView.IsSelectionLocked)
            {
                PrefabsTreeView.SetSelection(new List<int> { PrefabsTreeView.SelectedOccludee.GetInstanceID() });
            }
        }

        private void onViewButtonClicked()
        {
            if (PrefabsTreeView.SelectedOccludee == null)
            {
                return;
            }

            if (occludeesTreeView.SelectedOccludee != null)
            {
                openViewOcclusionDataScene(occludeesTreeView.SelectedOccludee, PrefabsTreeView.SelectedOccludee);
            }
            else if (occludersTreeView.SelectedOccludee != null)
            {
                openViewOcclusionDataScene(PrefabsTreeView.SelectedOccludee, occludersTreeView.SelectedOccludee);
            }
        }

        #endregion

        #region Private Methods

        private TreeViewState initTreeViewState(ref TreeViewState serializedState)
        {
            if (serializedState == null)
            {
                serializedState = new TreeViewState();
            }

            return serializedState;
        }

        private OccludeeCategoryTreeViewHeader initOccludeeCategoryTreeViewHeader(ref MultiColumnHeaderState serializedMultiColumnHeaderState)
        {
            MultiColumnHeaderState headerState = OccludeeCategoryTreeViewHeader.CreateMultiColumnHeaderState();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(serializedMultiColumnHeaderState, headerState))
            {
                MultiColumnHeaderState.OverwriteSerializedFields(serializedMultiColumnHeaderState, headerState);
            }
            serializedMultiColumnHeaderState = headerState;

            return new OccludeeCategoryTreeViewHeader(headerState);
        }

        private void onPrefabsTreeViewSelectedOccludeeChanged()
        {
            occludeesTreeView.ClearSelection();
            occludeesTreeView.Reload();

            occludersTreeView.ClearSelection();
            occludersTreeView.Reload();
        }

        private void onOccludeesTreeViewSelectedOccludeeChanged()
        {
            occludersTreeView.ClearSelection();
        }

        private void onOccludersTreeViewSelectedOccludeeChanged()
        {
            occludeesTreeView.ClearSelection();
        }

        private void validatePrefabs()
        {
            OccludeeValidationResults.Clear();

            foreach (OccludeeCategory occludeeCategory in Config.OccludeeCategories)
            {
                foreach (OccludeeConfiguration occludee in occludeeCategory.Occludees)
                {
                    OccludeeValidationResults.Add(occludee, OccludeeConfigurationValidationResult.Create(occludee));
                }
            }
        }

        private void removeNullValuesAndUnusedDataFromConfig()
        {
            foreach (OccludeeCategory occludeeCategory in Config.OccludeeCategories)
            {
                if (occludeeCategory.Occludees.RemoveUnityNullValues())
                {
                    EditorUtility.SetDirty(Config);
                }

                foreach (OccludeeConfiguration occludee in occludeeCategory.Occludees)
                {
                    if (occludee.Occluders.RemoveUnityNullValues() || occludee.RemoveUnusedLODOcclusionData())
                    {
                        EditorUtility.SetDirty(occludee);
                    }
                }
            }

            AssetDatabase.SaveAssets();
        }

        private void replaceInvalidOcclusionDataWithNew()
        {
            foreach (OccludeeCategory occludeeCategory in Config.OccludeeCategories)
            {
                foreach (OccludeeConfiguration occludee in occludeeCategory.Occludees)
                {
                    if (occludee.Prefab != null && !occludee.IsOcclusionDataValid())
                    {
                        string name = occludee.Prefab != null ? occludee.Prefab.name : occludee.name;

                        Debug.Log($"Creating new Occlusion data for '{name}' as the previous data is invalid.");

                        occludee.CreateNewLODOcclusionData();
                        EditorUtility.SetDirty(occludee);
                    }
                }
            }

            AssetDatabase.SaveAssets();
        }

        private void openViewOcclusionDataScene(OccludeeConfiguration occludee, OccludeeConfiguration occluder)
        {
            if (OcclusionDataViewer.IsSceneOpen)
            {
                OcclusionDataViewer.ReloadScene(occludee, occluder);
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            OcclusionDataViewer.OpenScene(occludee, occluder);
        }

        private void updateSkinnedMeshDataIfChanged()
        {
            foreach (OccludeeCategory occludeeCategory in Config.OccludeeCategories)
            {
                foreach (OccludeeConfiguration occludee in occludeeCategory.Occludees)
                {
                    occludee.UpdateSkinnedMeshDataIfChanged();
                }
            }
        }

        #endregion

        #region Public Methods

        public void Reload()
        {
            Config = ClothingCullerConfiguration.FindOrCreate();

            if (Config == null || PrefabsTreeView == null || occludeesTreeView == null || occludersTreeView == null)
            {
                return;
            }

            titleContent.text = $"Edit Configuration ({Config.name})";

            if (!OcclusionDataViewer.IsSceneOpen)
            {
                removeNullValuesAndUnusedDataFromConfig();
                replaceInvalidOcclusionDataWithNew();
            }

            if (isPrefabValidationEnabled)
            {
                validatePrefabs();
            }

            if (Config.IsModularClothingWorkflowEnabled)
            {
                updateSkinnedMeshDataIfChanged();
            }

            PrefabsTreeView.Reload();
            occludeesTreeView.Reload();
            occludersTreeView.Reload();
        }

        #endregion

        #region Inner Classes

        [Serializable]
        public class OccludeesByOccludee : SerializableDictionary<OccludeeConfiguration, OccludeeList>
        {

        }

        [Serializable]
        public class OccludeeList : List<OccludeeConfiguration>
        {

        }

        [Serializable]
        public class ValidationResultByOccludee : SerializableDictionary<OccludeeConfiguration, OccludeeConfigurationValidationResult>
        {

        }

        #endregion

    }
}
