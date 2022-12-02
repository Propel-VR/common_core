using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private AudioMixer audioMixer;
    void Start()
    {
        UpdateSettigns();
    }

    public void UpdateSettigns ()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("volume", 1f)) * 20);
        /*if(controller.GetControlType() != PlayerControlType.FirstPerson)
        {
            switch(PlayerPrefs.GetInt("navigation", 0))
            {
                case 0: controller.SetControlType(PlayerControlType.Teleport); break;
                case 1: controller.SetControlType(PlayerControlType.Continuous); break;
            }
        }*/
        controller.SetTurnType((PlayerTurnType)PlayerPrefs.GetInt("turnMode", 0));
    }
}
