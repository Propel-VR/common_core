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
        /// <summary>
        /// Load a level and do the proper fading in/out, etc.
        /// </summary>
        public void LoadLevel(Level level)
        {
            TransitionManager.Instance.TransitionToLevel(level);
        }
    }

}
