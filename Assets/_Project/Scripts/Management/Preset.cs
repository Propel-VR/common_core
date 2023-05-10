using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LM.Management
{

    /// <summary>
    /// Used to store a "preset" configuration that can be loaded via a single
    /// selection in the main menu.
    /// </summary>
    [CreateAssetMenu(fileName = "Preset", menuName = "Configurations/Preset", order = 1)]
    public class Preset : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField]
        string _name;

        [SerializeField]
        Configuration _configuration;

        #endregion

        #region Public Accessors

        public string Name => _name;

        public Configuration Configuration => _configuration;

        #endregion
    }

}