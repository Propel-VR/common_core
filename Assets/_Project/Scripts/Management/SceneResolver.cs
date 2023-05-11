using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LM.TaskManagement;
using CommonCoreScripts.SceneManagement;
using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;
using System.Linq;
using Unity.VectorGraphics;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LM.Management
{

    public class SceneResolver : SerializedMonoBehaviour
    {
        #region Supplementary Types

        /// <summary>
        /// Defines the scenes required to load a module.
        /// </summary>
        [Serializable]
        [HideReferenceObjectPicker]
        public class ModuleScenes
        {
            [HideReferenceObjectPicker, LabelWidth(100)]
            public List<SceneRef> scenes;

            [SerializeField]
            [GUIColor(.9f, .9f, .9f), DictionaryDrawerSettings(KeyLabel = "SubModule", ValueLabel = "Scene Resolution", DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
            public Dictionary<TaskSubModule, SubModuleScenes> subModuleSpecificScenes = new();
        }

        /// <summary>
        /// Defines the scenes required to load a submodule.
        /// </summary>
        [Serializable]
        [HideReferenceObjectPicker]
        public class SubModuleScenes
        {
            [HideReferenceObjectPicker, LabelWidth(150)]
            public List<SceneRef> scenes;

            //public List<TaskScenes> taskSpecificScenes;
            [GUIColor(.8f, .8f, .8f), DictionaryDrawerSettings(KeyLabel = "Task", ValueLabel = "Scene Resolution", DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
            public Dictionary<Task, TaskScenes> taskSpecificScenes = new();
        }

        /// <summary>
        /// Defines the scenes required to load a task.
        /// </summary>
        [Serializable]
        [HideReferenceObjectPicker]
        public class TaskScenes
        {
            [HideReferenceObjectPicker, LabelWidth(200)]
            public List<SceneRef> scenes;
        }

        #endregion

        #region Serialized Fields

        [TitleGroup("Locations")]
        [SerializeField]
        Dictionary<Location, SceneRef> _locationScenes = new();

        [TitleGroup("Weather")]
        [SerializeField]
        Dictionary<Weather, SceneRef> _weatherScenes = new();

        [TitleGroup("Time of Day")]
        [SerializeField]
        Dictionary<TimeOfDay, SceneRef> _timeScenes = new();

        [TitleGroup("Tasks & Modules")]
        [SerializeField]
        List<SceneRef> _baseScenes = new();

        [SerializeField]
        [DictionaryDrawerSettings(KeyLabel = "Module", ValueLabel = "Scene Resolution", DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        Dictionary<TaskModule, ModuleScenes> _moduleScenes = new();

        [TitleGroup("Lighting")]
        [SerializeField]
        [FolderPath]
        string _lightingScenesFolder;

        #endregion

        public Level ResolveConfiguration(Configuration configuration)
        {
            Level level = ScriptableObject.CreateInstance<Level>();

            // add location scene
            if (_locationScenes.ContainsKey(configuration.location))
                level.scenes.Add(_locationScenes[configuration.location]);
            else
                Debug.LogError($"[SceneResolver]: Could not resolve configuration! No scene found for given location '{configuration.location}'.");

            // add weather scene
            if (_weatherScenes.ContainsKey(configuration.weather))
                level.scenes.Add(_weatherScenes[configuration.weather]);
            else
                Debug.LogError($"[SceneResolver]: Could not resolve configuration! No scene found for given weather '{configuration.weather}'.");

            // add time scene
            if (_timeScenes.ContainsKey(configuration.timeOfDay))
                level.scenes.Add(_timeScenes[configuration.timeOfDay]);
            else
                Debug.LogError($"[SceneResolver]: Could not resolve configuration! No scene found for given time of day '{configuration.timeOfDay}'.");

            // add base scenes that get added to any configuration
            if (_baseScenes != null)
                level.scenes.AddRange(_baseScenes);

            // add module specific scenes
            if (configuration.module != null && _moduleScenes.ContainsKey(configuration.module))
                level.scenes.AddRange(_moduleScenes[configuration.module].scenes);
            else
                Debug.LogError($"[SceneResolver]: Could not resolve configuration! No scenes specified for given module '{configuration.module.Name}'.");

            // add submodule specific scenes (if there are any)
            Debug.Assert(configuration.subModules != null, "[SceneResolver]: Could not resolve configuration! Configuration does not include any submodules.");

            foreach (TaskSubModule subModule in configuration.subModules)
            {
                if (_moduleScenes[configuration.module].subModuleSpecificScenes.ContainsKey(subModule))
                {
                    level.scenes.AddRange(_moduleScenes[configuration.module].subModuleSpecificScenes[subModule].scenes);

                    // add task specific scenes (if there are any)
                    foreach (Task task in _moduleScenes[configuration.module].subModuleSpecificScenes[subModule].taskSpecificScenes.Keys)
                    {
                        if (configuration.tasks.Contains(task))
                            level.scenes.AddRange(_moduleScenes[configuration.module].subModuleSpecificScenes[subModule].taskSpecificScenes[task].scenes);
                    }
                }
            }

            // add lighting scene
            SceneRef lightingScene = new SceneRef(GetLightingSceneName(configuration), GetLightingScenePath(configuration));
            level.scenes.Add(lightingScene);
            level.activeScene = lightingScene;

            return level;
        }

        #region Private Methods

        string GetLightingScenePath(Configuration configuration)
        {
            return $"{_lightingScenesFolder}/{GetLightingSceneName(configuration)}.unity";
        }

        string GetLightingSceneName(Configuration configuration)
        {
            return GetLightingSceneName(configuration.module, configuration.location, configuration.weather, configuration.timeOfDay);
        }

        string GetLightingSceneName(TaskModule module, Location location, Weather weather, TimeOfDay timeOfDay)
        {
            return $"{module.Name} - {location} - {timeOfDay} - {weather}";
        }

        #endregion

        #region Editor
#if UNITY_EDITOR

        [PropertySpace, Button]
        private void GenerateLightingScenes()
        {
            int count = 0;

            foreach (Location location in _locationScenes.Keys)
            {
                foreach (Weather weather in _weatherScenes.Keys)
                {
                    foreach (TimeOfDay timeOfDay in _timeScenes.Keys)
                    {
                        foreach (TaskModule module in _moduleScenes.Keys)
                        {
                            if (GenerateLightingScene(module, location, weather, timeOfDay))
                                count++;
                        }
                    }
                }
            }

            Debug.Log($"[SceneResolver]: Successfully generated {count} scenes!");
        }

        bool GenerateLightingScene(TaskModule module, Location location, Weather weather, TimeOfDay timeOfDay)
        {
            string sceneName = GetLightingSceneName(module, location, weather, timeOfDay);
            string[] guids = AssetDatabase.FindAssets(sceneName + "t:Scene", new[] { _lightingScenesFolder });

            if (guids.Length > 1)
                Debug.LogWarning($"[SceneResolver]: Found multiple lighting scenes that resolve to '{sceneName}'. The one that will be used is going to be ambiguous.");

            if (guids.Length == 0)
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

                if (scene == null)
                {
                    Debug.LogWarning($"[SceneResolver]: Failed to create scene with name '{sceneName}'.");
                    return false;
                }

                if (!EditorSceneManager.SaveScene(scene, $"{_lightingScenesFolder}/{sceneName}.unity"))
                {
                    Debug.LogWarning($"[SceneResolver]: Failed to save newly generated scene with name '{sceneName}'.");
                    return false;
                }

                if (!EditorSceneManager.CloseScene(scene, true))
                {
                    Debug.LogWarning($"[SceneResolver]: Failed to close newly generated scene with name '{sceneName}'.");
                    return false;
                }

                return true;
            }

            return false;
        }

#endif
            #endregion
        }

}
