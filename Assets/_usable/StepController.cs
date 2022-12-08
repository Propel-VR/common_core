using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StepController : MonoBehaviour
{
   
    public static StepController Instance { get; private set; }

    public UnityEvent OnEachStepBegin, OnEachStepEnd, OnAllStepsComplete;


    [SerializeField]
    protected Step[] steps;

    
    int stepNum = 0;
    int numSubSteps= 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Instance = this;
        else
            Destroy(this);
    }

    public void CompleteSubStep()
    {
        numSubSteps++;

        if(numSubSteps>=steps[stepNum].subSteps)
            StartNextStep();
    }

    private void StartNextStep()
    {
        //HANDLE END EVENTS
        OnEachStepEnd?.Invoke();
        steps[stepNum].OnStepEnd?.Invoke();

        numSubSteps = 0;
        stepNum++;
        if(stepNum < steps.Length)
        {
            SetStep();
        }
        else
        {
            CompleteSteps();
        }
    }

    protected virtual IEnumerator SetStep()
    {
        yield return new WaitForSeconds(steps[stepNum].delay);

        //HANDLE START EVENTS
        OnEachStepBegin?.Invoke();
        steps[stepNum].OnStepBegin?.Invoke();
        
       
        //HANDLE OBJECTS
        foreach (GameObject gameObject in steps[stepNum].activatedObjects)
            gameObject.SetActive(true);
        foreach (GameObject gameObject in steps[stepNum].deactivatedObjects)
            gameObject.SetActive(false);

       
    }

    private void CompleteSteps()
    {
        OnAllStepsComplete?.Invoke();
    }

    [System.Serializable]
    public class Step
    {
        [Header("GENERAL")]
        public int subSteps;
        public float delay;

        [Header("EVENTS")]
        public UnityEvent OnStepBegin;
        public UnityEvent OnStepEnd;


        [Header("OBJECT MANIPULATION")]
        public GameObject[] activatedObjects;
        public GameObject[] deactivatedObjects;


    }


}
