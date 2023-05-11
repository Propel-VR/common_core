using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Default transition manager. 
    /// 
    /// Responsible for fading the scene out, loading a transition scene, and then going back again.
    /// </summary>
    public class TransitionManager : SerializedMonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// The TransitionManager instance, if it exists (should exist in a persistant scene since
        /// otherwise it will be destroyed during level transitions and not be able to complete the
        /// transition).
        /// </summary>
        public static TransitionManager Instance { get; private set; }

        #endregion

        /// <summary>
        /// True if currently in a transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        #region Monobehaviour Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[TransitionManager]: Multiple TransitionManager's active simultaneously.");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        #endregion

        /// <summary>
        /// Transition to a new level. This will fade out the current scene, swap to
        /// the loading scene, unload the previous level and load the new one, then 
        /// swap back to the player camera and fade back in.
        /// </summary>
        /// <param name="level">The new level to load.</param>
        public void TransitionToLevel(Level level)
        {
            if (IsInTransition)
                return;

            StartCoroutine(TransitionToLevelRoutine(level));
        }

        #region Private Methods

        /// <summary>
        /// Performs the actual transition sequence.
        /// </summary>
        /// <param name="level">The new level to load.</param>
        IEnumerator TransitionToLevelRoutine(Level level)
        {
            IsInTransition = true;

            yield return ScreenFader.Instance.FadeOut();

            // wait one frame so that fade out finishes (canvas should be all the way black before swapping to loading scene)
            yield return null;

            if (LoadingManager.Instance != null)
                LoadingManager.Instance.BeginLoading();

            yield return SceneManager.Instance.LoadLevel(level);

            if (LoadingManager.Instance != null)
                LoadingManager.Instance.FinishLoading();

            yield return null;

            yield return ScreenFader.Instance.FadeIn();

            IsInTransition = false;
        }

        #endregion

    }

}
