using Salvage.ClothingCuller.Configuration;
using UnityEditor.IMGUI.Controls;

namespace Salvage.ClothingCuller.Editor.TreeViews
{
    public class OccludeeConfigurationTreeViewItem : TreeViewItem
    {
        public readonly OccludeeConfiguration OccludeeConfiguration;
        public readonly bool IsNew;
        public readonly bool? IsValid;

        #region Constructor

        public OccludeeConfigurationTreeViewItem(int id, int depth, string displayName, OccludeeConfiguration occludeeConfiguration, bool isNew, bool? isValid) : base(id, depth, displayName)
        {
            OccludeeConfiguration = occludeeConfiguration;
            IsNew = isNew;
            IsValid = isValid;
        }

        #endregion

        #region Override Methods

        public override string displayName
        {
            get
            {
                string name;

                if (OccludeeConfiguration.Prefab != null)
                {
                    name = OccludeeConfiguration.Prefab.name;
                }
                else
                {
                    name = $"{OccludeeConfiguration.name} <color=red>(Missing Prefab)</color>";
                }

                return name;
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
