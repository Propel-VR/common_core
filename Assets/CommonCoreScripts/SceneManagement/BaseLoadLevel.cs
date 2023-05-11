using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Responsible for loading the first level in the application. Similar to
    /// <see cref="LoadLevel"/> except this script is meant to be placed in the
    /// 'Base' scene and is aware of the splash scene.
    /// 
    /// Also defines the loading and splash scenes so that they do not get unloaded
    /// by the SceneManager.
    /// </summary>
    public class BaseLoadLevel : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The first level to load on application start.")]
        Level _firstLevel;

        [SerializeField]
        [Tooltip("(Optional) A scene that contains a LoadingManager to be used for level loading sequences.")]
        SceneRef _loadingScene;

        [SerializeField]
        [Tooltip("(Optional) A scene that contains a SplashManager to be used for the splash sequence.")]
        SceneRef _splashScene;

        [SerializeField]
        [Tooltip("(Optional) Delay fading in the screen after the first level has finished loading. (Can be useed to keep the screen black while objects are activating.)")]
        float _fadeInDelaySeconds = 0f;

        bool _isSplashFinished = false;
        bool _isLoadingSceneReady = false;

        private void Start()
        {
            Debug.Log("[BaseLoadLevel]: Base scene has loaded");

            // only load first level if starting from the base scene
            if (SceneManager.Instance.CurrentLevel != null)
                return;

            if (_firstLevel == null)
            {
                Debug.LogWarning("[BaseLoadLevel]: You must define an initial level in the BaseLoadLevel script.");
                return;
            }

            // instant fade out (sets fade canvas to black)
            ScreenFader.Instance.FadeOut(0f);

            // if waiting for splash scene, then SplashManager Instance will be null when it has finished so dont need to wait in that case
            if (_splashScene != null && SplashManager.Instance != null)
            {
                // delay level activation until splash sequence is finished
                SceneManager.Instance.CanActivateLevel = false;
                SplashManager.Instance.OnEndSplash += OnEndSplash;
                SceneManager.Instance.DontUnloadScene(_splashScene.name);
            }
            else
            {
                _isSplashFinished = true;
            }

            // ready the loading scene
            if (_loadingScene != null)
            {
                SceneManager.Instance.DontUnloadScene(_loadingScene.name);
                SceneManager.Instance.LoadScene(_loadingScene.name, OnFinishLoadingLoadingScene);
            }

            //begin loading the first level
            SceneManager.Instance.LoadLevel(_firstLevel, OnFinishLoadingLevel);
        }

        /// <summary>
        /// Fired after the loading scene has loaded. Will begin the loading sequence if
        /// the splash sequence is already finished.
        /// </summary>
        void OnFinishLoadingLoadingScene()
        {
            Debug.Log("[BaseLoadLevel]: Loading scene has been loaded");

            if (LoadingManager.Instance == null)
                Debug.LogWarning("[BaseLoadLevel]: Could not find LoadingManager in loading scene.");

            if (_isSplashFinished && !SceneManager.Instance.IsLevelReady)
                LoadingManager.Instance.BeginLoading();
            else
                _isLoadingSceneReady = true;
        }

        /// <summary>
        /// Fired after the splash sequence has finished. Will begin the loading sequence if
        /// the loading scene has already loaded.
        /// </summary>
        void OnEndSplash()
        {
            Debug.Log("[BaseLoadLevel]: Splash scene has finished");

            SceneManager.Instance.CanActivateLevel = true;

            if (_isLoadingSceneReady && !SceneManager.Instance.IsLevelReady)
                LoadingManager.Instance.BeginLoading();
            else
                _isSplashFinished = true;
        }

        /// <summary>
        /// Fired after the first level ahas finished loading. Will end the loading sequence.
        /// </summary>
        void OnFinishLoadingLevel()
        {
            StartCoroutine(OnFinishLoadingLevelRoutine());
        }

        IEnumerator OnFinishLoadingLevelRoutine()
        {
            Debug.Log("[BaseLoadLevel]: Level has loaded");

            if (LoadingManager.Instance != null)
                LoadingManager.Instance.FinishLoading();

            yield return new WaitForSeconds(_fadeInDelaySeconds);

            Debug.Log("[BaseLoadLevel]: Fading in...");
            ScreenFader.Instance.FadeIn();
        }
    }

}
