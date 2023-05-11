using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCoreScripts.SceneManagement;


namespace LM.TaskManagement
{

    /// <summary>
    /// Holds any metadata for the different modules that can be chosen.
    /// </summary>
    [CreateAssetMenu(fileName = "TaskModule", menuName = "Tasks/Module", order = 1)]
    public class TaskModule : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField]
        string _name;

        #endregion

        public string Name => _name;
    }

}
