using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCoreScripts.SceneManagement;


namespace LM.TaskManagement
{

    /// <summary>
    /// What is a submodule? Is it the area where you will perform the tasks for a given 
    /// module?
    /// </summary>
    [CreateAssetMenu(fileName = "TaskSubModule", menuName = "Tasks/SubModule", order = 1)]
    public class TaskSubModule : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField]
        string _name;

        #endregion

        public string Name => _name; 
    }

}
