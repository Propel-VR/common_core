using UnityEngine;

namespace CommonCoreScripts.RandomSystem
{
    /// <summary>
    /// Created by: Juan Hiedra Primera
    ///
    /// General randomization helper used for setting a semi-permanent seed that can be overriden through
    /// player settings. The seed is stored in the PlayerPrefs, and by default is generated at Start if there is no other
    /// seed previously generated. This class is a singleton.
    /// </summary>
    public class RandomManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static RandomManager Instance {get; set;}
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // If there is no seed in the PlayerPrefs, generate one and store it.
            if (PlayerPrefs.HasKey("RandomSeed")) return;
            Random.InitState(Random.Range(0, int.MaxValue));
            PlayerPrefs.SetInt("RandomSeed", Random.seed); // using seed for PlayerPrefs compatibility
        }

        /// <summary>
        /// Sets the seed to a new value, and stores it in the PlayerPrefs.
        /// </summary>
        private void ShuffleSeed()
        {
            Random.InitState(Random.Range(0, int.MaxValue));
            PlayerPrefs.SetInt("RandomSeed", Random.seed);
        }
    }
}
