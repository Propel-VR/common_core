using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelection : MonoBehaviour
{
    [SerializeField]
    StepController stepController;


    public void SelectGuided()
    {
        //stepController.SetSimulationMode(StepController.SimulationMode.UnGuided);
        gameObject.SetActive(false);
    }
    public void SelectUnGuided()
    {
        //stepController.SetSimulationMode(StepController.SimulationMode.UnGuided);
        gameObject.SetActive(false);
    }
}
