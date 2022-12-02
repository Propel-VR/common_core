using Salvage.ClothingCuller.Configuration;
using UnityEditor;

namespace Salvage.ClothingCuller.Editor.Configuration
{
    public class ClothingCullerSettingsProvider : SettingsProvider
    {
        private ClothingCullerConfiguration clothingCullerConfiguration;
        private ClothingCullerConfiguration selectedClothingCullerConfiguration;

        #region Constructor

        public ClothingCullerSettingsProvider(ClothingCullerConfiguration clothingCullerConfiguration, string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
        {
            this.clothingCullerConfiguration = clothingCullerConfiguration;
            selectedClothingCullerConfiguration = clothingCullerConfiguration;
        }

        #endregion

        #region Override Methods

        public override void OnGUI(string searchContext)
        {
            EditorGUIUtility.labelWidth = 300f;

            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            selectedClothingCullerConfiguration = EditorGUILayout.ObjectField("Selected configuration", selectedClothingCullerConfiguration, typeof(ClothingCullerConfiguration), false) as ClothingCullerConfiguration;
            if (EditorGUI.EndChangeCheck())
            {
                onSelectedClothingCullerConfigurationChanged();
            }
            EditorGUI.BeginChangeCheck();
            clothingCullerConfiguration.IsModularClothingWorkflowEnabled = EditorGUILayout.Toggle("Modular clothing workflow", clothingCullerConfiguration.IsModularClothingWorkflowEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                onIsModularClothingWorkflowEnabledChanged(clothingCullerConfiguration.IsModularClothingWorkflowEnabled);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Painting scene", EditorStyles.boldLabel);
            ClothingCullerConfiguration.PersistentValues.ViewMeshesAsSkinned = EditorGUILayout.Toggle("View meshes as skinned", ClothingCullerConfiguration.PersistentValues.ViewMeshesAsSkinned);
            OcclusionDataViewerConfiguration.PersistentValues.DefaultPokeThroughRaycastDistance = EditorGUILayout.FloatField("Default poke-through raycast distance", OcclusionDataViewerConfiguration.PersistentValues.DefaultPokeThroughRaycastDistance);
            OcclusionDataViewerConfiguration.PersistentValues.DefaultAmountOfRaycastsPerVertex = EditorGUILayout.IntField("Default raycasts per vertex", OcclusionDataViewerConfiguration.PersistentValues.DefaultAmountOfRaycastsPerVertex);
        }

        #endregion

        #region Private Methods

        private void onSelectedClothingCullerConfigurationChanged()
        {
            if (selectedClothingCullerConfiguration != null)
            {
                clothingCullerConfiguration = selectedClothingCullerConfiguration;

                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(clothingCullerConfiguration, out string guid, out long localId))
                {
                    ClothingCullerConfiguration.PersistentValues.SelectedConfigurationGUID = guid;
                }
            }
            else
            {
                selectedClothingCullerConfiguration = clothingCullerConfiguration;
            }
        }

        private void onIsModularClothingWorkflowEnabledChanged(bool isModularClothingWorkflowEnabled)
        {
            foreach (OccludeeCategory occludeeCategory in clothingCullerConfiguration.OccludeeCategories)
            {
                foreach (OccludeeConfiguration occludee in occludeeCategory.Occludees)
                {
                    if (isModularClothingWorkflowEnabled)
                    {
                        occludee.CreateSkinnedMeshData();
                    }
                    else
                    {
                        occludee.ClearSkinnedMeshData();
                    }

                    EditorUtility.SetDirty(occludee);
                }
            }

            EditorUtility.SetDirty(clothingCullerConfiguration);
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region Public Methods

        [SettingsProvider]
        public static SettingsProvider CreateClothingCullerSettingsProvider()
        {
            var clothingCullerConfiguration = ClothingCullerConfiguration.Find();
            if (clothingCullerConfiguration == null)
            {
                return null;
            }

            return new ClothingCullerSettingsProvider(clothingCullerConfiguration, "Preferences/Clothing Culler");
        }

        #endregion

    }
}
