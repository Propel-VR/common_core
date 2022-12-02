using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableObject
{
    void OnBeginHover ();
    void OnEndHover ();
    void OnInteract ();
    bool IsInteractable ();
}
