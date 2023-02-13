using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChecklistItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _text;
    [SerializeField]
    Color _currentTaskColour;
    [SerializeField]
    Button _button;
    [SerializeField]
    GameObject _warning, _caution;

    public UnityEvent OnCompleteItem, OnFailItem, OnUnCommpleteItem;



    public void Complete()
    {

        _text.text = "<s>" + _text.text + "<s>";
        OnCompleteItem?.Invoke();
    }

    public void UnComplete()
    {
        ColorBlock colors = _button.colors;
        colors.normalColor = Color.clear;
        _button.colors = colors;

        _warning.SetActive(false); 
        _caution.SetActive(false);

        if (string.IsNullOrEmpty(_text.text))
            return;


        if (_text.text.Substring(0, 3).Equals("<s>"))
            _text.text = _text.text.Substring(3, _text.text.Length - 6);
        OnUnCommpleteItem?.Invoke();
    }

    public void Fail()
    {
        OnFailItem?.Invoke();
    }

    public void Current()
    {
        ColorBlock colors = _button.colors;
        colors.normalColor = _currentTaskColour;
        _button.colors = colors;
    }

    public void SetText(string textString)
    {
        _text.text = textString;

    }

    public void SetWarningCaution(bool warn, bool caut)
    {
        _warning.SetActive(warn);
        _caution.SetActive(caut);
    }

}
