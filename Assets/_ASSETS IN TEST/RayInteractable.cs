using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RayInteractable : MonoBehaviour, IInteractableObject
{
    public int outlineWidth = 5;
    static readonly float minTimeBeforeFadeIn = 0.4f;
    static readonly float fadeInTime = 0.6f;

    private bool isHovering = false;
    private float initHoverTime;
    private float fadeInValue;


    public Transform Player; // mike add for checking distance
    [SerializeField] UnityEvent onInteract;
    [SerializeField] Transform contextPoint;
    [SerializeField] Color outlineColor = new(0f, 1f, 140f / 255f);

    private Outline outline;
    bool stopInteracting = false;
    [HideInInspector]
    public bool forceHighlight = false;

    public void SetOutlineWidth() // mike added function to check distance from player, and set outline width based on it
    {
        if(Player)
        {
            float dist = Vector3.Distance(Player.position, transform.position);
            print("Distance to other: " + dist);
            
            if(dist <= 10.0f) { outlineWidth = 2; } else if ( dist >= 30.0f) { outlineWidth = 7; } else { outlineWidth = 4; }
            outline.OutlineWidth = outlineWidth;
        }
    }

    private void Awake()
    {

        outline = gameObject.AddComponent<Outline>();
    }

    private void Start()
    {

        outline.OutlineWidth = outlineWidth;
        outline.OutlineColor = outlineColor;
        outline.OutlineMode = Outline.Mode.None;

        if (contextPoint == null)
        {
            contextPoint = GameObject.Instantiate(new GameObject(), this.transform).transform;
        }
    }

    private void Update()
    {
        if (isHovering)
        {
            SetOutlineWidth();
            Debug.Log("IM BEING HOVERED");
            float hoverDelayValue = Mathf.Clamp01((Time.unscaledTime - initHoverTime) / minTimeBeforeFadeIn);

            if (hoverDelayValue < 1f)
            {
                fadeInValue = Mathf.Clamp01(fadeInValue - Time.deltaTime / fadeInTime);
            }
            if (hoverDelayValue == 1f && outline.OutlineColor.a != 1f)
            {
                fadeInValue = Mathf.Clamp01(fadeInValue + Time.deltaTime / fadeInTime);
            }
        }
        // Fade out
        else if (fadeInValue > 0f)
        {
            fadeInValue = Mathf.Clamp01(fadeInValue - Time.deltaTime / fadeInTime);
            if (fadeInValue == 0)
            {
                outline.OutlineMode = Outline.Mode.None;
            }
        }

        if (forceHighlight)
        {
            SetOutlineWidth();
            fadeInValue = 1;
            outline.OutlineMode = Outline.Mode.OutlineAll;

        }
        outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, fadeInValue);
    }


    public void OnInteract()
    {
        Debug.Log("IM BEING INTERACTED");
        if (!stopInteracting && Time.unscaledTime - initHoverTime > minTimeBeforeFadeIn)
        {
            onInteract?.Invoke();
        }
    }

    public void ClearInteractions()
    {
        onInteract.RemoveAllListeners();
        stopInteracting = true;
    }

    public void OnBeginHover()
    {
        if (!stopInteracting)
        {
            isHovering = true;
            if (fadeInValue == 0f)
                initHoverTime = Time.unscaledTime;
            outline.OutlineMode = Outline.Mode.OutlineAll;

            outline.OutlineColor = outlineColor;
        }
    }

    public void OnEndHover()
    {
        isHovering = false;
    }

    public bool IsInteractable()
    {
        return (Time.unscaledTime - initHoverTime > minTimeBeforeFadeIn);
    }

    Transform[] childs;
    int[] childLayers;
    public void SetContextState(bool contextEnabled)
    {
        if (contextEnabled)
        {
            gameObject.layer = LayerMask.NameToLayer("Highlight");
            childs = gameObject.GetComponentsInChildren<Transform>();
            childLayers = new int[childs.Length];
            for (int i = 0; i < childs.Length; i++)
            {
                if (childs[i] == transform) continue; // Skip if self
                childLayers[i] = childs[i].gameObject.layer;
                childs[i].gameObject.layer = gameObject.layer;
            }
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = new Color(1, 1, 1, 1f);
        }
        else
        {
            //gameObject.layer = LayerMask.NameToLayer(layerName);
            if (childLayers != null)
            {
                for (int i = 0; i < childLayers.Length; i++)
                {
                    if (childs[i] == transform) continue; // Skip if self
                    childs[i].gameObject.layer = childLayers[i];
                }
            }

            outline.OutlineMode = Outline.Mode.None;
        }
    }

    public Vector3 GetContextPoint()
    {
        return contextPoint.position;
    }
}