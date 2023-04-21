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
    AudioSource _goodSound;
    [SerializeField]
    AudioSource _badSound;


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

        transform.position = _preflightInteractable._UIPos.position;
        transform.rotation = _preflightInteractable._UIPos.rotation;
        transform.localScale = _preflightInteractable._UIPos.localScale;

    }

    public void CheckComplete()
    {
        _preflightInteractable.CheckComplete();
        if (_preflightInteractable && _preflightInteractable.Task != null)
        {
            _goodSound.Play();
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
            _goodSound.Play();
            gameObject.SetActive(false);

        }
        else
        {
            //SET TEXT TO INCORRECT
            _cleanButton.SetActive(false);
            _badSound.Play();
        }
    }

    public void TryReplace()
    {
        _preflightInteractable.Replace();
        _replaceButton.SetActive(false);
        _badSound.Play();

    }

    public void TryService()
    {
        _preflightInteractable.Service();
        _serviceButton.SetActive(false);
        _badSound.Play();


    }

    public void TryWriteUp()
    {
        _preflightInteractable.WriteUp();
        if (_preflightInteractable is WriteUpInteractable)
        {
            _preflightInteractable.MakeReadyForInteract();
            _goodSound.Play();
            gameObject.SetActive(false);

        }
        else
        {
            //SET TEXT TO INCORRECT
            _writeUpButton.SetActive(false);
            _badSound.Play();
        }
    }

}
