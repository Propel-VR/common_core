using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class ContextSystem : MonoBehaviour
{
    //[SerializeField] PlayerReferences playerRefs;
    //[SerializeField] LayerMask normalNoHighlightRenderLayer;
    //[SerializeField] LayerMask normalWithHighlightRenderLayer;

    [SerializeField] PostProcessLayer layer;
   
    public UnityEvent OnEnterContextMode;
    public UnityEvent OnExitContextMode;

    bool isInContext;
    Vector3 initPosition;
    //RiskObject lastRiskObject;
    PreFlightInteractable lastInteractable;

    public static bool IsInContext
    {
        get
        {
            if (Instance == null) return false;
            return Instance.isInContext;
        }
    }

    public static ContextSystem Instance;
    private void Start()
    {
        Instance = this;
        initPosition = AutoHandPlayer.Instance.transform.position;
        CloseContext();
    }


    public void CloseContext(Transform faceTowards = null)
    {
        if (lastInteractable) lastInteractable.SetContextState(false);
        if (isInContext)
        {
            // Teleport out of context mode
            AutoHandPlayer.Instance.SetPosition(initPosition);

            // Rotate properly
            if (faceTowards != null) AutoHandPlayer.Instance.SetRotation(faceTowards.rotation);

            OnExitContextMode?.Invoke();

            // Change rendering modes
            layer.enabled = false;
            //camera.cullingMask = normalNoHighlightRenderLayer;
            //highlightCamera.enabled = true;
        }

        isInContext = false;
    }

    public void OpenContext(PreFlightInteractable obj)
    {
        if (lastInteractable) lastInteractable.SetContextState(false);
        if (!isInContext) initPosition = AutoHandPlayer.Instance.transform.position;

        // Teleport into context mode
        AutoHandPlayer.Instance.SetPosition(obj.ContextPoint.position);

        // Rotate properly
        AutoHandPlayer.Instance.SetRotation(obj.ContextPoint.rotation);

        OnEnterContextMode?.Invoke();

        // Change rendering modes
        layer.enabled = true;
        //camera.cullingMask = normalNoHighlightRenderLayer;
        //highlightCamera.enabled = true;

        obj.SetContextState(true);

        isInContext = true;
        lastInteractable = obj;
    }
}