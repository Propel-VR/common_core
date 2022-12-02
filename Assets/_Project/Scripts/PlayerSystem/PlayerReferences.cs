using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hub for all reference objects in the world may require
/// for a multitude of purpose, and to also avoid singletons.
/// </summary>
public class PlayerReferences : MonoBehaviour
{
    [field: SerializeField] public PlayerController Controller { get; private set; } 
    [field: SerializeField] public HandSwitcher HandSwitcher { get; private set; } 
    [field: SerializeField] public ContextSystem Context { get; private set; } 
    [field: SerializeField] public Camera Camera { get; private set; } 
    [field: SerializeField] public Collider HeadCollider { get; private set; } 
    [field: SerializeField] public GameObject[] Tooltips { get; private set; } 
}
