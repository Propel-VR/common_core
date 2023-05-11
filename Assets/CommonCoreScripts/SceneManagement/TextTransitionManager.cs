using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Responsible for text transitions.
    /// </summary>
    public class TextTransitionManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// The TextTransitionManager instance, if it exists (should exist in the text transition scene).
        /// </summary>
        public static TextTransitionManager Instance { get; private set; }

        #endregion

        #region Serialized and Private Fields

        [SerializeField]
        [Tooltip("The amount of time (seconds) it takes for the canvas to fade in/out (will be overriden if the duration is less than 2x this amount).")]
        float _defaultCanvasFadeDuration = 0.3f;

        [SerializeField]
        [Tooltip("The parent object of all objects that will be enabled/disabled for the transtion.")]
        GameObject _transitionSceneGameObject;

        [SerializeField]
        [Tooltip("The camera used during the transtion.")]
        Camera _transitionCamera;

        [SerializeField]
        [Tooltip("The CanvasGroup component to fade in/out during transitions.")]
        CanvasGroup _canvas;

        [SerializeField]
        [Tooltip("The text component that will hold the transition text.")]
        TextMeshProUGUI _transitionText;

        [Header("Events")]
        [SerializeField]
        [Tooltip("Event fired when a transition begins.")]
        UnityEvent _onBeginTransition;

        [SerializeField]
        [Tooltip("Event fire when a transition ends.")]
        UnityEvent _onEndTransition;

        bool _endTransitionTriggered = false;
        Coroutine _waitForPlayerRoutine = null;

        #endregion

        /// <summary>
        /// True if currently in a transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        #region Events

        /// <summary>
        /// Event fired immediately after the screen has faded out.
        /// </summary>
        public Action OnFadedOut { get; set; }

        /// <summary>
        /// Event fired right before fading back in, while the screen is still black.
        /// </summary>
        public Action OnFadeIn { get; set; }

        /// <summary>
        /// Event fired before a transition, before fading out the camera.
        /// </summary>
        public Action OnBeginTransition { get; set; }

        /// <summary>
        /// Event fired after a transition, after the camera has faded back in.
        /// </summary>
        public Action OnEndTransition { get; set; }

        #endregion

        #region Monobehaviour Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[TextTransitionManager]: Multiple TextTransitionManager's active simultaneously.");
                Destroy(this);
                return;
            }

            Instance = this;

            _transitionSceneGameObject.SetActive(false);
            _transitionCamera.enabled = false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        #endregion

        /// <summary>
        /// Begin a text transition, fading out the screen fading to the given text, and 
        /// then fading back in.
        /// </summary>
        /// <param name="text">The text to display on screen/in front of the player.</param>
        /// <param name="duration">The length of time to show the text for. If -1, will show the text indefinitely until <see cref="EndTransition"/> is called.</param>
        /// <param name="onTransition">Optional event which to call during the transition, while the camera is faded out.</param>
        /// <returns>A routine which ends when the camera has faded back in.</returns>
        public Coroutine BeginTransition(string text, float duration = -1f, Action onFadedOut = null, Action onFadeIn = null)
        {
            if (IsInTransition)
                StopAllCoroutines();

            return StartCoroutine(TransitionRoutine(text, duration, onFadedOut, onFadeIn));
        }

        /// <summary>
        /// End the current transition (only needs to be called if transition was begun with a 
        /// duration of -1).
        /// </summary>
        public void EndTransition()
        {
            if (!IsInTransition)
                return;
            else
                _endTransitionTriggered = true;
        }

        #region Private Methods

        /// <summary>
        /// The routine which performs the transition (<see cref="BeginTransition"/>).
        /// </summary>
        IEnumerator TransitionRoutine(string text, float duration = -1f, Action onFadedOut = null, Action onFadeIn = null)
        {
            IsInTransition = true;

            // fire 'begin' events
            _onBeginTransition?.Invoke();
            OnBeginTransition?.Invoke();

            _transitionText.text = text;

            // fade out
            yield return ScreenFader.Instance.FadeOut();

            yield return null;

            // screen is now completely black
            onFadedOut?.Invoke();
            OnFadedOut?.Invoke();

            // enable transition gameobjects
            _transitionSceneGameObject.SetActive(true);

            // wait one frame between setting tracked pose driver active and setting camera active 
            // so that position is updated before rendering to camera (or something like that - this 
            // removes that single frame "lag" that occurs otherwise when switching rigs)
            yield return null;

            // swap cameras/rigs
            if (Player.Get())
                Player.Get().Camera.enabled = false;
            else
                _waitForPlayerRoutine = StartCoroutine(WaitForPlayerAndSetActive(false));

            _transitionCamera.enabled = true;

            // fade in the text
            float canvasFadeDuration = _defaultCanvasFadeDuration;
            
            // if duration is smaller than what will allow for a full transition, use half the duration
            if (duration > 0f && duration < _defaultCanvasFadeDuration * 2)
                canvasFadeDuration = duration / 2f;

            StartCoroutine(FadeCanvas(1f, canvasFadeDuration));

            // wait for the moment when we need to fade out the text
            if (duration >= 0f)
                yield return new WaitForSecondsRealtime(duration - canvasFadeDuration);
            else
                yield return new WaitUntil(() => _endTransitionTriggered);

            // fade out the text
            yield return FadeCanvas(0f, canvasFadeDuration);

            // wait one frame to finish fade out
            yield return null;

            // swap back to player rig camera
            _transitionSceneGameObject.SetActive(false);
            _transitionCamera.enabled = false;

            if (Player.Get())
                Player.Get().Camera.enabled = true;

            if (_waitForPlayerRoutine != null)
                StopCoroutine(_waitForPlayerRoutine);

            yield return null;

            // screen is now black (again) - we are about to fade back in
            onFadeIn?.Invoke();
            OnFadeIn?.Invoke();

            // fade back in
            yield return ScreenFader.Instance.FadeIn();

            IsInTransition = false;
            _endTransitionTriggered = false;

            // fire 'end' events
            _onEndTransition?.Invoke();
            OnEndTransition?.Invoke();
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

        /// <summary>
        /// Fade a CanvasGroup to a specific value over time.
        /// </summary>
        /// <param name="to">The transparency to fade the canvas to.</param>
        /// <param name="duration">The amount of time it should take to fade the canvas.</param>
        IEnumerator FadeCanvas(float to, float duration)
        {
            float t = 0f;
            float start = _canvas.alpha;

            while (t < duration)
            {
                _canvas.alpha = Mathf.Lerp(start, to, t / duration);

                t += Time.unscaledDeltaTime;
                yield return null;
            }

            _canvas.alpha = to;
        }

        #endregion
    }

}
