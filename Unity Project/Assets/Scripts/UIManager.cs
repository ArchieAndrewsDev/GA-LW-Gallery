using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject settingsButtonRoot;
    public GameObject settingsWindowRoot;

    public Slider volumeSlider;
    public Dropdown dropDown;

    public void OpenSettings()
    {
        settingsButtonRoot.SetActive(false);
        settingsWindowRoot.SetActive(true);
        UserControl._instance.runCamera = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseSettings()
    {
        settingsButtonRoot.SetActive(true);
        settingsWindowRoot.SetActive(false);
        UserControl._instance.runCamera = true;
    }

    public void SetVolume()
    {
        WorldManager._instance.SetVideoPlayerVolume(volumeSlider.value);
    }

    public void SetQualitySettings()
    {
        int id = dropDown.value;
        Debug.Log(id);
        switch (id)
        {
            case 0:
                WorldManager._instance.ChangeQualitySettings(VideoQuality.High);
                break;
            case 1:
                WorldManager._instance.ChangeQualitySettings(VideoQuality.Medium);
                break;
            case 2:
                WorldManager._instance.ChangeQualitySettings(VideoQuality.Low);
                break;
        }
    }
}
