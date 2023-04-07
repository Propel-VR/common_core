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
    TextMeshProUGUI _titleText, _bodyText, _subtitleText, _panelNum;

    [SerializeField]
    VideoPlayer videoPlayer;

    [SerializeField]
    TutorialContent[] tutorialContents;

    [SerializeField]
    GameObject skipBtn;
    [SerializeField]
    TextMeshProUGUI nextBtn;

    int currentTutorial = 0;

    private void Start()
    {
        PlayTutorial();
    }

    public void NextTutorial()
    {
        skipBtn.SetActive(false);
        currentTutorial++;

        if(currentTutorial>= tutorialContents.Length)
        {
            gameObject.SetActive(false);
            pool.StartPool();
        }
        else
        {
            if ((currentTutorial == tutorialContents.Length - 1))
                nextBtn.text = "Finish";

            PlayTutorial();
        }
    }

    private void PlayTutorial()
    {
        _subtitleText.text = tutorialContents[currentTutorial].subtext;
        videoPlayer.clip = tutorialContents[currentTutorial].video;
        _titleText.text = tutorialContents[currentTutorial].title;
        _bodyText.text = tutorialContents[currentTutorial].body;
        _panelNum.text = (currentTutorial + 1) + " / " + tutorialContents.Length;
        transform.position = tutorialContents[currentTutorial].anchor.position;
        transform.rotation = tutorialContents[currentTutorial].anchor.rotation;
    }

    public void SkipTutorial()
    {
        gameObject.SetActive(false);
        pool.StartPool();
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
