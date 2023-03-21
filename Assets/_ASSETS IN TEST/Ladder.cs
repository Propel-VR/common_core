using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class for a ladder that allows a player to teleport on top 
/// </summary>
public class Ladder : MonoBehaviour
{
    [SerializeField]
    AutoHandPlayer _playerController;
    [SerializeField]
    Transform _teleportPos, _offPos;

    bool hasPlayer = false;


    public void OnTeleport()
    {
        if (!hasPlayer)
        {
            _playerController.SetPosition(_teleportPos.position, _teleportPos.rotation);
            _playerController.useMovement= false;
            hasPlayer = true;
        }
        else
        {
            _playerController.SetPosition(_offPos.position, _offPos.rotation);
            _playerController.useMovement = true;
            hasPlayer = false;
        }
    }
}
