using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Switches back and forth between normal mode and context mode. Context mode is used
/// to highlight a specific object in a scene and making everything else greyed out.
/// </summary>
public class ContextSystem : MonoBehaviour
{
    [SerializeField] PlayerReferences playerRefs;
    [SerializeField] LayerMask normalNoHighlightRenderLayer;
    [SerializeField] LayerMask normalWithHighlightRenderLayer;

    [SerializeField] PostProcessLayer layer;
    [SerializeField] Camera camera;
    [SerializeField] Camera highlightCamera;

    public event Action OnEnterContextMode;
    public event Action OnExitContextMode;

    bool isInContext;
    Vector3 initPosition;
    RiskObject lastRiskObject;



    private void Start ()
    {
        initPosition = playerRefs.Controller.GetPosition();
        CloseContext();
    }


    public void CloseContext (Transform faceTowards = null)
    {
        if (lastRiskObject) lastRiskObject.SetContextState(false);
        if (isInContext)
        {
            // Teleport out of context mode
            playerRefs.Controller.TeleportTo(initPosition, true, () =>
            {
                // Rotate properly
                if (faceTowards != null) playerRefs.Controller.RotateTowards(faceTowards.position);

                OnExitContextMode?.Invoke();

                // Change rendering modes
                layer.enabled = false;
                camera.cullingMask = normalNoHighlightRenderLayer;
                highlightCamera.enabled = true;
            });
        }

        

        isInContext = false;
    }

    public void OpenContext (RiskObject riskObject) 
    {
        if (lastRiskObject) lastRiskObject.SetContextState(false);
        if (!isInContext) initPosition = playerRefs.Controller.GetPosition();

        // Teleport into context mode
        playerRefs.Controller.TeleportTo(riskObject.GetContextPoint(), true, () => {

            // Rotate properly
            playerRefs.Controller.RotateTowards(riskObject.transform.position);
            
            OnEnterContextMode?.Invoke();

            // Change rendering modes
            layer.enabled = true;
            camera.cullingMask = normalNoHighlightRenderLayer;
            highlightCamera.enabled = true;
        });
        riskObject.SetContextState(true);

        isInContext = true;
        lastRiskObject = riskObject;
    }
}
