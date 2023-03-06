using CamhOO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialPane : MonoBehaviour
{

    [SerializeField]
    TaskPool pool;

    [SerializeField]
    TextMeshProUGUI _titleText, _bodyText;

    [SerializeField]
    VideoPlayer videoPlayer;

    [SerializeField]
    TutorialContent[] tutorialContents;

    int currentTutorial = 0;

    private void Start()
    {
        PlayTutorial();
    }

    public void NextTutorial()
    {
        currentTutorial++;
        if(currentTutorial>= tutorialContents.Length)
        {
            gameObject.SetActive(false);
            pool.StartPool();
        }
        else
        {
            PlayTutorial();
        }
    }

    private void PlayTutorial()
    {
        videoPlayer.clip = tutorialContents[currentTutorial].video;
        _titleText.text = tutorialContents[currentTutorial].title;
        _bodyText.text = tutorialContents[currentTutorial].body;
    }

    [System.Serializable]
    public struct TutorialContent
    {
        public string title; 
        public string body;
        public VideoClip video;
    }
}
