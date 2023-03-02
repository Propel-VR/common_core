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
    TextMeshProUGUI _descText;
    [SerializeField]
    Color _currentTaskColour;
    [SerializeField]
    Button _button;
    [SerializeField]
    GameObject _warning, _caution;
    [SerializeField]
    TextMeshProUGUI _quantity,_smin;
    [SerializeField]
    GameObject cmpltObj; 

    public UnityEvent OnCompleteItem, OnFailItem, OnUnCommpleteItem;



    public void Complete()
    {

        _text.text = "<s>" + _text.text + "<s>";
        _quantity.gameObject.SetActive(false);
        cmpltObj.SetActive(true);
        OnCompleteItem?.Invoke();
    }

    public void UnComplete()
    {
        ColorBlock colors = _button.colors;
        colors.normalColor = Color.clear;
        _button.colors = colors;

        _warning.SetActive(false); 
        _caution.SetActive(false);

        _quantity.gameObject.SetActive(false);
        cmpltObj.SetActive(false);
        _smin.gameObject.SetActive(false);


        if (string.IsNullOrEmpty(_text.text))
            return;


        if (_text.text.Substring(0, 3).Equals("<s>"))
            _text.text = _text.text.Substring(3, _text.text.Length - 6);
        OnUnCommpleteItem?.Invoke();
    }

    public void SetButtonValue(int num)
    {
        Debug.Log("ADDED LISTENER TO BUTTON WITH ID: " + num);
        _button.onClick.AddListener(() => Tablet.Instance.ClickChecklist(num));
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

    public void SetText(string textString, string descText)
    {
        _text.text = textString;
        _descText.text = descText;

    }

    public void SetSmin(string sminNum)
    {
        _smin.gameObject.SetActive(true);
        _smin.text = sminNum;
    }

    public void SetQuantity(int complete, int total)
    {
        if (total > 1 && !cmpltObj.activeSelf)
        {

            _quantity.gameObject.SetActive(true);
            _quantity.text = complete + "/" + total;

        }
    }

    public void SetWarningCaution(bool warn, bool caut)
    {
        _warning.SetActive(warn);
        _caution.SetActive(caut);
    }

}
