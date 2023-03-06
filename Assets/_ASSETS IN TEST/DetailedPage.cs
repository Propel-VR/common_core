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
    GameObject ReqContent;
    [SerializeField]
    Transform ReqParent;
    [SerializeField]
    GameObject _warningGo;
    [SerializeField]
    TextMeshProUGUI _warningText;

    [SerializeField]
    List<GameObject> _reqGOs;

    public void SetDetails(string smin, string zone, string  duration, string checkType, string rectification, Sprite[] rqSprites, string[] rqTxts)
    {

        
        foreach(GameObject go in _reqGOs)
        {
            GameObject.Destroy(go);
        }
        _reqGOs.Clear();

        Debug.Log("SHOWING DETAILS");

        _smin.text = smin;
        _zone.text = zone;
        _duration.text = duration;
        _typeOfCheck.text = checkType;
        _rectification.text = rectification;

        /*REQUIREMENT SPRITES*/

        for (int i = 0; i < rqSprites.Length; i++)
        {
            GameObject newGO = GameObject.Instantiate(ReqContent, ReqParent);

            newGO.GetComponentInChildren<Image>().sprite = rqSprites[i];
            newGO.GetComponentInChildren<TextMeshProUGUI>().text = rqTxts[i];

            newGO.SetActive(true);

            _reqGOs.Add(newGO);

        }
    }

    public void SetDetails(ChecklistTaskHelper.DetailsPageData data)
    {
        
        foreach (GameObject go in _reqGOs)
        {
            GameObject.Destroy(go);
        }
        _reqGOs.Clear();


        Debug.Log("SHOWING DETAILS");

        _smin.text = data.smin;
        _zone.text = data.zone;
        _duration.text = data.duration;
        _typeOfCheck.text = data.checkType;
        _rectification.text = data.rectification;
        if (string.IsNullOrEmpty(data.warningText))
        {
            _warningGo.SetActive(false);
        }
        else
        {
            _warningGo.SetActive(true);
            _warningText.text = data.warningText;
        }
        

        /*REQUIREMENT TEXT AND SPRITES*/

        for (int i = 0; i < data.requireConditions.Length; i++)
        {

            GameObject newGO = GameObject.Instantiate(ReqContent, ReqParent);

            newGO.GetComponentInChildren<Image>().sprite = data.requireConditions[i].image;
            newGO.GetComponentInChildren<TextMeshProUGUI>().text = data.requireConditions[i].text;

            newGO.SetActive(true);

            _reqGOs.Add(newGO);

        }

    }

}
