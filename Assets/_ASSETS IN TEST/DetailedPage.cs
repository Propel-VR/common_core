using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailedPage : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _smin, _zone, _duration, _typeOfCheck, _rectification;
    [SerializeField]
    Image[] _reqSprites;
    [SerializeField]
    TextMeshProUGUI[] _reqText;

    public void SetDetails(string smin, string zone, string  duration, string checkType, string rectification, Sprite[] rqSprites, string[] rqTxts)
    {

        _smin.text = smin;
        _zone.text = zone;
        _duration.text = duration;
        _typeOfCheck.text = checkType;
        _rectification.text = rectification;

        /*REQUIREMENT SPRITES*/
        for(int i=0; i<rqSprites.Length; i++)
        {
            _reqSprites[i].sprite= rqSprites[i];
        }
        for(int i=rqSprites.Length; i<_reqSprites.Length; i++)
        {
            //hide this one
            _reqSprites[i].sprite = null;
        }

        /*REQUIREMENT TEXT*/
        for (int i = 0; i < rqTxts.Length; i++)
        {
            _reqText[i].text= rqTxts[i];
        }
        for (int i = rqTxts.Length; i < _reqText.Length; i++)
        {
            //hide this one
            _reqText[i].text = "";
        }

    }

}
