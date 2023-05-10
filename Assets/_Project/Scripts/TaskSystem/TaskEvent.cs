using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace LM
{
    public class TaskEvent : MonoBehaviour
    {
        [SerializeField]
        Task _task;

        [SerializeField]
        UnityEvent _onTaskStarted;

        [SerializeField]
        UnityEvent _onTaskCompleted;
    }
}
