using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StepController : MonoBehaviour
{
    #region singleton
    public static StepController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this;
        else
            Destroy(this);
    }
    #endregion

    #region unity events
    public UnityEvent OnStepsBegin;
    public UnityEvent OnEachStepBegin;
    public UnityEvent OnEachStepEnd;
    public UnityEvent OnStepsComplete;
    #endregion

    public Step CurrentStep => _steps[_stepNum];
    public bool isActive { get; private set; }
    public int NumSteps => _steps.Length;

    #region private variables
    private int _stepNum = -1;
    private int _numSubSteps = 0;

    [SerializeReference]
    private Step[] _steps;
    [SerializeField]
    bool _startActive;
    #endregion



    #region public methods
    /// <summary>
    /// Called externally whenever a substep has been completed 
    /// </summary>
    public void CompleteSubStep()
    {
        _numSubSteps++;

        if (ReadyForNextStep())
        {
            CompleteStep();
        }
    }

    public virtual void StartSteps()
    {
        Debug.Log("Starting Steps");
        isActive = true;
        StartNextStep();
        OnStepsBegin?.Invoke();

    }
    #endregion

    #region protected methods
    /// <summary>
    /// Start the next stepin the process. This Method should be overridden for the use of custom steps
    /// </summary>
    /// <returns> IEnumerator used for delays</returns>
    protected virtual IEnumerator StartStep()
    {
        yield return new WaitForSeconds(_steps[_stepNum].Delay);
        yield return new WaitForEndOfFrame();
        //HANDLE START EVENTS
        OnEachStepBegin?.Invoke();
        _steps[_stepNum].OnStepBegin?.Invoke();


        //HANDLE OBJECTS
        foreach (GameObject gameObject in _steps[_stepNum].ActivatedObjects)
            gameObject.SetActive(true);
        foreach (GameObject gameObject in _steps[_stepNum].DeactivatedObjects)
            gameObject.SetActive(false);

        if(_steps[_stepNum].NumSubSteps <=0)
            CompleteStep();
    }

    /// <summary>
    /// Checks to see if the StepController is ready for the next step
    /// </summary>
    /// <returns></returns>
    protected virtual bool ReadyForNextStep()
    {
        return _numSubSteps >= _steps[_stepNum].NumSubSteps;
    }

    /// <summary>
    /// Called on a step end
    /// </summary>
    protected virtual void StepEnd()
    {
        //HANDLE END EVENTS
        OnEachStepEnd?.Invoke();
        _steps[_stepNum].OnStepEnd?.Invoke();

    }

    protected virtual void CompleteStep()
    {
        OnEachStepEnd?.Invoke();
        _steps[_stepNum].OnStepEnd?.Invoke();
        StartNextStep();
    }

    /// <summary>
    /// Called when all steps are complete
    /// </summary>
    protected virtual void CompleteSteps()
    {
        isActive = false;
        OnStepsComplete?.Invoke();
    }
    #endregion

    #region private methods
    private void Start()
    {

        if (_startActive)
        {     
            StartSteps();
        }
    }


    /// <summary>
    /// Called to start the next step
    /// </summary>
    private void StartNextStep()
    {

        _numSubSteps = 0;
        _stepNum++;
        if (_stepNum < NumSteps)
        {
            StartCoroutine(StartStep());
        }
        else
        {
            CompleteSteps();
        }
    }
    #endregion

    #region classes
    /// <summary>
    /// class that holds the data for a step. This should be subclassed for the use of custom steps
    /// </summary>
    [System.Serializable]
    public class Step
    {
        [Header("GENERAL")]
        public string stepName;
        public string stepDescription;
        public float Delay;
        public int NumSubSteps;


        [Header("EVENTS")]
        public UnityEvent OnStepBegin;
        public UnityEvent OnStepEnd;


        [Header("OBJECT MANIPULATION")]
        public GameObject[] ActivatedObjects;
        public GameObject[] DeactivatedObjects;
    }
    #endregion
    
}
