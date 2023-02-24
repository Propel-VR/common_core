using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using RootMotion.Demos;
using Autohand;

public class XRRayInteractorCustom : MonoBehaviour, IUIInteractor
{
    public enum Mode
    {
        VisibleLine,
        ReticleOnly
    }

    [SerializeField] Mode mode = Mode.VisibleLine;

    [Header("Ray Parameters")]
    [SerializeField] bool autoUpdate = true;
    [SerializeField] float maxDistance = 5;
    [SerializeField] LayerMask colliderLayers;
    [SerializeField] string targetLayer;
    [SerializeField] QueryTriggerInteraction triggerInteraction;

    [Header("Line Parameters")]
    [SerializeField] float scrollSpeed = -0.5f;
    [SerializeField] Gradient validGradient;
    [SerializeField] Gradient invalidGradient;

    [Header("Reference")]
    [SerializeField] Transform playerTransform;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform origin;
    [SerializeField] Transform reticle;

    int targetLayerIndex = -1;
    bool lastTriggerValue;
    Collider lastCollider;
    IInteractableObject currentObject;
    bool isValidHit = false;

    public float MaxDistance { get => maxDistance; set => maxDistance = value; }

    private void Awake()
    {
        if (!string.IsNullOrEmpty(targetLayer))
        {
            targetLayerIndex = LayerMask.NameToLayer(targetLayer);
        }

    }

    private void LateUpdate()
    {
        if (autoUpdate) OnUpdate();

        if (lineRenderer.isVisible)
        {
            lineRenderer.material.mainTextureOffset = new Vector4(Time.time * scrollSpeed, 0f);
        }
        lineRenderer.positionCount = 2;
    }

    public void OnUpdate()
    {
        Collider newCollider = null;
        Vector3 hitPoint = Vector3.zero;
        Vector3 hitNormal = Vector3.zero;
        bool didHitSomething = false;
        bool didHitUI = false;

        // Raycasting geometry
        Ray ray = new Ray(origin.position, origin.forward);
        m_rayStart = ray.origin;
        m_rayEnd = ray.origin + ray.direction * maxDistance;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, colliderLayers, triggerInteraction))
        {
            hitPoint = hitInfo.point;
            hitNormal = hitInfo.normal;
            didHitSomething = true;

            bool isValid = true;
            if (targetLayerIndex != -1 && hitInfo.collider.gameObject.layer != targetLayerIndex)
            {
                isValid = false;
            }
            if (Physics.Linecast(playerTransform.TransformPoint(Vector3.up), m_rayStart, colliderLayers.value, triggerInteraction))
            {
                // Cancel if the arm is not linked to the body (trying to put your hands throught the wall to teleport throught it
                isValid = false;
            }
            if (isValid)
            {
                newCollider = hitInfo.collider;
                Debug.Log("HIT OBJ: "+hitInfo.collider.gameObject.name);
            }
        }

        // Raycasting UI
        if (TryGetCurrentUIRaycastResult(out RaycastResult raycastResult, out int raycastEndpointIndex))
        {
            if (raycastResult.isValid)
            {
                didHitSomething = true;
                didHitUI = true;
                hitPoint = raycastResult.worldPosition;
                hitNormal = raycastResult.worldNormal;
            }
        }

        // No longer focusing on last,
        if (newCollider != lastCollider && lastCollider != null)
        {
            currentObject.OnEndHover();
            currentObject = null;
        }
        // Focusing on new object
        if (newCollider != lastCollider && newCollider != null)
        {
            currentObject = (IInteractableObject)newCollider.GetComponent(typeof(IInteractableObject));
            currentObject.OnBeginHover();
        }

        isValidHit = didHitUI || (currentObject != null && currentObject.IsInteractable());

        if (isValidHit)
        {
            Debug.Log("VALID");
            Debug.Log(currentObject);
        }

            // Reticle
            if (mode == Mode.VisibleLine)
        {
            if (reticle.gameObject.activeSelf != didHitSomething)
                reticle.gameObject.SetActive(didHitSomething);
        }
        else
        {
            if (reticle.gameObject.activeSelf != isValidHit)
                reticle.gameObject.SetActive(isValidHit);
        }

        if (reticle.gameObject.activeSelf)
        {
            reticle.transform.forward = hitNormal;
            reticle.transform.position = hitPoint + hitNormal * 0.005f;
        }

        // Renderer

        if (mode == Mode.VisibleLine)
        {
            lineRenderer.colorGradient = isValidHit ? validGradient : invalidGradient;
            lineRenderer.SetPosition(0, ray.origin);
            if (didHitSomething)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(1, hitPoint - ray.direction * 0.02f);
            }
            else
                lineRenderer.enabled = false;
                //lineRenderer.SetPosition(1, ray.origin + ray.direction * maxDistance);
        }

        // Selection
        bool triggerValue = Input.GetAxisRaw("XRI_Right_Trigger") > 0.5f || Input.GetMouseButton(0);
        if (triggerValue && !lastTriggerValue && currentObject != null)
        {
            Debug.Log("INTERACTED");
            currentObject.OnInteract();
        }
        else
        {
            Debug.Log("NO INTERACT");
        }

        lastTriggerValue = triggerValue;
        lastCollider = newCollider;

    }
    void OnEnable()
    {
        if (m_EnableUIInteraction)
            RegisterWithXRUIInputModule();
    }

    void OnDisable()
    {
        if (m_EnableUIInteraction)
            UnregisterFromXRUIInputModule();
    }

    public void SetMode(Mode mode)
    {
        this.mode = mode;

        switch (mode)
        {
            case Mode.VisibleLine:
                lineRenderer.enabled = true;
                break;
            case Mode.ReticleOnly:
                lineRenderer.enabled = false;
                break;
        }
    }

    public void SetModeVisibleLine() => SetMode(Mode.VisibleLine);
    public void SetModeReticleOnly() => SetMode(Mode.ReticleOnly);



    #region UI Interaction
    XRUIInputModule m_InputModule;
    XRUIInputModule m_RegisteredInputModule;
    const bool m_EnableUIInteraction = true;
    Vector3 m_rayStart;
    Vector3 m_rayEnd;
    XRInteractionManager m_InteractionManager;
    static XRInteractionManager s_InteractionManagerCache;

    void FindCreateInteractionManager()
    {
        if (m_InteractionManager != null)
            return;

        if (s_InteractionManagerCache == null)
            s_InteractionManagerCache = FindObjectOfType<XRInteractionManager>();

        if (s_InteractionManagerCache == null)
        {
            var interactionManagerGO = new GameObject("XR Interaction Manager", typeof(XRInteractionManager));
            s_InteractionManagerCache = interactionManagerGO.GetComponent<XRInteractionManager>();
        }

        m_InteractionManager = s_InteractionManagerCache;
    }


    public void ShowRay()
    {
        if(!AutoHandPlayerHelper.Instance.GetRightHand().GetHeldGrabbable())
            lineRenderer.enabled = true;
    }

    public void HideRay() 
    {

        lineRenderer.enabled = false;
    
    }
    #region XRUI Creation/Registering
    void FindOrCreateXRUIInputModule()
    {
        var eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
            eventSystem = new GameObject("EventSystem", typeof(EventSystem)).GetComponent<EventSystem>();
        else
        {
            // Remove the Standalone Input Module if already implemented, since it will block the XRUIInputModule
            var standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneInputModule != null)
                Destroy(standaloneInputModule);
        }

        m_InputModule = eventSystem.GetComponent<XRUIInputModule>();
        if (m_InputModule == null)
            m_InputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();
    }

    void RegisterWithXRUIInputModule()
    {
        if (m_InputModule == null)
            FindOrCreateXRUIInputModule();

        if (m_RegisteredInputModule == m_InputModule)
            return;

        UnregisterFromXRUIInputModule();

        m_InputModule.RegisterInteractor(this);
        m_RegisteredInputModule = m_InputModule;

    }

    void UnregisterFromXRUIInputModule()
    {
        if (m_RegisteredInputModule != null)
            m_RegisteredInputModule.UnregisterInteractor(this);

        m_RegisteredInputModule = null;
    }
    #endregion

    public bool TryGetCurrentUIRaycastResult(out RaycastResult raycastResult, out int raycastEndpointIndex)
    {
        if (TryGetUIModel(out var model) && model.currentRaycast.isValid)
        {
            raycastResult = model.currentRaycast;
            raycastEndpointIndex = model.currentRaycastEndpointIndex;
            return true;
        }

        raycastResult = default;
        raycastEndpointIndex = default;
        return false;
    }


    public virtual void UpdateUIModel(ref TrackedDeviceModel model)
    {
        if (!isActiveAndEnabled)
            return;

        model.position = origin.position;
        model.orientation = origin.rotation;
        model.select = m_EnableUIInteraction;
        model.raycastLayerMask = colliderLayers;

        var raycastPoints = model.raycastPoints;
        raycastPoints.Clear();
        raycastPoints.Add(m_rayStart);
        raycastPoints.Add(m_rayEnd);
    }

    public bool TryGetUIModel(out TrackedDeviceModel model)
    {
        if (m_InputModule != null)
        {
            return m_InputModule.GetTrackedDeviceModel(this, out model);
        }

        model = new TrackedDeviceModel(-1);
        return false;
    }

    public bool IsOverUIGameObject()
    {
        return m_EnableUIInteraction && m_InputModule != null && TryGetUIModel(out var uiModel) && m_InputModule.IsPointerOverGameObject(uiModel.pointerId);
    }


    #endregion

}