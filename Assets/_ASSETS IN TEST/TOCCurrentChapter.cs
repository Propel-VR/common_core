using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TOCCurrentChapter : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI _text;
    [SerializeField]
    Image image;
    

    public void SetChapter(string chapterName, Sprite chapterImage)
    {
        _text.text = chapterName;
        image.sprite= chapterImage;
    }
    

}
