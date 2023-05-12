using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace CommonCoreScripts.SceneManagement
{

    public class SceneLoadingEvents : MonoBehaviour
    {
        [SerializeField]
        UnityEvent _onFinishLoadLevel;

        private void OnEnable()
        {
            SceneManager.OnFinishLoadLevel += OnFinishLoadLevel;
        }

        private void OnDisable()
        {
            SceneManager.OnFinishLoadLevel -= OnFinishLoadLevel;
        }

        void OnFinishLoadLevel(Level from, Level to)
        {
            _onFinishLoadLevel?.Invoke();
        }
    }

}
