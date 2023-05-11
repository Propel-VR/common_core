using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Manages a specific level and performs any events related to the start/end of the level.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The player's starting position")]
        Transform _startPosition;

        [Header("Events")]

        public UnityEvent onStartLevel;

        public UnityEvent onEndLevel;

        /// <summary>
        /// The current LevelManager instance - i.e. the one responsible for the current level.
        /// 
        /// Note: Not valid during level transitions.
        /// </summary>
        public static LevelManager Current { get; private set; }

        private void Awake()
        {
            Debug.Log("[LevelManager]: Awake");

            // when this object awakes (assuming it is not in a persistent scene) the level we 
            // it is responsible for will still be loading - wait for it to finish loading
            SceneManager.OnFinishLoadLevel += OnStartLevel;
        }

        private void Start()
        {
            Debug.Log("[LevelManager]: Start");
        }

        void OnStartLevel(Level from, Level to)
        {
            Debug.Log("[LevelManager]: OnLoadLevel");
            Debug.Assert(Current == null, "Multiple LevelManager's found in level", this);
            Current = this;

            if (_startPosition)
                Player.Get().Teleport(_startPosition);

            onStartLevel?.Invoke();
            SceneManager.OnFinishLoadLevel -= OnStartLevel;

            // the next time we BEGIN to load a level will be when we plan to unload this one
            SceneManager.OnBeginLoadLevel += OnEndLevel;
        }

        private void OnEndLevel(Level from, Level to)
        {
            onEndLevel?.Invoke();
            SceneManager.OnBeginLoadLevel -= OnEndLevel;
        }

        private void OnDestroy()
        {
            if (Current != this)
                return;

            Current = null;
        }
    }

}

