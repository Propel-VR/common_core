using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChecklistItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _text;

    public UnityEvent OnCompleteItem, OnFailItem, OnUnCommpleteItem;



    public void Complete()
    {
        _text.text = "</s>" + _text.text + "</s>";
        OnCompleteItem?.Invoke();
    }

    public void UnComplete()
    {
        if (_text.text.Substring(0, 4).Equals("</s>"))
            _text.text = _text.text.Substring(4, _text.text.Length - 8);
        OnUnCommpleteItem?.Invoke();
    }

    public void Fail()
    {
        OnFailItem?.Invoke();
    }

    public void SetText(string textString)
    {
        _text.text = textString;

    }

}
