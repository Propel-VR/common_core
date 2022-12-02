using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Salvage.ClothingCuller.Configuration;
using Salvage.ClothingCuller.Editor.EditorWindows;
using Salvage.ClothingCuller.ExtensionMethods;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Salvage.ClothingCuller.Editor.TreeViews
{
    public class OccludeesTreeView : BaseOccludeeCategoryTreeView
    {
        #region Constructor

        public OccludeesTreeView(TreeViewState state, MultiColumnHeader multiColumnHeaderheader, ClothingCullerConfigurationEditorWindow editorWindow) : base(state, multiColumnHeaderheader, editorWindow)
        {

        }

        #endregion

        #region Override Methods

        protected override bool canBuildOccludeeCategoryRow(OccludeeCategory occludeeCategory)
        {
            return editorWindow.PrefabsTreeView.SelectedOccludee != null;
        }

        protected override bool canBuildOccludeeConfigurationRow(OccludeeConfiguration occludeeConfiguration)
        {
            if (editorWindow.PrefabsTreeView.SelectedOccludee == null)
            {
                return false;
            }

            return occludeeConfiguration.Occluders.Contains(editorWindow.PrefabsTreeView.SelectedOccludee);
        }

        protected override bool canReceiveOccludeeConfigurationDrop(OccludeeConfiguration occludeeConfiguration)
        {
            if (editorWindow.PrefabsTreeView.SelectedOccludee == null || editorWindow.PrefabsTreeView.SelectedOccludee == occludeeConfiguration)
            {
                return false;
            }

            if (occludeeConfiguration.Occluders.Contains(editorWindow.PrefabsTreeView.SelectedOccludee))
            {
                return false;
            }

            if (editorWindow.PrefabsTreeView.SelectedOccludee.Occluders.Contains(occludeeConfiguration))
            {
                return false;
            }

            return true;
        }

        protected override void receiveOccludeeConfigurationsDrop(HashSet<OccludeeConfiguration> occludeeConfigurations)
        {
            if (editorWindow.PrefabsTreeView.SelectedOccludee == null)
            {
                return;
            }

            foreach (OccludeeConfiguration occludeeConfiguration in occludeeConfigurations)
            {
                occludeeConfiguration.Occluders.Add(editorWindow.PrefabsTreeView.SelectedOccludee);

                EditorUtility.SetDirty(occludeeConfiguration);
            }

            AssetDatabase.SaveAssets();
            editorWindow.Reload();
        }

        protected override void handleContextClickedOnItem()
        {
            if (editorWindow.PrefabsTreeView.SelectedOccludee == null)
            {
                return;
            }

            var selectedRows = GetRows().Where(x => GetSelection().Contains(x.id)).ToList();
            var selectedOccludeeItems = selectedRows.OfType<OccludeeConfigurationTreeViewItem>().ToList();

            if (selectedRows.Count != selectedOccludeeItems.Count)
            {
                return;
            }

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove"), false, onRemoveOccludeesClicked, selectedOccludeeItems);
            menu.ShowAsContext();
        }

        protected override void DoubleClickedItem(int id)
        {
            OccludeeConfigurationTreeViewItem selectedOccludeeItem = GetRows().OfType<OccludeeConfigurationTreeViewItem>().FirstOrDefault(x => x.id == id);
            if (selectedOccludeeItem == null || editorWindow.PrefabsTreeView.IsSelectionLocked)
            {
                return;
            }

            editorWindow.PrefabsTreeView.SetSelection(new List<int> { id }, TreeViewSelectionOptions.FireSelectionChanged);
            editorWindow.PrefabsTreeView.SetFocus();
        }

        protected override void KeyEvent()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                IList<int> selection = GetSelection();
                if (selection.Count == 0)
                {
                    return;
                }

                var selectedRows = GetRows().Where(x => GetSelection().Contains(x.id)).ToList();
                var selectedOccludeeItems = selectedRows.OfType<OccludeeConfigurationTreeViewItem>().ToList();

                if (selectedRows.Count != selectedOccludeeItems.Count)
                {
                    return;
                }

                removeOccludeeItems(selectedOccludeeItems);
            }
        }

        protected override bool isOccludeeNew(OccludeeConfiguration occludee)
        {
            if (editorWindow.PrefabsTreeView.SelectedOccludee == null)
            {
                return false;
            }

            for (int i = 0; i < occludee.LodOcclusionData.Length; i++)
            {
                LODOcclusionData occludeeLODOcclusionData = occludee.LodOcclusionData[i];
                LODOcclusionData occluderLODOcclusionData = editorWindow.PrefabsTreeView.SelectedOccludee.LodOcclusionData.ElementAtOrLast(i);

                foreach (MeshOcclusionData meshOcclusionData in occludeeLODOcclusionData.MeshOcclusionData)
                {
                    if (meshOcclusionData.TryGetTriangleOcclusionData(occluderLODOcclusionData.Id, out bool[] triangleOcclusionData))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override bool? isOccludeeValid(OccludeeConfiguration occludeeConfiguration)
        {
            if (editorWindow.OccludeeValidationResults == null || editorWindow.PrefabsTreeView.SelectedOccludee == null || !editorWindow.OccludeeValidationResults.TryGetValue(occludeeConfiguration, out OccludeeConfigurationValidationResult validationResult))
            {
                return null;
            }

            return validationResult.IsOcclusionDataComplete(editorWindow.PrefabsTreeView.SelectedOccludee);
        }

        #endregion

        #region Private Methods

        private void onRemoveOccludeesClicked(object selectedRows)
        {
            removeOccludeeItems(selectedRows as List<OccludeeConfigurationTreeViewItem>);
        }

        private void removeOccludeeItems(List<OccludeeConfigurationTreeViewItem> occludeeItems)
        {
            string subject = occludeeItems.Count > 1 ? "Occludees" : "Occludee";
            string title = $"Remove {subject}?";
            string message = $"Are you sue you want to remove the selected {subject}?\n\n" +
                             $"You cannot undo this action.";

            if (!EditorUtility.DisplayDialog(title, message, "Remove", "Cancel"))
            {
                return;
            }

            occludeeItems.ForEach(x => x.OccludeeConfiguration.Occluders.Remove(editorWindow.PrefabsTreeView.SelectedOccludee));

            ClearSelection();
            editorWindow.Reload();
        }

        #endregion

        #region Public Methods



        #endregion

    }
}
