using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LM
{

    [CreateAssetMenu(fileName = "TaskSubModule", menuName = "Tasks/SubModule", order = 1)]
    public class TaskSubModule : ScriptableObject
    {
        [SerializeField]
        string _name;
    }

}
