using PaintIn3D;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(P3dChannelCounterText))]
public class CleanableObject : MonoBehaviour
{

    #region serialized private
    [SerializeField] TextMeshProUGUI _text;
    #endregion

    #region private fields
    P3dChannelCounterText _counterText;
    #endregion


    private void Awake()
    {
        _counterText = GetComponent<P3dChannelCounterText>();
        _counterText.OnString.AddListener(SetText);
    }

    private void SetText(string text)
    {
        _text.text = text;

    }


}
