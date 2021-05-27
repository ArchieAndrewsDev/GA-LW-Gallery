using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    public GameObject settingsButtonRoot;
    public GameObject settingsWindowRoot;

    public Slider volumeSlider;
    public Dropdown dropDown;

    public GameObject urlPrompt, movePrompt;
    public Text metaTitle, metaText;
    public GameObject metaRoot;

    private void Awake()
    {
        _instance = this;
    }

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
        }
    }

    public void ShowPrompt(PromptType type, bool show = true)
    {
        switch (type)
        {
            case PromptType.URL:
                urlPrompt.SetActive(show);
                break;
            case PromptType.Move:
                movePrompt.SetActive(show);
                break;
        }
    }

    public void SetMetaData(string title, string text)
    {
        metaRoot.SetActive(true);
        metaTitle.text = title;
        metaText.text = text;
    }

    public void EnableMetaData(bool show = true)
    {
        metaRoot.SetActive(show);
    }
}

public enum PromptType { URL, Move}