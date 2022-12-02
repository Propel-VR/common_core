using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EasyInteractable : MonoBehaviour, IInteractableObject
{
    static readonly string layerName = "Interactable";
    static readonly int outlineWidth = 6;
    static readonly Color outlineColor = new(0f, 1f, 140f/255f);
    static readonly Color outlineDisabledColor = new(0.5f, 0.5f, 0.5f);
    static readonly float minTimeBeforeFadeIn = 0f;
    static readonly float fadeInTime = 1f;

    private bool isHovering = false;
    private float initHoverTime;
    private float fadeInValue;




    [SerializeField] UnityEvent OnInteractEvent;
    private Outline outline;

    private void Start ()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineWidth = outlineWidth;
        outline.OutlineColor = outlineColor;
        outline.OutlineMode = Outline.Mode.None;

        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    private void Update ()
    {
        if (isHovering)
        {
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
        outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, fadeInValue);
    }

    public void OnInteract ()
    {
        if (Time.unscaledTime - initHoverTime > minTimeBeforeFadeIn)
        {
            OnInteractEvent?.Invoke();
        }
    }

    public void OnBeginHover ()
    {
        isHovering = true;
        if (fadeInValue == 0f)
            initHoverTime = Time.unscaledTime;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0f);
    }

    public void OnEndHover ()
    {
        isHovering = false;
    }

    public bool IsInteractable ()
    {
        return Time.unscaledTime - initHoverTime > minTimeBeforeFadeIn;
    }
}
