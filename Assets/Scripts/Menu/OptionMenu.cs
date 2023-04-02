using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    public AudioMixer audioMixer;

    //[SerializeField] TMP_Dropdown m_Dropdown;

    public void SetActiveOptionsPanel() => optionsPanel.SetActive(!optionsPanel.activeSelf);

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    public void Sound()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    /*public void ResolutionOptions()
    {
        UnityEngine.Resolution[] resolutions = Screen.resolutions;
        List<string> m_DropOptions = new List<string> { };
        foreach (var res in resolutions)
        {
            m_DropOptions.Add(res.width + "x" + res.height);
            //Debug.Log(res.width + "x" + res.height + " : " + res.refreshRate);
        }
        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(m_DropOptions);
    }

    public void ChangeResolution()
    {
       int current = m_Dropdown.value;
       Screen.SetResolution(Screen.Resolutions[current].width, Screen.Resolutions[current].height, false, Screen.Resolutions[current].refreshRate);
    }*/
}
