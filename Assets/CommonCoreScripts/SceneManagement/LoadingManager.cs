using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Responsible for handling the transition scene during loading transitions.
    /// 
    /// Controls the loading bar.
    /// </summary>
    public class LoadingManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// The LoadingManager instance, if it exists (should exist in the loading scene).
        /// </summary>
        public static LoadingManager Instance { get; private set; }

        #endregion

        #region Serialized and Private Fields

        [SerializeField]
        [Tooltip("The parent object of everything that should be enabled/disabled when loading.")]
        GameObject _loadingSceneGameObject;

        [SerializeField]
        [Tooltip("The camera to use while loading.")]
        Camera _loadingCamera;

        [SerializeField]
        [Tooltip("The Image component (whose 'type' is set to 'filled') that displays the loading progress.")]
        Image _loadingBarFill;

        [Header("Events")]
        [SerializeField]
        [Tooltip("Will fire whenever the loading sequence is started.")]
        UnityEvent _onBeginLoading;

        [SerializeField]
        [Tooltip("Will fire whenever the loading sequence is stopped.")]
        UnityEvent _onFinishLoading;

        bool _isLoading;

        #endregion

        /// <summary>
        /// True if currently in the loading sequence.
        /// </summary>
        public bool IsLoading => _isLoading;

        #region Monobehaviour Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[LoadingManager]: Multiple LoadingManager's active simultaneously.");
                Destroy(this);
                return;
            }

            Instance = this;

            _loadingSceneGameObject.SetActive(false);
            _loadingCamera.enabled = false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        #endregion

        /// <summary>
        /// Start loading. This will disable the player camera and enable the loading scene (which 
        /// includes its own camera). This does not call any scene load/unload methods.
        /// </summary>
        public void BeginLoading()
        {
            if (_isLoading)
                return;

            StartCoroutine(BeginLoadingRoutine()); 
        }

        /// <summary>
        /// Stop loading. This will re-enable the player camera and disable the loading 
        /// scene. This will not stop any actual scene loading from taking place.
        /// </summary>
        public void FinishLoading()
        {
            if (!_isLoading)
                return;

            Debug.Log("[LoadingManager]: ...finish loading");

            StopAllCoroutines();

            // disable the loading scene and swap back to player camera
            _loadingSceneGameObject.SetActive(false);
            _loadingCamera.enabled = false;

            if (Player.Get())
                Player.Get().Camera.enabled = true;

            _isLoading = false;

            // fire events
            _onFinishLoading?.Invoke();
        }

        #region Private Methods

        IEnumerator BeginLoadingRoutine()
        {
            Debug.Log("[LoadingManager]: Begin loading...");

            _isLoading = true;

            // fire events
            _onBeginLoading?.Invoke();

            // set loading amount to zero
            if (_loadingBarFill)
                _loadingBarFill.fillAmount = 0f;

            // enable loading scene + loading rig
            _loadingSceneGameObject.SetActive(true);

            // wait one frame between setting tracked pose driver active and setting camera active 
            // so that position is updated before rendering to camera (or something like that - this 
            // removes that single frame "lag" that occurs otherwise when switching rigs)
            yield return null;

            // swap cameras/rigs
            if (Player.Get())
                Player.Get().Camera.enabled = false;
            else
                StartCoroutine(WaitForPlayerAndSetActive(false));

            _loadingCamera.enabled = true;

            StartCoroutine(LoadingRoutine());
        }

        /// <summary>
        /// Responsible for runnign the loading sequence between calls to BeginLoading and 
        /// FinishLoading. This routine controls the loading bar.
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadingRoutine()
        {
            while (true)
            {
                _loadingBarFill.fillAmount = SceneManager.Instance.CurrentProgress;

                yield return null;
            }
        }

        /// <summary>
        /// Starting this routine will wait until the player can be found and then as soon as
        /// it's found, enable or disable the its camera.
        /// </summary>
        /// <param name="value">Whether to enable or disalbe the camera.</param>
        IEnumerator WaitForPlayerAndSetActive(bool value)
        {
            while (!Player.Get())
                yield return null;

            Player.Get().Camera.enabled = value;
        }

        #endregion
    }

}
