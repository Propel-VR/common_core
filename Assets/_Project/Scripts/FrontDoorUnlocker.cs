using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDoorUnlocker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerReferences targetPlayer;
    [SerializeField] AudioSource audioSource;
    [SerializeField] DoorScript door;

    [Header("Ressources")]
    [SerializeField] int delayBeforeSound = 3;
    [SerializeField] AudioClip annouceEntranceClip;
    [SerializeField] AudioClip doorUnlockClip;

    private bool hasBeenUnlocked;
    private void Start ()
    {
        ActivityManager.onActivityStart += OnActivityStart;
    }
    private void OnDisable ()
    {
        ActivityManager.onActivityStart -= OnActivityStart;
    }

    private void OnActivityStart (ActivityType obj)
    {

        hasBeenUnlocked = false;
        door.SetLock(true);
    }

    public void OnClickDoorBell ()
    {
        if (ActivityManager.CurrentActivityType != ActivityType.RiskAssesment) return;
        if (hasBeenUnlocked) return;
        hasBeenUnlocked = true;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence ()
    {
        yield return new WaitForSeconds(delayBeforeSound);
        audioSource.clip = annouceEntranceClip;
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.clip = doorUnlockClip;
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        door.SetLock(false);
        door.SimulateOnTriggerEnter(targetPlayer.HeadCollider);
    }
}
