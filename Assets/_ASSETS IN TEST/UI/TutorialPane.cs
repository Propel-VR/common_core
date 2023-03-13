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
    TextMeshProUGUI _titleText, _bodyText, _subtitleText;

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
        _subtitleText.text = tutorialContents[currentTutorial].subtext;
        videoPlayer.clip = tutorialContents[currentTutorial].video;
        _titleText.text = tutorialContents[currentTutorial].title;
        _bodyText.text = tutorialContents[currentTutorial].body;
        transform.position = tutorialContents[currentTutorial].anchor.position;
        transform.rotation = tutorialContents[currentTutorial].anchor.rotation;
    }

    [System.Serializable]
    public struct TutorialContent
    {
        public string subtext;
        public string title; 
        public string body;
        public VideoClip video;
        public Transform anchor;
    }
}
