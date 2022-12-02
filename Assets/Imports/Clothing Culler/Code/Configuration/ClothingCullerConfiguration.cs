using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Salvage.ClothingCuller.Configuration
{
    public class ClothingCullerConfiguration : ScriptableObject
    {
        #region SerializeFields

#if UNITY_EDITOR

        [field: SerializeField]
        public List<OccludeeCategory> OccludeeCategories { get; private set; }

#endif

        [HideInInspector]
        public bool IsModularClothingWorkflowEnabled;

        #endregion

        #region Constructor

        private ClothingCullerConfiguration()
        {

        }

        #endregion

        #region Private Methods

#if UNITY_EDITOR

        private static string getExecutionPathRelativeToAssetsFolder([CallerFilePath] string sourceFilePath = "")
        {
            string path = Path.GetDirectoryName(sourceFilePath);

            int startIndex = path.IndexOf("Assets");
            int endIndex = path.Length - startIndex;

            return path.Substring(startIndex, endIndex);
        }

        private static ClothingCullerConfiguration create(string targetFolderPath)
        {
            ClothingCullerConfiguration instance = CreateInstance<ClothingCullerConfiguration>();
            instance.OccludeeCategories = new List<OccludeeCategory>();

            Directory.CreateDirectory(targetFolderPath);

            string targetAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{targetFolderPath}/{nameof(ClothingCullerConfiguration)}.asset");
            AssetDatabase.CreateAsset(instance, targetAssetPath);
            AssetDatabase.SaveAssets();

            return instance;
        }

#endif

        #endregion

        #region Public Methods

#if UNITY_EDITOR

        public static ClothingCullerConfiguration Find()
        {
            if (!string.IsNullOrEmpty(PersistentValues.SelectedConfigurationGUID))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(PersistentValues.SelectedConfigurationGUID);

                if (!string.IsNullOrEmpty(assetPath))
                {
                    return AssetDatabase.LoadAssetAtPath<ClothingCullerConfiguration>(assetPath);
                }

                PersistentValues.SelectedConfigurationGUID = string.Empty;
            }

            string guid = AssetDatabase.FindAssets($"t:{typeof(ClothingCullerConfiguration)}").FirstOrDefault();

            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<ClothingCullerConfiguration>(AssetDatabase.GUIDToAssetPath(guid));
        }

        public static ClothingCullerConfiguration FindOrCreate()
        {
            ClothingCullerConfiguration clothingCullerConfiguration = Find();
            if (clothingCullerConfiguration != null)
            {
                return clothingCullerConfiguration;
            }

            string configFolderPath = Path.Combine(getExecutionPathRelativeToAssetsFolder(), "../../Configuration");
            Debug.Log($"Unable to find existing configuration, creating a new one at '{Path.GetFullPath(configFolderPath)}'.");

            return create(configFolderPath);
        }

#endif

        #endregion

        #region Inner Classes

#if UNITY_EDITOR

        public static class PersistentValues
        {
            private const string editorPrefsPrefix = "CC_";

            #region Properties

            public static string SelectedConfigurationGUID
            {
                get
                {
                    return EditorPrefs.GetString($"{editorPrefsPrefix}{nameof(SelectedConfigurationGUID)}", string.Empty);
                }
                set
                {
                    EditorPrefs.SetString($"{editorPrefsPrefix}{nameof(SelectedConfigurationGUID)}", value);
                }
            }

            public static bool ViewMeshesAsSkinned
            {
                get
                {
                    return EditorPrefs.GetBool($"{editorPrefsPrefix}{nameof(ViewMeshesAsSkinned)}", true);
                }
                set
                {
                    EditorPrefs.SetBool($"{editorPrefsPrefix}{nameof(ViewMeshesAsSkinned)}", value);
                }
            }


            #endregion
        }

#endif
        #endregion

    }
}
