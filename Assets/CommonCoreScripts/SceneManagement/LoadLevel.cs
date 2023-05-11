using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Utility class for loading a level on start. Will not load the level
    /// if a level has already been loaded or is loading and is intended only 
    /// for use in Editor to load the required level when pressing the 'Play'
    /// button.
    /// 
    /// Note: Assumes that the scene this script is in is part of the level
    /// we are loading, otherwise the Coroutine will end early and the 
    /// transition will fail.
    /// </summary>
    public class LoadLevel : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Level to load")]
        Level _levelToLoad;

        private IEnumerator Start()
        {
            // only load the level if it exists and if we're still starting up (only one LoadLevel/BaseLoadLevel script should ever run per session)
            if (_levelToLoad == null || SceneManager.Instance.CurrentLevel != null || SceneManager.Instance.IsLoadingLevel)
                yield break;

            // instant fade out (sets fade canvas to black)
            yield return ScreenFader.Instance.FadeOut(0f);

            if (LoadingManager.Instance != null)
                LoadingManager.Instance.BeginLoading();

            yield return SceneManager.Instance.LoadLevel(_levelToLoad);

            if (LoadingManager.Instance != null)
                LoadingManager.Instance.FinishLoading();

            yield return ScreenFader.Instance.FadeIn();
        }
    }

}
