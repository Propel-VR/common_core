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
    
    //[SerializeField]
    //GameObject _topLayer;

    bool _hasPlayer = false;
    bool _allowTeleport = false;


    private void Start()
    {
        //_topLayer.SetActive(false);
    }

    public void OnTeleport()
    {
        if (!_hasPlayer)
        {
            //_topLayer.SetActive(true);
            _playerController.SetPosition(_teleportPos.position, _teleportPos.rotation);
            _playerController.useMovement= false;
            _hasPlayer = true;
        }
        else
        {
            //_topLayer.SetActive(false);
            _playerController.SetPosition(_offPos.position, _offPos.rotation);
            _playerController.useMovement = true;
            _hasPlayer = false;
        }
    }

    public void OnPlace()
    {
        _allowTeleport = false;

    }

    public void OnRemove()
    {
        _allowTeleport = false;

    }
}
