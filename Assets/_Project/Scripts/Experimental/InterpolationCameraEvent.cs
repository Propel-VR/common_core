using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class InterpolationCameraEvent : MonoBehaviour
{
    private void OnPreCull ()
    {
        InterpolationController.ApplyInterpolation();
    }

    private void OnPostRender ()
    {
        InterpolationController.ClearInterpolation();
    }
}
