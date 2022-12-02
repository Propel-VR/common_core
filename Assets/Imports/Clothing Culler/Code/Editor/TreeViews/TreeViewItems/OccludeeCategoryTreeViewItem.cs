using Salvage.ClothingCuller.Configuration;
using UnityEditor.IMGUI.Controls;

namespace Salvage.ClothingCuller.Editor.TreeViews
{
    public class OccludeeCategoryTreeViewItem : TreeViewItem
    {
        public readonly OccludeeCategory OccludeeCategory;
        public bool IsBeingRenamed { get; set; }

        #region Constructor

        public OccludeeCategoryTreeViewItem(int id, int depth, string displayName, OccludeeCategory occludeeCategory) : base(id, depth, displayName)
        {
            OccludeeCategory = occludeeCategory;
        }

        #endregion

        #region Override Methods

        public override string displayName
        {
            get
            {
                if (IsBeingRenamed)
                {
                    return OccludeeCategory.Name;
                }

                int childCount = hasChildren ? children.Count : 0;
                return $"{OccludeeCategory.Name} ({childCount})";
            }
            set
            {
                base.displayName = value;
            }
        }

        #endregion

        #region Private Methods



        #endregion

        #region Public Methods



        #endregion

    }
}
