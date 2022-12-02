using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] SettingsManager settingsManager;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle snapTurnToggle;
    [SerializeField] Image[] checkmarks;
    private int navigationMethod;

    private void OnEnable ()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", volumeSlider.value);
        //snapTurnToggle.isOn = PlayerPrefs.GetInt("snapTurn", 1) > 0;
        SelectNavigation(PlayerPrefs.GetInt("turnMode", 0));
    }


    public void SelectNavigation (int index)
    {
        navigationMethod = index;
        checkmarks[0].enabled = index == 0;
        checkmarks[1].enabled = index == 1;
    }

    public void ApplySettings ()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        //PlayerPrefs.SetInt("snapTurn", snapTurnToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("turnMode", navigationMethod);
        PlayerPrefs.Save();
        settingsManager.UpdateSettigns();
    }
}
