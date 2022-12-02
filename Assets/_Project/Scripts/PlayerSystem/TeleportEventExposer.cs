using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEventExposer : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] List<Transform> teleportTarget;

    public void TeleportPlayerToTarget (int index)
    {
        player.TeleportTo(teleportTarget[index].position, true, () => { 
            player.RotateTowards(teleportTarget[index].position + teleportTarget[index].forward * 100f);
        });
    }
}
