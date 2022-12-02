using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Salvage.ClothingCuller.Editor.TreeViews
{
    public class OccludeeCategoryTreeViewHeader : MultiColumnHeader
    {
        #region Enums

        public enum Column
        {
            AssetType,
            Name
        }

        #endregion

        #region Constructor

        public OccludeeCategoryTreeViewHeader(MultiColumnHeaderState state) : base(state)
        {

        }

        #endregion

        #region Private Methods



        #endregion

        #region Public Methods

        public static MultiColumnHeaderState CreateMultiColumnHeaderState()
        {
            var columns = new List<MultiColumnHeaderState.Column>
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Asset Type"),
                    contextMenuText = "Asset Type",
                    headerTextAlignment = TextAlignment.Left,
                    width = 20,
                    minWidth = 20,
                    maxWidth = 20,
                    autoResize = true,
                    allowToggleVisibility = false,
                    canSort = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false,
                    canSort = true
                }
            };

            return new MultiColumnHeaderState(columns.ToArray());
        }

        #endregion

    }
}
