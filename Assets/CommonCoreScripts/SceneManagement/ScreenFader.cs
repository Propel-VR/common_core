using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.OdinInspector;


namespace CommonCoreScripts.SceneManagement
{
    /// <summary>
    /// Responsible for fading the screen in or out.
    /// 
    /// Based off of BNG.ScreenFader but more generic. Createa a canvas in 
    /// front of the main camera.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        #region Singleton

        static ScreenFader s_instance = null;

        public static ScreenFader Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = Instantiate(new GameObject()).AddComponent<ScreenFader>();
                    s_instance.gameObject.name = "ScreenFader";
                    DontDestroyOnLoad(s_instance.gameObject);
                }

                return s_instance;
            }
        }

        #endregion

        #region Serialized and Private Fields

        // for debugging
        [SerializeField]
        [Tooltip("Material for the canvas used to fade the screen to black")]
        Material _canvasMat;

        bool IsFadedIn => _canvasGroup != null && _canvasGroup.alpha < 0.5f;

        [Header("Debug")]
        [SerializeField]
        [HideIf("IsFadedIn", false)]
        bool _fadeIn;

        [Header("Debug")]
        [SerializeField]
        [ShowIf("IsFadedIn", false)]
        bool _fadeOut;

        float _initialFadeLevel = 0f;
        GameObject _fadeObject = null;
        CanvasGroup _canvasGroup;
        Coroutine _fadeRoutine;

        const string c_fadeObjectName = "ScreenFader";

        #endregion

        #region Monobehaviour Methods

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(this);
                return;
            }

            s_instance = this;
        }

        IEnumerator Start()
        {
            yield return new WaitUntil(() => Camera.main != null);

            Init();
        }

        private void OnValidate()
        {
            if (_fadeIn)
            {
                FadeIn();
                _fadeIn = false;
            }

            if (_fadeOut)
            {
                FadeOut();
                _fadeOut = false;
            }
        }

        #endregion

        /// <summary>
        /// Create the fade canvas and set its properties (can only be done
        /// once the main camera has been loaded).
        /// </summary>
        void Init()
        {
            if (_fadeObject != null)
                return;

            _fadeObject = new GameObject();
            _fadeObject.transform.SetParent(Camera.main.transform);
            _fadeObject.transform.localPosition = new Vector3(0, 0, Camera.main.nearClipPlane + 0.02f);
            _fadeObject.transform.localEulerAngles = Vector3.zero;
            _fadeObject.transform.name = c_fadeObjectName;

            Canvas canvas = _fadeObject.AddComponent<Canvas>();
            canvas.sortingOrder = 100; // Make sure the canvas renders on top
            canvas.renderMode = RenderMode.WorldSpace;

            _canvasGroup = _fadeObject.AddComponent<CanvasGroup>();
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = _initialFadeLevel;

            Image image = _fadeObject.AddComponent<Image>();
            image.color = Color.black;
            image.raycastTarget = false;
            image.material = _canvasMat;

            // Stretch the image
            RectTransform rect = _fadeObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0.2f, 0.2f);
            rect.localScale = new Vector2(2f, 2f);
        }

        /// <summary>
        /// Fade to transparent.
        /// </summary>
        /// <param name="duration">Fade duration.</param>
        /// <returns>Coroutine which finishes when the fade is done.</returns>
        public Coroutine FadeIn(float duration = 1f)
        {
            return FadeTo(0f, duration);
        }

        /// <summary>
        /// Fade to black.
        /// </summary>
        /// <param name="duration">Fade duration.</param>
        /// <returns>Coroutine which finishes when the fade is done.</returns>
        public Coroutine FadeOut(float duration = 1f)
        {
            return FadeTo(1f, duration);
        }

        /// <summary>
        /// Fade to black and then back to transparent.
        /// </summary>
        /// <param name="duration">Fade duration.</param>
        /// <returns>Coroutine which finishes when the faded back in (to transparent).</returns>
        public Coroutine FadeOutAndIn(float delay = 0f, float duration = 1f)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeOutAndInRoutine(delay, duration));
            return _fadeRoutine;
        }

        /// <summary>
        /// Fade to a custom level between 0 and 1 (0 => transparent, 1 => black).
        /// </summary>
        /// <param name="duration">Fade duration.</param>
        /// <returns>Coroutine which finishes when the fade is done.</returns>
        public Coroutine FadeTo(float fadeLevel, float duration = 1f)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeRoutine(fadeLevel, duration));
            return _fadeRoutine;
        }

        /// <summary>
        /// The fade routine.
        /// </summary>
        IEnumerator FadeRoutine(float fadeTo, float duration)
        {
            if (_fadeObject == null)
            {
                _initialFadeLevel = fadeTo;
                yield break;
            }

            if (!_fadeObject.activeSelf)
                _fadeObject.SetActive(true);

            float t = 0;
            float fadeFrom = _canvasGroup.alpha;

            while (t < duration)
            {
                _canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, t / duration);

                t += Time.unscaledDeltaTime;

                yield return null;
            }

            if (fadeTo == 0f)
                _fadeObject.SetActive(false);

            _canvasGroup.alpha = fadeTo;
        }

        /// <summary>
        /// The fade routine for fading out and in all in one go.
        /// </summary>
        IEnumerator FadeOutAndInRoutine(float delay, float duration)
        {
            yield return FadeRoutine(1f, duration);

            yield return new WaitForSecondsRealtime(delay);

            yield return FadeRoutine(0f, duration);
        }
    }
}

