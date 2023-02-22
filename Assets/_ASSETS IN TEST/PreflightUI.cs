using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreflightUI : MonoBehaviour
{

    PreFlightInteractable _preflightInteractable;


    [SerializeField]
    GameObject _interactableSelected, _snagTypes;


    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetItem(PreFlightInteractable preflightInteractable)
    {
        gameObject.SetActive(true);
        _snagTypes.SetActive(false);
        _interactableSelected.SetActive(true);
        _preflightInteractable= preflightInteractable;

    }

    public void CheckComplete()
    {
        if(_preflightInteractable.Task != null )
            _preflightInteractable.Task.CompleteTask();
    }

    public void SnagFound()
    {
        _interactableSelected.SetActive(false);
        _snagTypes.SetActive(true);
    }

    public void TryWipe()
    {
        if(_preflightInteractable is WipeAwayInteractable)
        {
            _preflightInteractable.ReadyForInteraction=true;
            gameObject.SetActive(false);
        }
        else
        {
            //SET TEXT TO INCORRECT
        }
    }


}
