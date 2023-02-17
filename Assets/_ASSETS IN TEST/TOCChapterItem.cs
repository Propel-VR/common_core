using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TOCChapterItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _chapterNameText;
    [SerializeField]
    TextMeshProUGUI _chapterProgressText;
    [SerializeField]
    Image _chapterBackground;
    [SerializeField]
    Color _currentTaskColour;
    [SerializeField]
    Button _button;

    public UnityEvent OnCompleteItem, OnFailItem, OnUnCommpleteItem;



    public void Complete()
    {

        _chapterNameText.text = "<s>" + _chapterNameText.text + "<s>";
        OnCompleteItem?.Invoke();
    }

    public void UnComplete()
    {
        ColorBlock colors = _button.colors;
        colors.normalColor = Color.clear;
        _button.colors = colors;
        transform.GetChild(0).gameObject.SetActive(false);
        
        if (string.IsNullOrEmpty(_chapterNameText.text))
            return;


        if (_chapterNameText.text.Substring(0, 3).Equals("<s>"))
            _chapterNameText.text = _chapterNameText.text.Substring(3, _chapterNameText.text.Length - 6);
        OnUnCommpleteItem?.Invoke();
    }

    public void Fail()
    {
        OnFailItem?.Invoke();
    }

    public void Current()
    {
        

    }


    public void SetTextAndImage(string textString, Sprite img)
    {
        _chapterNameText.text = textString;
        _chapterProgressText.text = "";
        if(!string.IsNullOrEmpty(textString))
        {
            ColorBlock colors = _button.colors;
            colors.normalColor = Color.white;
            _chapterBackground.sprite= img;
            _button.colors = colors;
            transform.GetChild(0).gameObject.SetActive(true);

        }


    }

    public void SetProgress(int current, int total)
    {
        _chapterProgressText.text=current+" of "+total+" complete";

        if(current==total)
        {
            _chapterNameText.text = "<s>" + _chapterNameText.text + "<s>";
            _chapterProgressText.text = "<s>" + _chapterProgressText.text + "<s>";

        }
    }

}
