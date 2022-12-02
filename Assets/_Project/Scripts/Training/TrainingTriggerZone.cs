using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTriggerZone : MonoBehaviour
{
    [SerializeField] int zoneId;
    [SerializeField] TrainingSystem trainingSystem;
    [SerializeField] Transform visual;
    [SerializeField] bool isOpen;
    bool alreadyTriggered;
    float value = 0f;

    public void SetOpen (bool isOpen)
    {
        if(!gameObject.activeSelf && isOpen) gameObject.SetActive(true);
        this.isOpen = isOpen;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (!isOpen) return;
        if (alreadyTriggered) return;
        if (other.gameObject.layer != 6) return;
        if (other.gameObject.tag != "DoorOpener") return; // Lol that's just the collider for the player head

        alreadyTriggered = true;
        trainingSystem.OnZoneTrigger(zoneId);
    }

    private void Update ()
    {
        value = Mathf.Clamp01(value + (isOpen ? 1 : -1) * Time.deltaTime * 2f);
        visual.localScale = value * Vector3.one;
    }
}
