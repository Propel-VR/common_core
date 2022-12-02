using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeOccluderSystem : MonoBehaviour
{
    [SerializeField] Image fadeInImage;
    [SerializeField] float fadeSpeed = 1f;

    private Color occludeColor;

    private void Awake ()
    {
        occludeColor = fadeInImage.color;
    }

    public void OnUpdate(float alpha)
    {
        fadeInImage.color = new Color(occludeColor.r, occludeColor.g, occludeColor.b, alpha);
    }
}
