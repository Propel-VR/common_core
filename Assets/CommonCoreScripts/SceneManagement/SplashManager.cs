using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Responsible for loading the first scene after the splash scene and
    /// telling it when the splash sequence is finished.
    /// </summary>
    public class SplashManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// The SplashManager instance, if it exists (should exist in the splash scene, which
        /// will be unloaded once the splash sequence has finished).
        /// </summary>
        public static SplashManager Instance { get; private set; }

        #endregion

        #region Serialized and Private Fields

        [SerializeField]
        SceneRef _nextScene;

        #endregion

        /// <summary>
        /// Event fired when the splash sequence has ended.
        /// </summary>
        public event Action OnEndSplash;

        #region Monobehaviour Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[SplashManager]: Multiple SplashManager's active simultaneously.");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Start()
        {
            if (_nextScene != null)
                UnitySceneManager.LoadSceneAsync(_nextScene.name, LoadSceneMode.Additive);
        }

        #endregion

        /// <summary>
        /// Callback to be fired at the end of the splash timeline. Must be assigned to a 
        /// SignalReceiver event if actually using a Timeline.
        /// </summary>
        public void OnEndTimeline()
        {
            OnEndSplash?.Invoke();

            UnitySceneManager.UnloadSceneAsync(UnitySceneManager.GetActiveScene());
        }
    }

}
