using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class EasyAnimatedPanelUI : MonoBehaviour
{
    [SerializeField] bool startOpened = true;
    const float fadeSpeed = 2f;
    const float fadeMinSize = 0.8f;

    private bool isOpened;
    private float value = 0f;

    private CanvasGroup group;
    private void Awake ()
    {
        if(!startOpened) gameObject.SetActive(false);
        isOpened = startOpened;
        value = startOpened ? 1f : 0f;
    }

    private void Update ()
    {
        group.blocksRaycasts = group.alpha >= 0.1f;
        group.interactable = group.alpha == 1f;

        value = Mathf.Clamp01(value + fadeSpeed * Time.deltaTime * (isOpened ? 1 : -1));
        transform.localScale = Vector3.one * Mathf.Lerp(fadeMinSize, 1f, value);

        if (!isOpened && value == 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void Open ()
    {
        isOpened = true;
        gameObject.SetActive(true);
    }

    public void Close ()
    {
        isOpened = false;

    }
}
