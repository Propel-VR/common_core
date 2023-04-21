using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LM
{

    [CreateAssetMenu(fileName = "TaskCategory", menuName = "Tasks/Category", order = 1)]
    public class TaskCategory : ScriptableObject
    {
        [SerializeField]
        string _name;
    }

}
