using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Utility class for loading scenes on start.
    /// </summary>
    public class LoadScenes : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Scenes to load when playing in editor")]
        List<SceneRef> _scenesToLoad;

        [SerializeField]
        float _loadDelaySeconds = 0f;

        [SerializeField]
        bool _dontUnloadScenes = false;

        private void Start()
        {
            StartCoroutine(LoadRoutine());
        }

        IEnumerator LoadRoutine()
        {
            if (_loadDelaySeconds > 0f)
                yield return new WaitForSeconds(_loadDelaySeconds);

            if (_scenesToLoad.Count > 0)
                SceneManager.Instance.LoadScenes(_scenesToLoad.ConvertAll(s => s.name));

            if (_dontUnloadScenes)
            {
                foreach (var scene in _scenesToLoad)
                    SceneManager.Instance.DontUnloadScene(scene.name);
            }
        }
    }

}

