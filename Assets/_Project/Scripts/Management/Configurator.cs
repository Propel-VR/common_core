using CommonCoreScripts.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LM.TaskManagement;


namespace LM.Management
{

    public class Configurator : MonoBehaviour
    {
        #region Singleton

        public static Configurator Instance { get; private set; }

        #endregion

        [SerializeField]
        SceneResolver _sceneResolver;

        Configuration _configuration;

        #region Monobehaviour Methods

        private void Awake()
        {
            Debug.Assert(Instance == null, "[Configurator]: Found multiple Configurators. There should only be one Configurator, and it should be in the base scene.");
            Instance = this;
        }

        #endregion

        public void SetWeather(Weather weather)
        {
            _configuration.weather = weather;
        }

        public void SetLocation(Location location)
        {
            _configuration.location = location;
        }

        public void SetTimeOfDay(TimeOfDay timeOfDay)
        {
            _configuration.timeOfDay = timeOfDay;
        }

        public void LoadPreset(Preset preset)
        {
            _configuration = preset.Configuration;
        }

        public void LoadConfiguration()
        {
            // determine scenes to load based on configuration
            Level level = _sceneResolver.ResolveConfiguration(_configuration);

            // setup loading screen with info from configuration


            // begin loading new scenes
            SceneManager.Instance.LoadLevel(level);

            // hand off tasks to TaskManager
        }
    }

}
