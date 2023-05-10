using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LM.Management
{

    public class Configurator : MonoBehaviour
    {
        Configuration _configuration;

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

            // setup loading screen with info from configuration

            // begin loading new scenes

            // hand off tasks to TaskManager
        }
    }

}
