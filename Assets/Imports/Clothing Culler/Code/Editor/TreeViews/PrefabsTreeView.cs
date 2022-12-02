using System.Collections.Generic;
using System.Linq;
using Salvage.ClothingCuller.Configuration;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Salvage.ClothingCuller.Components;
using Salvage.ClothingCuller.Editor.EditorWindows;
using System.IO;
using Salvage.ClothingCuller.ExtensionMethods;

namespace Salvage.ClothingCuller.Editor.TreeViews
{
    public class PrefabsTreeView : BaseOccludeeCategoryTreeView
    {
        public bool IsSelectionLocked { get; set; }

        #region Constructor

        public PrefabsTreeView(TreeViewState state, MultiColumnHeader multiColumnHeaderheader, ClothingCullerConfigurationEditorWindow editorWindow) : base(state, multiColumnHeaderheader, editorWindow)
        {

        }

        #endregion

        #region Override Methods

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        protected override bool CanRename(TreeViewItem item)
        {
            if (item is OccludeeCategoryTreeViewItem occludeeCategoryTreeViewItem)
            {
                occludeeCategoryTreeViewItem.IsBeingRenamed = true;
                return true;
            }

            return false;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            TreeViewItem item = GetRows().FirstOrDefault(x => x.id == args.itemID);
            if (item is OccludeeCategoryTreeViewItem occludeeCategoryTreeViewItem)
            {
                occludeeCategoryTreeViewItem.IsBeingRenamed = false;

                if (args.acceptedRename)
                {
                    occludeeCategoryTreeViewItem.OccludeeCategory.Name = args.newName;
                    Reload();
                }
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (IsSelectionLocked)
            {
                return;
            }

            GetRows().OfType<OccludeeCategoryTreeViewItem>().ToList().ForEach(x => x.IsBeingRenamed = false);

            base.SelectionChanged(selectedIds);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (DragAndDrop.objectReferences.Length > 0)
            {
                return handleDragAndDropPrefabs(args, DragAndDrop.objectReferences);
            }

            return base.HandleDragAndDrop(args);
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            var draggedRows = GetRows().Where(x => args.draggedItemIDs.Contains(x.id)).ToList();

            DragAndDrop.PrepareStartDrag();
            DragAndDrop.paths = new string[0];
            DragAndDrop.objectReferences = new Object[0];
            DragAndDrop.activeControlID = 0;
            DragAndDrop.SetGenericData("draggedRows", draggedRows);
            DragAndDrop.StartDrag("Drag rows");
        }

        protected override void handleContextClicked()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Create Prefab Category"), false, CreatePrefabCategory);

            menu.ShowAsContext();
        }

        protected override void handleContextClickedOnItem()
        {
            var selectedRows = GetRows().Where(x => GetSelection().Contains(x.id)).ToList();

            if (selectedRows.Select(x => x.GetType()).Distinct().Count() != 1)
            {
                return;
            }

            var menu = new GenericMenu();

            switch (selectedRows.First())
            {
                case OccludeeCategoryTreeViewItem occludeeCategoryTreeViewItem:
                    if (selectedRows.Count == 1)
                    {
                        menu.AddItem(new GUIContent("Rename"), false, onRenameCategoryClicked, occludeeCategoryTreeViewItem);
                        menu.AddSeparator(string.Empty);
                    }
                    menu.AddItem(new GUIContent("Remove"), false, onRemoveCategoriesClicked, selectedRows.OfType<OccludeeCategoryTreeViewItem>().ToList());
                    break;
                case OccludeeConfigurationTreeViewItem occludeeConfigurationTreeViewItem:
                    var selectedOccludeeItems = selectedRows.OfType<OccludeeConfigurationTreeViewItem>().ToList();
                    addMoveToCategoryMenuItems(menu, selectedOccludeeItems);

                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Remove"), false, onRemoveOccludeesClicked, selectedOccludeeItems);
                    break;
            }

            menu.ShowAsContext();
        }

        private void addMoveToCategoryMenuItems(GenericMenu menu, List<OccludeeConfigurationTreeViewItem> selectedOccludeeItems)
        {
            List<OccludeeCategory> sortedCategories;

            if (isSortedAscending())
            {
                sortedCategories = editorWindow.Config.OccludeeCategories.OrderBy(x => x.Name).ToList();
            }
            else
            {
                sortedCategories = editorWindow.Config.OccludeeCategories.OrderByDescending(x => x.Name).ToList();
            }

            foreach (OccludeeCategory category in sortedCategories)
            {
                var targetCategoryWithSelectedOccludeeItems = new System.Tuple<OccludeeCategory, List<OccludeeConfigurationTreeViewItem>>(category, selectedOccludeeItems);

                menu.AddItem(new GUIContent($"Move to Prefab Category/{category.Name}"), false, onMoveToCategoryClicked, targetCategoryWithSelectedOccludeeItems);
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            OccludeeConfigurationTreeViewItem selectedOccludeeItem = GetRows().OfType<OccludeeConfigurationTreeViewItem>().FirstOrDefault(x => x.id == id);
            if (selectedOccludeeItem == null)
            {
                return;
            }

            if (selectedOccludeeItem.OccludeeConfiguration.Prefab != null)
            {
                EditorGUIUtility.PingObject(selectedOccludeeItem.OccludeeConfiguration.Prefab.gameObject);
                return;
            }

            EditorGUIUtility.PingObject(selectedOccludeeItem.OccludeeConfiguration);
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
                if (selectedRows.Select(x => x.GetType()).Distinct().Count() != 1)
                {
                    return;
                }

                switch (selectedRows.First())
                {
                    case OccludeeCategoryTreeViewItem occludeeCategoryTreeViewItem:
                        removeCategoryItems(selectedRows.OfType<OccludeeCategoryTreeViewItem>().ToList());
                        break;
                    case OccludeeConfigurationTreeViewItem occludeeConfigurationTreeViewItem:
                        removeOccludeeItems(selectedRows.OfType<OccludeeConfigurationTreeViewItem>().ToList());
                        break;
                }
            }
        }

        protected override bool isOccludeeNew(OccludeeConfiguration occludee)
        {
            if (occludee.Occluders.Count > 0)
            {
                return false;
            }

            foreach (OccludeeCategory configCategory in editorWindow.Config.OccludeeCategories)
            {
                foreach (OccludeeConfiguration configOccludee in configCategory.Occludees)
                {
                    if (occludee == configOccludee)
                    {
                        continue;
                    }

                    if (configOccludee.Occluders.Contains(occludee))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override bool? isOccludeeValid(OccludeeConfiguration occludeeConfiguration)
        {
            if (editorWindow.OccludeeValidationResults == null || !editorWindow.OccludeeValidationResults.TryGetValue(occludeeConfiguration, out OccludeeConfigurationValidationResult validationResult))
            {
                return null;
            }

            if (!validationResult.IsOcclusionDataValid)
            {
                return false;
            }

            foreach (OccludeeConfiguration occluder in occludeeConfiguration.Occluders)
            {
                bool? isOcclusionDataComplete = validationResult.IsOcclusionDataComplete(occluder);
                if (isOcclusionDataComplete.HasValue && !isOcclusionDataComplete.Value)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void onRenameCategoryClicked(object selectedItem)
        {
            var categoryTreeViewItem = selectedItem as OccludeeCategoryTreeViewItem;
            categoryTreeViewItem.IsBeingRenamed = true;

            BeginRename(categoryTreeViewItem);
        }

        private void onMoveToCategoryClicked(object selectedItem)
        {
            var targetCategoryWithSelectedOccludeeItems = selectedItem as System.Tuple<OccludeeCategory, List<OccludeeConfigurationTreeViewItem>>;

            moveOccludeeItemsToCategory(targetCategoryWithSelectedOccludeeItems.Item1, targetCategoryWithSelectedOccludeeItems.Item2);

            Reload();
        }

        private void onRemoveCategoriesClicked(object selectedRows)
        {
            removeCategoryItems(selectedRows as List<OccludeeCategoryTreeViewItem>);
        }

        private void onRemoveOccludeesClicked(object selectedRows)
        {
            removeOccludeeItems(selectedRows as List<OccludeeConfigurationTreeViewItem>);
        }

        private void removeCategoryItems(List<OccludeeCategoryTreeViewItem> categoryItems)
        {
            if (categoryItems.Any(x => x.hasChildren))
            {
                string subject = categoryItems.Count > 1 ? "Prefab Categories" : "Prefab Category";
                string title = $"Remove {subject}?";
                string message = "Removing a Prefab Category will also remove all of it's configured prefabs.\n\n" +
                                 "You cannot undo this action.";

                if (!EditorUtility.DisplayDialog(title, message, "Remove", "Cancel"))
                {
                    return;
                }
            }

            foreach (OccludeeCategoryTreeViewItem selectedCategoryItem in categoryItems)
            {
                selectedCategoryItem.OccludeeCategory.Occludees.ForEach(x => removeOccludeeComponentAndDeleteConfigurationFile(x));

                editorWindow.Config.OccludeeCategories.Remove(selectedCategoryItem.OccludeeCategory);
            }

            EditorUtility.SetDirty(editorWindow.Config);
            AssetDatabase.SaveAssets();

            ClearSelection();
            editorWindow.Reload();
        }

        private void removeOccludeeItems(List<OccludeeConfigurationTreeViewItem> occludeeItems)
        {
            string subject = occludeeItems.Count > 1 ? "Prefabs" : "Prefab";
            string title = $"Remove {subject}?";
            string message = $"Are you sure you want to remove the selected {subject}?\n\n" +
                             $"You cannot undo this action.";

            if (!EditorUtility.DisplayDialog(title, message, "Remove", "Cancel"))
            {
                return;
            }

            foreach (OccludeeConfigurationTreeViewItem selectedOccludeeItem in occludeeItems)
            {
                var parentCategory = selectedOccludeeItem.parent as OccludeeCategoryTreeViewItem;
                parentCategory.OccludeeCategory.Occludees.Remove(selectedOccludeeItem.OccludeeConfiguration);

                removeOccludeeComponentAndDeleteConfigurationFile(selectedOccludeeItem.OccludeeConfiguration);
            }

            EditorUtility.SetDirty(editorWindow.Config);
            AssetDatabase.SaveAssets();

            ClearSelection();
            editorWindow.Reload();
        }

        private void removeOccludeeComponentAndDeleteConfigurationFile(OccludeeConfiguration occludeeConfiguration)
        {
            if (occludeeConfiguration.Prefab != null)
            {
                Occludee occludeeComponent = occludeeConfiguration.Prefab.GetComponent<Occludee>();
                if (occludeeComponent != null)
                {
                    Object.DestroyImmediate(occludeeComponent, true);
                }
            }

            string assetPath = AssetDatabase.GetAssetPath(occludeeConfiguration);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }
        }

        private DragAndDropVisualMode handleDragAndDropPrefabs(DragAndDropArgs args, Object[] objectReferences)
        {
            if (!(args.parentItem is OccludeeCategoryTreeViewItem occludeeCategoryTreeViewItem))
            {
                return DragAndDropVisualMode.Rejected;
            }

            for (int i = 0; i < objectReferences.Length; i++)
            {
                Object objectReference = objectReferences[i];
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(objectReference);

                if (prefabAssetType != PrefabAssetType.Regular && prefabAssetType != PrefabAssetType.Variant)
                {
                    return DragAndDropVisualMode.Rejected;
                }

                PrefabInstanceStatus instance = PrefabUtility.GetPrefabInstanceStatus(objectReference);
                GameObject prefab;

                if (instance == PrefabInstanceStatus.Connected)
                {
                    objectReference = PrefabUtility.GetCorrespondingObjectFromOriginalSource(objectReference);
                    objectReferences[i] = objectReference;
                }

                prefab = objectReference as GameObject;
                if (editorWindow.Config.OccludeeCategories.Any(x => x.Occludees.Any(y => y.Prefab == prefab)))
                {
                    return DragAndDropVisualMode.Rejected;
                }
            }

            if (args.performDrop)
            {
                foreach (Object objectReference in objectReferences)
                {
                    var prefab = objectReference as GameObject;

                    Occludee occludeeComponent = prefab.GetComponent<Occludee>();
                    if (occludeeComponent == null)
                    {
                        occludeeComponent = prefab.AddComponent<Occludee>();
                    }

                    if (occludeeComponent.Config == null)
                    {
                        var occludeeConfiguration = OccludeeConfiguration.Create(occludeeComponent);

                        if (editorWindow.Config.IsModularClothingWorkflowEnabled)
                        {
                            occludeeConfiguration.CreateSkinnedMeshData();
                        }
                        occludeeConfiguration.SaveToDisk(getOccludeeFolderPath());

                        occludeeComponent.SetConfiguration(occludeeConfiguration);
                        EditorUtility.SetDirty(occludeeComponent);
                    }

                    enableReadWriteForMeshes(occludeeComponent);

                    occludeeCategoryTreeViewItem.OccludeeCategory.Occludees.Add(occludeeComponent.Config);
                }

                EditorUtility.SetDirty(editorWindow.Config);
                AssetDatabase.SaveAssets();

                editorWindow.Reload();
            }

            return DragAndDropVisualMode.Copy;
        }

        private string getOccludeeFolderPath()
        {
            string configFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(editorWindow.Config));

            return Path.Combine(configFolderPath, "Occludees");
        }

        private void enableReadWriteForMeshes(Occludee occludee)
        {
            foreach (Renderer[] renderers in occludee.GetRenderersByLOD())
            {
                foreach (Renderer renderer in renderers)
                {
                    Mesh sharedMesh = renderer.GetSharedMesh();
                    if (sharedMesh == null || sharedMesh.isReadable)
                    {
                        continue;
                    }

                    var modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sharedMesh)) as ModelImporter;
                    if (modelImporter.isReadable)
                    {
                        continue;
                    }

                    modelImporter.isReadable = true;
                    modelImporter.SaveAndReimport();
                    Debug.Log($"Enabled Read/Write for '{modelImporter.assetPath}'.");
                }
            }
        }

        #endregion

        #region Public Methods

        public void CreatePrefabCategory()
        {
            var category = new OccludeeCategory { Name = "New Prefab Category", Occludees = new List<OccludeeConfiguration>() };

            editorWindow.Config.OccludeeCategories.Add(category);
            EditorUtility.SetDirty(editorWindow.Config);
            AssetDatabase.SaveAssets();

            Reload();

            OccludeeCategoryTreeViewItem newRow = GetRows().OfType<OccludeeCategoryTreeViewItem>().FirstOrDefault(x => x.OccludeeCategory == category);
            if (newRow != null)
            {
                newRow.IsBeingRenamed = true;
                BeginRename(newRow);
            }
        }

        #endregion

    }
}
