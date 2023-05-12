using CommonCoreScripts.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LM.TaskManagement;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace LM.Management
{

    public class Configurator : MonoBehaviour
    {
        #region Singleton

        public static Configurator Instance { get; private set; }

        #endregion

        #region Serialized and Private Fields

        [SerializeField]
        SceneResolver _sceneResolver;

        [Space, ShowInInspector, HideReferenceObjectPicker]
        Configuration _configuration;

        [ShowInInspector, OnValueChanged("LoadPreset")]
        [Tooltip("Drag a preset into this field to load it into the configuration.")]
        Preset _loadPreset;

        #endregion

        public Configuration Configuration { get => _configuration; set => _configuration = value; }

        #region Monobehaviour Methods

        private void Awake()
        {
            Debug.Assert(Instance == null, "[Configurator]: Found multiple Configurators. There should only be one Configurator, and it should be in the base scene.");
            Instance = this;
        }

        #endregion

        public void LoadPreset(Preset preset)
        {
            _configuration = preset.Configuration;

            // reset the preset to null in case we're loading from the inspector
            _loadPreset = null;
        }

        public void LoadConfiguration()
        {
            // determine scenes to load based on configuration
            Level level = _sceneResolver.ResolveConfiguration(_configuration);

            // setup loading screen with info from configuration


            // begin loading new scenes
            TransitionManager.Instance.TransitionToLevel(level, false);
        }

        #region Editor
#if UNITY_EDITOR

        [Button("Load Configuration")]
        void LoadConfigurationInEditor()
        {
            if (_sceneResolver == null)
            {
                Debug.LogWarning("[Configurator]: Reference to a SceneResolver is missing but required in order to load a configuration.");
                return;
            }

            if (Application.isPlaying)
            {
                LoadConfiguration();
                return;
            }

            Level level = _sceneResolver.ResolveConfiguration(_configuration);

            EditorSceneManager.OpenScene(level.activeScene.path);

            foreach (SceneRef scene in level.scenes)
                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
        }

#endif
        #endregion
    }

}
