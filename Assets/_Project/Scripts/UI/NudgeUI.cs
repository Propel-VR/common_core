using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NudgeUI : MonoBehaviour
{
    
    [SerializeField] float fadeSpeed = 0.5f;
    private float value;
    private CanvasGroup group;
    private SpriteRenderer spriteRenderer;
    const float min = 0.8f;
    [SerializeField] private bool m_isOn = false;

    bool useSpriteRenderer;
    Vector3 initScale;

    public bool isOn
    {
        get { return m_isOn; }
        set
        {
            m_isOn = value;
            if(gameObject.activeSelf != m_isOn && m_isOn)
            {
                gameObject.SetActive(value);
            }
        }
    }

    private void Start ()
    {
        initScale = transform.localScale;
        group = GetComponent<CanvasGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        useSpriteRenderer = spriteRenderer != null;
        value = isOn ? 1 : -1;
    }

    public void FadeIn ()
    {
        isOn = true;
    }

    public void FadeOut ()
    {
        isOn = false;
    }

    private void Update ()
    {
        value = Mathf.Clamp01(value + (isOn ? 1 : -1) * fadeSpeed * Time.deltaTime);
        transform.localScale = initScale * Mathf.Lerp(min, 1f, value);
        if(useSpriteRenderer)
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = value;
            spriteRenderer.color = spriteColor;
        }
        else
        {
            group.alpha = value;
            group.blocksRaycasts = value != 0f;
            group.interactable = value == 1f;
        }
    }
}
