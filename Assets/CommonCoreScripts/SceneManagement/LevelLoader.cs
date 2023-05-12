using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Utility class for loading levels. Can be used in any scene.
    /// </summary>
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Should the level automatically start when finished loading, or should we wait in the loading scene until some other event?")]
        bool _autoStartLevel = true;

        /// <summary>
        /// Load a level and do the proper fading in/out, etc.
        /// </summary>
        public void LoadLevel(Level level)
        {
            TransitionManager.Instance.TransitionToLevel(level, _autoStartLevel);
        }

        /// <summary>
        /// Finish
        /// </summary>
        public void FinishLevelTransition()
        {
            TransitionManager.Instance.FinishTransition();
        }
    }

}
