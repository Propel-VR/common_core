using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


namespace CamhOO
{

    /// <summary>
    /// A task that can be completed, assuming its prerequisites have
    /// been met and it has been started. Also holds events which will 
    /// be called on start/completion.
    /// </summary>
    public class Task : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        [Tooltip("Keep this task from starting until all the prerequisite tasks have completed")]
        List<Task> _prerequisites = new();
        [SerializeField]
        bool _repeatable = false;

        [Header("Events")]

        /// <summary>
        /// Called when the task is started
        /// </summary>
        public UnityEvent OnTaskStarted;

        /// <summary>
        /// Called when the task is successfully completed
        /// </summary>
        public UnityEvent OnTaskCompleted;

        /// <summary>
        /// Called if the task objects becomes active
        /// </summary>
        public UnityEvent OnTaskEnabled;

        /// <summary>
        /// Called if the task object becomes disabled (task cannot be completed at this time)
        /// </summary>
        public UnityEvent OnTaskDisabled;

        #endregion

        #region Private Fields

        bool _isStarted = false;
        bool _isCompleted;

        #endregion

        #region Monobehaviour Methods

        private void OnEnable()
        {
            OnTaskEnabled?.Invoke();
        }

        private void OnDisable()
        {
            OnTaskDisabled?.Invoke();
        }

        #endregion

        #region Public Accessors and Methods

        /// <summary>
        /// A task can be completed only if it has been started, is active in the hierarchy,
        /// and has not yet been completed.
        /// </summary>
        public bool CanBeCompleted => _isStarted && (!_isCompleted || _repeatable) && gameObject.activeInHierarchy;

        /// <summary>
        /// Is this task completed?
        /// </summary>
        public bool IsCompleted => _isCompleted;

        /// <summary>
        /// Start this task
        /// 
        /// A task can only start if it is active and any prerequisite tasks are completed.
        /// </summary>
        public void StartTask()
        {
            if (_isStarted || !gameObject.activeInHierarchy)
                return;

            if (!_prerequisites.TrueForAll(t => t.IsCompleted))
                return;

            _isStarted = true;
            OnTaskStarted?.Invoke();
        }

        /// <summary>
        /// Mark this task as complete 
        /// 
        /// A task can only be marked as complete if it is active and has been started
        /// </summary>
        public void CompleteTask()
        {
            if (CanBeCompleted)
            {
                _isCompleted = true;
                OnTaskCompleted?.Invoke();
            }
        }

        #endregion
    }

}
