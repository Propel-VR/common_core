using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LM.TaskManagement;


namespace LM.Management
{
    public enum Location
    {
        Hanger1,
    }

    public enum Weather
    {
        Sunny, 
        Rainy
    }

    public enum TimeOfDay
    {
        Morning, 
        Afternoon,
        Evening
    }

    /// <summary>
    /// Responsible for storing any settings (e.g. tasks, location, weather) 
    /// that can be saved as a "preset."
    /// </summary>
    [Serializable]
    public class Configuration
    {
        public Location location;

        public Weather weather;

        public TimeOfDay timeOfDay;

        public List<Task> tasks;
    }

}
