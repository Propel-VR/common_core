using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreflightUI : MonoBehaviour
{

    PreFlightInteractable _preflightInteractable;


    [SerializeField]
    GameObject _interactableSelected, _snagTypes;

    [Header("BUTTON REFERENCES")]
    [SerializeField]
    GameObject _cleanButton;
    [SerializeField]
    GameObject _replaceButton, _repairButton, _serviceButton, _writeUpButton;

    [Header("AUDIO REFERENCES")]
    [SerializeField]
    AudioSource goodSound;
    [SerializeField]
    AudioSource badSound;


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

        transform.position = _preflightInteractable.UIPos.position;
        transform.rotation = _preflightInteractable.UIPos.rotation;

    }

    public void CheckComplete()
    {
        if (_preflightInteractable.Task != null)
        {
            _preflightInteractable.CheckComplete();
            goodSound.Play();
        }
        gameObject.SetActive(false);
    }

    public void SnagFound()
    {
        _interactableSelected.SetActive(false);
        _snagTypes.SetActive(true);

        //SHOW ALL BUTTONS AGAIN
        _cleanButton.SetActive(true);
        _replaceButton.SetActive(true);
        _repairButton.SetActive(true);
        _serviceButton.SetActive(true);
        _writeUpButton.SetActive(true);
    }

    public void TryWipe()
    {
        if(_preflightInteractable is WipeAwayInteractable)
        {
            _preflightInteractable.MakeReadyForInteract();
            goodSound.Play();
            gameObject.SetActive(false);

        }
        else
        {
            //SET TEXT TO INCORRECT
            _cleanButton.SetActive(false);
            badSound.Play();
        }
    }

    public void TryReplace()
    {
        _replaceButton.SetActive(false);
        badSound.Play();

    }

    public void TryRepair()
    {
        _repairButton.SetActive(false);
        badSound.Play();


    }

    public void TryService()
    {
        _serviceButton.SetActive(false);
        badSound.Play();


    }

    public void TryWriteUp()
    {
        if (_preflightInteractable is WriteUpInteractable)
        {
            _preflightInteractable.MakeReadyForInteract();
            goodSound.Play();
            gameObject.SetActive(false);

        }
        else
        {
            //SET TEXT TO INCORRECT
            _writeUpButton.SetActive(false);
            badSound.Play();
        }
    }

}
