using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StepController : MonoBehaviour
{
    [SerializeField]
    PuppetMaster puppet;

    [SerializeField]
    Animator animator;

    private SimulationMode simulationMode = SimulationMode.Guided;

    private static StepController instance;
    public static StepController Instance { get { return instance; } }

    public UnityEvent OnEachStepBegin;


    [SerializeField]
    Step[] steps;

    [SerializeField]
    Step completionStep;

    int stepNum = 0;
    int numSubSteps= 0;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        SetSimulationMode(SimulationMode.Guided);
    }

    public void SetSimulationMode(SimulationMode _simMode)
    {
        simulationMode = _simMode;

        switch (_simMode)
        {
            case SimulationMode.Guided:
                //start the simulation
                SetStep();
                break;
            case SimulationMode.UnGuided:
                //remove visuals
                GrabPointHandler.Instance.HideAllSprites();
                //start simulation
                SetStep();
                break;
            case SimulationMode.Watch:
                //show the watch mode
                //NOT IMPLIMENTED
                break;
        }
    }

    public void CompleteSubStep()
    {
        numSubSteps++;

        if(numSubSteps>=steps[stepNum].subSteps)
            StartNextStep();
    }

    private void StartNextStep()
    {
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

    private void SetStep()
    {
        //clear muscle overrides
        if (steps[stepNum].resetPreviousMuscles)
        {
            for (int i = 0; i < puppet.muscles.Length; i++)
            {
                puppet.muscles[i].props.muscleWeight = 1;
                puppet.muscles[i].props.pinWeight = 1;
            }
        }

        //GLOBAL WEIGHTS
        puppet.muscleWeight = steps[stepNum].muscleWeight;
        puppet.pinWeight = steps[stepNum].pinWeight;

       
        //INDIVIDUAL OVERRIDES
        foreach (MuscleOverride muscleOverride in steps[stepNum].muscleOverrides)
        {
            puppet.muscles[muscleOverride.muscleIndex].props.muscleWeight = muscleOverride.muscleWeight;
            puppet.muscles[muscleOverride.muscleIndex].props.pinWeight = muscleOverride.pinWeight;
        }

        //HANDLE OBJECTS
        foreach (GameObject gameObject in steps[stepNum].activatedObjects)
            gameObject.SetActive(true);
        foreach (GameObject gameObject in steps[stepNum].deactivatedObjects)
            gameObject.SetActive(false);

        //CHARACTER ANIMATIONS
        if (steps[stepNum].newAnimator != null)
            animator = steps[stepNum].newAnimator;
        if (!string.IsNullOrEmpty(steps[stepNum].animationTriggerOnStart))
            animator.SetTrigger(steps[stepNum].animationTriggerOnStart);
        

        //HANDLE EVENTS
        OnEachStepBegin?.Invoke();
        steps[stepNum].OnStepBegin?.Invoke();


    }

    /*
     * NEEDS TO BE COMPLETED. PROBABLY NOT THE WAY TO TAKE IT
     */
    private void StartCompletionStep()
    {
        //GLOBAL WEIGHTS
        puppet.muscleWeight = completionStep.muscleWeight;
        puppet.pinWeight = completionStep.pinWeight;

        //INDIVIDUAL OVERRIDES
        foreach (MuscleOverride muscleOverride in completionStep.muscleOverrides)
        {
            puppet.muscles[muscleOverride.muscleIndex].props.muscleWeight = muscleOverride.muscleWeight;
            puppet.muscles[muscleOverride.muscleIndex].props.pinWeight = muscleOverride.pinWeight;
        }

        //HANDLE OBJECTS
        foreach (GameObject gameObject in completionStep.activatedObjects)
            gameObject.SetActive(true);
        foreach (GameObject gameObject in completionStep.deactivatedObjects)
            gameObject.SetActive(false);

        //CHARACTER ANIMATIONS
        if (completionStep.newAnimator != null)
            animator = completionStep.newAnimator;
        if (!string.IsNullOrEmpty(completionStep.animationTriggerOnStart))
            animator.SetTrigger(completionStep.animationTriggerOnStart);
    }

    private void CompleteSteps()
    {
        StartCompletionStep();
    }

    [System.Serializable]
    public struct Step
    {
        [Header("General")]
        public StepType stepType;
        public int subSteps;

        public UnityEvent OnStepBegin;


        [Header("Global Muscle Settings")]
        [Range(0f, 1f)]
        public float pinWeight;
        [Range(0f, 1f)]
        public float muscleWeight;

        [Header("Override Muscle Settings")]
        public bool resetPreviousMuscles;
        public MuscleOverride[] muscleOverrides;

        [Header("Object Manipulation")]
        public GameObject[] activatedObjects;
        public GameObject[] deactivatedObjects;

        [Header("Character Manipulation")]
        [Tooltip("leave empty for none")]
        public Animator newAnimator;
        [Tooltip("leave empty for none")]
        public string animationTriggerOnStart;

    }

    [System.Serializable]
    public struct MuscleOverride
    {
        [Tooltip(
            "0  > hips\n" +
            "1  > L Thigh\n" +
            "2  > L Calf\n" + 
            "3  > L Foot\n" +
            "4  > R Thigh\n" +
            "5  > R Calf\n" +
            "6  > R Foot\n" +
            "7  > Back\n" +
            "8  > L Upper Arm\n" +
            "9  > L Forearm\n" +
            "10  > L Hand\n" +
            "11  > Head\n" +
            "12  > R Upper Arm\n" +
            "13 > R Forearm"+
            "14 > R Hand\n")]
        public int muscleIndex;

        [Range(0f, 1f)]
        public float pinWeight, muscleWeight;
    }

    [System.Serializable]
    public enum StepType
    {
        Reposition,
        Move,
        Complete
    }

    public enum SimulationMode
    {
        Watch,
        Guided,
        UnGuided
    }

}
