using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ProgressBar : MonoBehaviour
{

    public Slider slider;
    TextMeshProUGUI scoreText;
    public GameObject textCounter; 

    private string stringText;
    private int textInt;

    private void Awake()
    {
        scoreText = textCounter.GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        HandleConversion(scoreText);
        Debug.Log(textInt);
        slider.value = textInt;
        
    }

    

    public void HandleConversion(TextMeshProUGUI scoreText)
    {
        stringText = scoreText.GetParsedText();
        int.TryParse(stringText, out textInt);
        
    }

}
