using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace LM.TaskManagement
{
    public class TaskEvents : MonoBehaviour
    {
        [SerializeField]
        Task _task;

        [SerializeField]
        UnityTaskEvent _onTaskStarted;

        [SerializeField]
        UnityTaskEvent _onTaskCompleted;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }
    }
}
