using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    AutoHandPlayer playerController;
    [SerializeField]
    Transform teleportPos, offPos;

    bool hasPlayer = false;


    public void OnTeleport()
    {
        if (!hasPlayer)
        {
            playerController.SetPosition(teleportPos.position, teleportPos.rotation);
            playerController.useMovement= false;
            hasPlayer = true;
        }
        else
        {
            playerController.SetPosition(offPos.position, offPos.rotation);
            playerController.useMovement = true;
            hasPlayer = false;
        }
    }
}
