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
        UnityEvent _onTaskStetup;

        [SerializeField]
        UnityEvent _onTaskStarted;

        [SerializeField]
        UnityEvent _onTaskCompleted;

        private void OnEnable()
        {
            _task.OnSetup += OnTaskSetup;
            _task.OnStarted += OnTaskStarted;
            _task.OnCompleted += OnTaskCompleted;
        }

        private void OnDisable()
        {
            _task.OnSetup -= OnTaskSetup;
            _task.OnStarted -= OnTaskStarted;
            _task.OnCompleted -= OnTaskCompleted;
        }

        void OnTaskSetup()
        {
            _onTaskStetup?.Invoke();
        }

        void OnTaskStarted()
        {
            _onTaskStarted?.Invoke();
        }

        void OnTaskCompleted()
        {
            _onTaskCompleted?.Invoke();
        }
    }
}
