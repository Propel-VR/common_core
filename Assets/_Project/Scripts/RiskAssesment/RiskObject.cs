using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RiskObject : MonoBehaviour, IInteractableObject
{
    static readonly string layerName = "Interactable";
    static readonly string tagName = "Risk";
    static readonly int outlineWidth = 10;
    static readonly Color outlineColor = new(0f, 1f, 140f/255f);
    static readonly Color outlineDisabledColor = new(0.5f, 0.5f, 0.5f);
    static readonly float minTimeBeforeFadeIn = 0.4f;
    static readonly float fadeInTime = 0.6f;

    private bool isHovering = false;
    private float initHoverTime;
    private float fadeInValue;



    [SerializeField] RiskAsset asset;
    [SerializeField] UnityEvent onInteract;
    [SerializeField] UnityEvent onAssesRisk;
    [SerializeField] Transform contextPoint;
    private Outline outline;

    private void Start ()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineWidth = outlineWidth;
        outline.OutlineColor = outlineColor;
        outline.OutlineMode = Outline.Mode.None;

        gameObject.layer = LayerMask.NameToLayer(layerName); 
        //gameObject.tag = tagName;

        RiskAssesmentManager.RegisterObject(this);

        if(contextPoint == null && asset.Type != RiskType.NotHazard)
        {
            Debug.LogError("Context point missing for: " + this.name, this);
        }
    }

    private void Update ()
    {
        if(isHovering)
        {
            float hoverDelayValue = Mathf.Clamp01((Time.unscaledTime - initHoverTime) / minTimeBeforeFadeIn);

            if(hoverDelayValue < 1f)
            {
                fadeInValue = Mathf.Clamp01(fadeInValue - Time.deltaTime / fadeInTime);
            }
            if(hoverDelayValue == 1f && outline.OutlineColor.a != 1f)
            {
                fadeInValue = Mathf.Clamp01(fadeInValue + Time.deltaTime / fadeInTime);
            }
        }
        // Fade out
        else if(fadeInValue > 0f)
        {
            fadeInValue = Mathf.Clamp01(fadeInValue - Time.deltaTime / fadeInTime);
            if (fadeInValue == 0)
            {
                outline.OutlineMode = Outline.Mode.None;
            }
        }
        outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, fadeInValue);
    }

    public RiskAsset Asset
    {
        get { return asset; }
    }

    public void OnInteract ()
    {
        if(Time.unscaledTime - initHoverTime > minTimeBeforeFadeIn && RiskAssesmentManager.CanInteractWithObject(this))
        {
            RiskAssesmentManager.InteractWithObject(this);
            onInteract?.Invoke();
        }
    }

    public void OnBeginHover ()
    {
        isHovering = true;
        if (fadeInValue == 0f)
            initHoverTime = Time.unscaledTime;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        if (RiskAssesmentManager.CanInteractWithObject(this)) outline.OutlineColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, fadeInValue);
        else outline.OutlineColor = new Color(outlineDisabledColor.r, outlineDisabledColor.g, outlineDisabledColor.b, fadeInValue);
    }

    public void OnEndHover ()
    {
        isHovering = false;
    }

    public bool IsInteractable ()
    {
        return (Time.unscaledTime - initHoverTime > minTimeBeforeFadeIn) &&
        ActivityManager.CurrentActivityType == ActivityType.RiskAssesment &&
        RiskAssesmentManager.CanInteractWithObject(this);
    }

    public void OnAssesRisk ()
    {
        onAssesRisk?.Invoke();
    }

    Transform[] childs;
    int[] childLayers;
    public void SetContextState (bool contextEnabled)
    {
        if(contextEnabled)
        {
            gameObject.layer = LayerMask.NameToLayer("Highlight");
            childs = gameObject.GetComponentsInChildren<Transform>();
            childLayers = new int[childs.Length];
            for(int i = 0; i < childs.Length; i++)
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
            gameObject.layer = LayerMask.NameToLayer(layerName);
            if(childLayers != null)
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

    public Vector3 GetContextPoint ()
    {
        return contextPoint.position;
    }
}
