using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CommonCoreScripts.SceneManagement
{

    /// <summary>
    /// Defines a list of scenes to be loaded all at one time.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Level", fileName = "Level", order = 0)]
    public class Level : ScriptableObject
    {
        [Tooltip("Scenes required by the level")]
        [Required]
        public List<SceneRef> scenes = new();

        [Tooltip("The scene that will be set to 'active' for this level (should hold the lighting data for the level)")]
        [Required]
        public SceneRef activeScene;
    }

}

