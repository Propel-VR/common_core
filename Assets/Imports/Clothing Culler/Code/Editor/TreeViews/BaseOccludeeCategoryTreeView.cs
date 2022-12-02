using System;
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
    public abstract class BaseOccludeeCategoryTreeView : TreeView
    {
        public event Action SelectedOccludeeChanged;

        public OccludeeConfiguration SelectedOccludee { get; private set; }

        protected readonly ClothingCullerConfigurationEditorWindow editorWindow;
        private readonly List<int> emptySelectionList;
        private bool contextClickedItem;

        #region Constructor

        public BaseOccludeeCategoryTreeView(TreeViewState state, MultiColumnHeader multiColumnHeaderheader, ClothingCullerConfigurationEditorWindow editorWindow) : base(state, multiColumnHeaderheader)
        {
            this.editorWindow = editorWindow;
            emptySelectionList = new List<int>();

            columnIndexForTreeFoldouts = 1;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            multiColumnHeader.sortingChanged += onSortingChanged;

            Reload();
            SelectionChanged(GetSelection());
        }

        #endregion

        #region Override Methods

        protected sealed override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem(-1, -1, "root");
            var rows = new List<TreeViewItem>();

            for (int i = 0; i < editorWindow.Config.OccludeeCategories.Count; i++)
            {
                OccludeeCategory occludeeCategory = editorWindow.Config.OccludeeCategories[i];

                if (!canBuildOccludeeCategoryRow(occludeeCategory))
                {
                    continue;
                }

                rows.Add(new OccludeeCategoryTreeViewItem(i, 0, occludeeCategory.Name, occludeeCategory));

                for (int j = 0; j < occludeeCategory.Occludees.Count; j++)
                {
                    OccludeeConfiguration occludeeConfiguration = occludeeCategory.Occludees[j];
                    if (!canBuildOccludeeConfigurationRow(occludeeConfiguration))
                    {
                        continue;
                    }

                    string name = occludeeConfiguration.Prefab != null ? occludeeConfiguration.Prefab.name : $"{occludeeConfiguration.name} (Prefab link missing)";
                    rows.Add(new OccludeeConfigurationTreeViewItem(occludeeConfiguration.GetInstanceID(), 1, name, occludeeConfiguration, isOccludeeNew(occludeeConfiguration), isOccludeeValid(occludeeConfiguration)));
                }
            }

            SetupParentsAndChildrenFromDepths(root, rows);

            sortRecursively(root, isSortedAscending());

            return root;
        }

        protected sealed override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                cellGUI(args.GetCellRect(i), i, args.item);
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count == 1)
            {
                TreeViewItem item = GetRows().FirstOrDefault(x => x.id == selectedIds.First());
                if (item is OccludeeConfigurationTreeViewItem occludeeConfigurationTreeViewItem)
                {
                    SelectedOccludee = occludeeConfigurationTreeViewItem.OccludeeConfiguration;
                }
                else
                {
                    SelectedOccludee = null;
                }
            }
            else
            {
                SelectedOccludee = null;
            }

            SelectedOccludeeChanged?.Invoke();
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (!rootItem.hasChildren)
            {
                return DragAndDropVisualMode.Rejected;
            }

            if (!(DragAndDrop.GetGenericData("draggedRows") is List<TreeViewItem> draggedRows) || draggedRows.Count == 0)
            {
                return DragAndDropVisualMode.None;
            }

            if (draggedRows.Select(x => x.GetType()).Distinct().Count() != 1)
            {
                return DragAndDropVisualMode.Rejected;
            }

            if (draggedRows.Any(x => getRootParent(x) == rootItem))
            {
                return handleDragAndDropMove(args, draggedRows);
            }

            return handleDragAndDropCopy(args, draggedRows);
        }

        protected sealed override void ContextClicked()
        {
            if (contextClickedItem)
            {
                handleContextClickedOnItem();

                contextClickedItem = false;
                return;
            }

            handleContextClicked();
        }

        protected sealed override void ContextClickedItem(int id)
        {
            contextClickedItem = true;
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            if (!(item is OccludeeConfigurationTreeViewItem))
            {
                return false;
            }

            return base.DoesItemMatchSearch(item, search);
        }

        #endregion

        #region Private Methods

        protected virtual void handleContextClickedOnItem()
        {

        }

        protected virtual void handleContextClicked()
        {

        }

        protected virtual bool canBuildOccludeeCategoryRow(OccludeeCategory occludeeCategory)
        {
            return true;
        }

        protected virtual bool canBuildOccludeeConfigurationRow(OccludeeConfiguration occludeeConfiguration)
        {
            return true;
        }

        protected virtual bool canReceiveOccludeeConfigurationDrop(OccludeeConfiguration occludeeConfiguration)
        {
            return false;
        }

        protected virtual void receiveOccludeeConfigurationsDrop(HashSet<OccludeeConfiguration> occludeeConfigurations)
        {

        }

        protected virtual bool isOccludeeNew(OccludeeConfiguration occludee)
        {
            return false;
        }

        protected virtual bool? isOccludeeValid(OccludeeConfiguration occludeeConfiguration)
        {
            return null;
        }

        private void cellGUI(Rect cellRect, int columnIndex, TreeViewItem item)
        {
            switch (columnIndex)
            {
                case 0:
                    if (item is OccludeeCategoryTreeViewItem occludeeCategoryItem)
                    {
                        GUI.DrawTexture(cellRect, EditorGUIUtility.IconContent("Folder Icon").image, ScaleMode.ScaleAndCrop);
                    }
                    else if (item is OccludeeConfigurationTreeViewItem occludeeConfigurationItem)
                    {
                        GUI.DrawTexture(cellRect, EditorGUIUtility.IconContent("Prefab Icon").image, ScaleMode.ScaleAndCrop);

                        if (occludeeConfigurationItem.IsValid.HasValue)
                        {
                            string iconName = occludeeConfigurationItem.IsValid.Value ? "TestPassed" : "TestFailed";
                            cellRect.x += 2f;

                            GUI.DrawTexture(cellRect, EditorGUIUtility.IconContent(iconName).image, ScaleMode.ScaleAndCrop);
                        }
                    }
                    break;
                case 1:
                    var style = new GUIStyle(EditorStyles.label) { richText = true };
                    float indent = GetContentIndent(item);
                    Rect indentedRect = cellRect;
                    indentedRect.xMin += indent;

                    EditorGUI.LabelField(indentedRect, item.displayName, style);

                    if (item is OccludeeConfigurationTreeViewItem occludeeConfigurationItem2 && occludeeConfigurationItem2.IsNew)
                    {
                        indentedRect.x = indentedRect.width * 0.5f + indent;
                        indentedRect.y += 3;
                        indentedRect.height = 10;
                        GUI.DrawTexture(indentedRect, EditorGUIUtility.IconContent("d_AS Badge New").image, ScaleMode.ScaleToFit);
                    }
                    break;
            }
        }

        protected bool isSortedAscending()
        {
            if (multiColumnHeader.sortedColumnIndex == -1)
            {
                return true;
            }

            return multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);
        }

        private void onSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            var column = (OccludeeCategoryTreeViewHeader.Column)multiColumnHeader.sortedColumnIndex;
            bool isSortedAscending = multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);

            switch (column)
            {
                case OccludeeCategoryTreeViewHeader.Column.Name:
                    sortRecursively(rootItem, isSortedAscending);
                    break;
                default:
                    return;
            }

            Reload();
        }

        private void sortRecursively(TreeViewItem treeViewItem, bool sortAscending)
        {
            treeViewItem.children.Sort();

            if (!sortAscending)
            {
                treeViewItem.children.Reverse();
            }

            foreach (TreeViewItem child in treeViewItem.children)
            {
                if (child.hasChildren)
                {
                    sortRecursively(child, sortAscending);
                }
            }
        }

        private TreeViewItem getRootParent(TreeViewItem treeViewItem)
        {
            TreeViewItem parent = treeViewItem;

            while (parent.parent != null)
            {
                parent = parent.parent;
            }

            return parent;
        }

        private DragAndDropVisualMode handleDragAndDropMove(DragAndDropArgs args, List<TreeViewItem> draggedRows)
        {
            if (!(draggedRows.First() is OccludeeConfigurationTreeViewItem) || !(args.parentItem is OccludeeCategoryTreeViewItem targetOccludeeCategoryTreeViewItem))
            {
                return DragAndDropVisualMode.Rejected;
            }

            if (draggedRows.All(x => x.parent == args.parentItem))
            {
                return DragAndDropVisualMode.Rejected;
            }

            if (args.performDrop)
            {
                moveOccludeeItemsToCategory(targetOccludeeCategoryTreeViewItem.OccludeeCategory, draggedRows.OfType<OccludeeConfigurationTreeViewItem>().ToList());

                Reload();
            }

            return DragAndDropVisualMode.Move;
        }

        protected void moveOccludeeItemsToCategory(OccludeeCategory targetCategory, List<OccludeeConfigurationTreeViewItem> occludeeItems)
        {
            foreach (OccludeeConfigurationTreeViewItem occludeeItem in occludeeItems)
            {
                var currentCategoryTreeViewItem = occludeeItem.parent as OccludeeCategoryTreeViewItem;

                currentCategoryTreeViewItem.OccludeeCategory.Occludees.Remove(occludeeItem.OccludeeConfiguration);
                targetCategory.Occludees.Add(occludeeItem.OccludeeConfiguration);
            }

            EditorUtility.SetDirty(editorWindow.Config);
            AssetDatabase.SaveAssets();
        }

        private DragAndDropVisualMode handleDragAndDropCopy(DragAndDropArgs args, List<TreeViewItem> draggedRows)
        {
            var droppedOccludees = new HashSet<OccludeeConfiguration>();
            foreach (TreeViewItem draggedRow in draggedRows)
            {
                switch (draggedRow)
                {
                    case OccludeeCategoryTreeViewItem occludeeCategoryTreeViewItem:
                        droppedOccludees.AddRange(occludeeCategoryTreeViewItem.OccludeeCategory.Occludees);
                        break;
                    case OccludeeConfigurationTreeViewItem occludeeConfigurationTreeViewItem:
                        droppedOccludees.Add(occludeeConfigurationTreeViewItem.OccludeeConfiguration);
                        break;
                }
            }

            if (!droppedOccludees.All(x => canReceiveOccludeeConfigurationDrop(x)))
            {
                return DragAndDropVisualMode.Rejected;
            }

            if (args.performDrop)
            {
                receiveOccludeeConfigurationsDrop(droppedOccludees);
            }

            return DragAndDropVisualMode.Copy;
        }

        #endregion

        #region Public Methods        

        public int GetOccludeeConfigurationCount()
        {
            return GetRows().OfType<OccludeeConfigurationTreeViewItem>().Count();
        }

        public void ClearSelection()
        {
            SelectedOccludee = null;

            SetSelection(emptySelectionList);
        }

        public float GetWidth()
        {
            return treeViewRect.width;
        }

        #endregion

    }
}
