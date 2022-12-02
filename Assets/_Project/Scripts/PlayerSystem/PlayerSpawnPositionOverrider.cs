using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a script to fix a silly issue with the PlayerController
/// It requires the spawn positon to be know before Awake gets called on it.
/// To do stuff like spawn the player somewhere else based on training, this is
/// required. This should only be temporary.
/// </summary>
[DefaultExecutionOrder(-30000), RequireComponent(typeof(PlayerController))]
public class PlayerSpawnPositionOverrider : MonoBehaviour
{
    [SerializeField] Transform trainingSpawnPosition;
    [SerializeField] Transform defaultSpawnPosition;

    static bool hasBeenTrained = false;
    private void Awake ()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (!hasBeenTrained)
        {
            hasBeenTrained = true;
            playerController.SetSpawnAnchor(trainingSpawnPosition);
        }
        else
        {
            playerController.SetSpawnAnchor(defaultSpawnPosition);
        }
    }
}
