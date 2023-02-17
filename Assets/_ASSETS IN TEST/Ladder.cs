using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    AutoHandPlayer playerController;
    [SerializeField]
    Transform teleportPos;

    public void OnTeleport()
    {
        playerController.SetPosition(teleportPos.position,teleportPos.rotation);
    }
}
