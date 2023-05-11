using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using Sirenix.OdinInspector;
using System.Linq;

namespace CommonCoreScripts.SceneManagement
{
    #region Supplementary Types

    /// <summary>
    /// An event that may occur at some point during a transition from one scene to another.
    /// </summary>
    /// <param name="from">The current or previous scene - i.e. the scene being unloaded.</param>
    /// <param name="to">The new scene - i.e. the one being loaded.</param>
    public delegate void LoadSceneEvent(Level from, Level to);

    #endregion

    /// <summary>
    /// Responsible for managing levels and loading scenes.
    /// </summary>
    public class SceneManager : MonoBehaviour
    {
        #region Singleton

        static SceneManager s_instance = null;

        /// <summary>
        /// The SceneManager instance (will automatically be created on first reference).
        /// </summary>
        public static SceneManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = Instantiate(new GameObject()).AddComponent<SceneManager>();
                    s_instance.gameObject.name = "SceneManager";
                }

                return s_instance;
            }
        }

        #endregion

        #region Serialized Fields

        [InfoBox("Note: These fields will only be set if starting from this scene as the SceneManager will be created on the fly the first time it is referenced (must also ensure nothing in this scene tries to reference it before the Start method).")]

        [SerializeField]
        [Tooltip("Whether to load levels synchronously or asynchronously (sync support not yet tested and more just for debugging purposes).")]
        bool _loadLevelsAsync = true;

        [Header("Debug")]
        [SerializeField]
        [Tooltip("Log events like which scenes get loaded/unloaded and how long it takes.")]
        bool _logEvents = false;

        [SerializeField]
        [Tooltip("Delay level loading to simulate slower platforms (disabled automatically in builds).")]
        bool _simulateSlowLoadTimes = false;

        [SerializeField]
        [Tooltip("Drag a level into this field to load a level.")]
        Level _loadLevel = null;

        #endregion

        #region Private Fields

        Level _previousLevel;
        Level _currentLevel;
        Level _nextLevel;
        bool _isLoadingLevel;
        bool _isLevelReady;
        float _curProgress;

        List<string> _dontUnloadScenes = new();

        #endregion

        /// <summary>
        /// Whether the current loading level will activate immediately upon completion.
        /// (Set this to false to delay level activation, then set to true when ready to
        /// activate).
        /// </summary>
        public bool CanActivateLevel { get; set; } = true;

        /// <summary>
        /// The current loaded level.
        /// </summary>
        public Level CurrentLevel => _currentLevel;

        /// <summary>
        /// True if currently loading a level.
        /// </summary>
        public bool IsLoadingLevel => _isLoadingLevel;

        /// <summary>
        /// True if currently loading a level and the level is just waiting for activation.
        /// </summary>
        public bool IsLevelReady => _isLevelReady;

        /// <summary>
        /// The progress of the current level loading process, as a value from 0 - 1.
        /// </summary>
        public float CurrentProgress => _curProgress;

        /// <summary>
        /// Fired immediately before loading a new level.
        /// </summary>
        public static event LoadSceneEvent OnBeginLoadLevel;

        /// <summary>
        /// Fired immediately after loading a new level, after Awake and Start have been called 
        /// on all scripts in all scenes for that level.
        /// </summary>
        public static event LoadSceneEvent OnFinishLoadLevel;

        #region MonoBehaviour Methods

        private void Awake()
        {
#if !UNITY_EDITOR
            _simulateSlowLoadTimes = false;
#endif

            if (s_instance != null && s_instance != this)
            {
                Debug.LogWarning("[SceneManager]: Multiple SceneManager's found! Ensure your SceneManager is in your base scene and there are no other SceneManager's present in your project. \n" +
                    "(If you are playing in editor from a scene that isn't your base scene, ignore this warning.)");

                s_instance._loadLevelsAsync = _loadLevelsAsync;
                s_instance._logEvents = _logEvents;
                s_instance._simulateSlowLoadTimes = _simulateSlowLoadTimes;

                Destroy(this);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(s_instance.gameObject);
        }

        #region Editor

        private void OnValidate()
        {
            if (_loadLevel != null)
            {
                LoadLevel(_loadLevel);
                _loadLevel = null;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Additively load a scene asynchronously.
        /// </summary>
        /// <param name="scene">Name of the scene to load</param>
        /// <param name="onFinish">(Optional) event to fire after the scene has loaded.</param>
        /// <returns>A Coroutine which finishes when the scene has finished loading.</returns>
        public Coroutine LoadScene(string scene, Action onFinish = null)
        {
            AsyncOperation asyncLoad = null;

            if (_logEvents)
                Debug.Log($"[SceneManager]: Loading 1 Scene");

            if (!IsSceneLoaded(scene))
            {
                asyncLoad = UnitySceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

                if (_logEvents)
                    Debug.Log($"[SceneManager] ...loading scene '{scene}'");
            }

            if (_logEvents)
                onFinish += () => Debug.Log("[SceneManager] ...done loading scene");

            Coroutine coroutine = StartCoroutine(WaitFor(asyncLoad, onFinish));
            return coroutine;
        }

        /// <summary>
        /// Unload a scene asynchronously.
        /// </summary>
        /// <param name="scene">Name of the scene to unload</param>
        /// <param name="onFinish">(Optional) event to fire after the scene has unloaded.</param>
        /// <returns>A Coroutine which finishes when the scene has finished unloading.</returns>
        public Coroutine UnloadScene(string scene, Action onFinish = null)
        {
            AsyncOperation asyncLoad = null;

            if (_logEvents)
                Debug.Log($"[SceneManager]: Unloading 1 Scene");

            if (IsSceneLoaded(scene))
            {
                asyncLoad = UnitySceneManager.UnloadSceneAsync(scene);

                if (_logEvents)
                    Debug.Log($"[SceneManager] ...unloading scene '{scene}'");
            }
            else if (_logEvents)
            { 
                Debug.Log($"[SceneManager] ...scene '{scene}' not loaded");
            }

            if (_logEvents)
                onFinish += () => Debug.Log("[SceneManager] ...done unloading scene");

            Coroutine coroutine = StartCoroutine(WaitFor(asyncLoad, onFinish));
            return coroutine;
        }

        /// <summary>
        /// Additively load multiple scenes asynchronously.
        /// </summary>
        /// <param name="scenesToLoad">List of scenes to load.</param>
        /// <param name="onFinish">(Optional) event to fire after all the scenes have loaded.</param>
        /// <returns>A Coroutine which finishes when all the scenes have finished loading.</returns>
        public Coroutine LoadScenes(List<string> scenesToLoad, Action onFinish = null)
        {
            List<AsyncOperation> asyncLoads = new();

            if (_logEvents)
                Debug.Log($"[SceneManager]: Loading {scenesToLoad.Count} Scenes");

            foreach (string scene in scenesToLoad)
            {
                if (!IsSceneLoaded(scene))
                {
                    asyncLoads.Add(UnitySceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));

                    if (_logEvents)
                        Debug.Log($"[SceneManager] ...loading '{scene}'");
                }
                else if(_logEvents)
                {
                    Debug.Log($"[SceneManager] ...'{scene}' already loaded");
                }
            }

            if (_logEvents)
                onFinish += () => Debug.Log("[SceneManager] ...done loading scenes");

            return StartCoroutine(WaitForAll(asyncLoads, onFinish));
        }

        /// <summary>
        /// Unload multiple scenes asynchronously.
        /// </summary>
        /// <param name="scenesToUnload">List of scenes to unload.</param>
        /// <param name="onFinish">(Optional) event to fire after all the scenes have unloaded.</param>
        /// <returns>A Coroutine which finishes when all the scenes have finished unloading.</returns>
        public Coroutine UnloadScenes(List<string> scenesToUnload, Action onFinish = null)
        {
            List<AsyncOperation> asyncLoads = new();

            if (_logEvents)
                Debug.Log($"[SceneManager]: Unloading {scenesToUnload.Count} Scenes");

            foreach (string scene in scenesToUnload)
            {
                if (IsSceneLoaded(scene))
                {
                    asyncLoads.Add(UnitySceneManager.UnloadSceneAsync(scene));

                    if (_logEvents)
                        Debug.Log($"[SceneManager] ...unloading '{scene}'");
                }
                else if (_logEvents)
                {
                    Debug.Log($"[SceneManager] ...'{scene}' not loaded");
                }
            }

            if (_logEvents)
                onFinish += () => Debug.Log("[SceneManager] ...done unloading scenes");

            return StartCoroutine(WaitForAll(asyncLoads, onFinish));
        }

        /// <summary>
        /// Unload the current level and load a new one.
        /// </summary>
        /// <param name="level">Level to load.</param>
        /// <param name="onFinish">(Optional) Event to fire after the new level has finished loading and is active.</param>
        /// <returns></returns>
        public Coroutine LoadLevel(Level level, Action onFinish = null)
        {
            if (level.scenes.Count == 0)
            {
                Debug.LogError("[SceneManager]: Level must define at least one scene to load");
                return null;
            }

            if (level.activeScene == null)
            {
                Debug.LogError("[SceneManager]: Level must define an active scene");
                return null;
            }

            if (_isLoadingLevel)
            {
                Debug.LogWarning($"[SceneManager]: Already loading level: {_nextLevel.name} - Cannot load new level: {level.name}");
                return null;
            }

            return StartCoroutine(LoadLevelRoutine(level, onFinish));
        }

        /// <summary>
        /// Is this scene currently loaded?
        /// </summary>
        /// <param name="scene">Scene to check if loaded.</param>
        /// <returns>True if the scen is currently loaded, false otherwise</returns>
        public bool IsSceneLoaded(string scene)
        {
            for (int i = 0; i < UnitySceneManager.sceneCount; i++)
            {
                if (UnitySceneManager.GetSceneAt(i).name == scene)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Convenience method to set the active scene by scene name. 
        /// 
        /// The active scene should hold any lighting data for the current level.
        /// </summary>
        /// <param name="scene">Scene to set as the active scene.</param>
        public void SetActiveScene(string scene)
        {
            UnitySceneManager.SetActiveScene(UnitySceneManager.GetSceneByName(scene));
        }

        /// <summary>
        /// Tell the SceneManager not to unload this scene when loading levels.
        /// 
        /// By default, all scenes not needed by a level are unloaded on calls to 
        /// LoadLevel. Call this method to ensure that a scene does not get unloaded.
        /// </summary>
        /// <param name="scene">The name of a scene that should never be unloaded as a result of calls to LoadLevel.</param>
        public void DontUnloadScene(string scene)
        {
            _dontUnloadScenes.Add(scene);
        }

        #region Private Methods

        /// <summary>
        /// Sets any varaibles, and fires any events for loading levels, and
        /// passes on the actual level loading to the requried method.
        /// </summary>
        /// <param name="level">Level to load.</param>
        /// <param name="onFinish">(Optional) Event to fire after the new level has finished loading and is active.</param>
        /// <returns></returns>
        IEnumerator LoadLevelRoutine(Level level, Action onFinish = null)
        {
            _nextLevel = level;
            _isLevelReady = false;
            _isLoadingLevel = true;

            if (_logEvents)
                Debug.Log($"[SceneManager]: Loading level: {level.name}");

            // fire events
            OnBeginLoadLevel?.Invoke(_currentLevel, level);

            if (_loadLevelsAsync)
                yield return LoadLevelAsyncRoutine(level);
            else
                yield return LoadLevelSyncRoutine(level);

            _previousLevel = _currentLevel;
            _currentLevel = level;
            _isLoadingLevel = false;

            // fire events
            OnFinishLoadLevel?.Invoke(_previousLevel, _currentLevel);
            onFinish?.Invoke();

            if (_logEvents)
                Debug.Log("[SceneManager]: ...loading complete!");

            // reset activateion flag (probably not necessary but just in case)
            CanActivateLevel = true;
        }

        /// <summary>
        /// Load a level synchronoously. (Not tested in a while)
        /// </summary>
        /// <param name="level">Level to load.</param>
        /// <returns></returns>
        IEnumerator LoadLevelSyncRoutine(Level level)
        {
            UnitySceneManager.LoadScene(level.activeScene.name);

            if (_logEvents)
                Debug.Log($"[SceneManager]: ...loading '{level.activeScene.name}'");

            if (_simulateSlowLoadTimes)
                yield return new WaitForSeconds(2f);

            int i = 0;

            foreach (SceneRef scene in level.scenes)
            {
                if (scene.name != level.activeScene.name)
                {
                    UnitySceneManager.LoadScene(scene.name, LoadSceneMode.Additive);

                    _curProgress = (float)i / level.scenes.Count;

                    if (_logEvents)
                        Debug.Log($"[SceneManager]: ...loading '{scene}'");

                if (_simulateSlowLoadTimes)
                    yield return new WaitForSeconds(2f);
                }
            }

            yield return null;
        }

        /// <summary>
        /// Load a level asynchronously.
        /// </summary>
        /// <param name="level">Level to load.</param>
        /// <returns></returns>
        IEnumerator LoadLevelAsyncRoutine(Level level)
        {
            float loadLevelStartTime = Time.unscaledTime;

            Application.backgroundLoadingPriority = ThreadPriority.High;

            List<AsyncOperation> asyncLoads = new();

            // unload current scenes
            for (int i = 0; i < UnitySceneManager.sceneCount; i++)
            {
                Scene scene = UnitySceneManager.GetSceneAt(i);

                if (!level.scenes.Exists(s => s.name == scene.name) && !_dontUnloadScenes.Contains(scene.name))
                {
                    asyncLoads.Add(UnitySceneManager.UnloadSceneAsync(scene.name));

                    if (_logEvents)
                        Debug.Log($"[SceneManager]: ...unloading '{scene.name}'");
                }
            }

            // wait... 
            while (asyncLoads.Exists(op => op != null && op.isDone))
                yield return null;

            asyncLoads.Clear();

            // load new scenes
            foreach (SceneRef scene in level.scenes)
            {
                if (!IsSceneLoaded(scene.name))
                {
                    AsyncOperation asyncLoad = UnitySceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
                    asyncLoad.allowSceneActivation = CanActivateLevel;
                    asyncLoads.Add(asyncLoad);

                    if (_logEvents)
                        Debug.Log($"[SceneManager]: ...loading '{scene}'");
                }
            }

            if (_simulateSlowLoadTimes)
                yield return new WaitForSeconds(10f);

            // wait... (progress stops at 0.9 when scene is ready to be activated)
            while (asyncLoads.Exists(op => op != null && op.progress < 0.9f))
            {
                // progress = sum(op.progress) / (# of ops) divided by 0.9 since 0.9 is the end of the actual loading phase
                //_curProgress = asyncLoads.Aggregate(0f, (value, op) => value + op.progress) / ((float)asyncLoads.Count * 0.9f);

                // progress = min(op => op.progress)
                _curProgress = asyncLoads.Aggregate(1f, (value, op) => Mathf.Min(value, op.progress));

                yield return null;
            }

            //Resources.UnloadUnusedAssets();

            Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;

            _isLevelReady = true;

            if (_logEvents)
                Debug.Log($"[SceneManager]: ...time to load level: '{Time.unscaledTime - loadLevelStartTime}'");

            // wait for activation
            while (!CanActivateLevel)
                yield return null;

            float activateLevelStartTime = Time.unscaledTime;

            foreach (var op in asyncLoads)
                op.allowSceneActivation = true;

            while (!asyncLoads.TrueForAll(op => op != null && op.isDone))
                yield return null;

            _curProgress = 1f;

            yield return null;

            SetActiveScene(level.activeScene.name);

            yield return null;

            if (_logEvents)
                Debug.Log($"[SceneManager]: ...time to activate level: '{Time.unscaledTime - activateLevelStartTime}'");
        }
        
        /// <summary>
        /// Wait for an async operation (such as loading a scene) to finish.
        /// </summary>
        IEnumerator WaitFor(AsyncOperation asyncOperation, Action onEndWait = null, Action<float> onUpdate = null)
        {
            while (asyncOperation != null && !asyncOperation.isDone)
            {
                onUpdate?.Invoke(asyncOperation.progress / 0.9f);

                yield return null;
            }

            onUpdate?.Invoke(1f);
            onEndWait?.Invoke();
        }

        /// <summary>
        /// Wait for a list of async operations to finish.
        /// </summary>
        IEnumerator WaitForAll(List<AsyncOperation> asyncOperations, Action onEndWait = null, Action<float> onUpdate = null)
        {
            while (!asyncOperations.TrueForAll(op => op == null || op.isDone))
            {
                float progress = asyncOperations.Aggregate(0f, (value, op) => value + op.progress) / ((float)asyncOperations.Count * 0.9f);
                onUpdate?.Invoke(progress);

                yield return null;
            }

            onUpdate?.Invoke(1f);
            onEndWait?.Invoke();
        }

#endregion
    }

}

