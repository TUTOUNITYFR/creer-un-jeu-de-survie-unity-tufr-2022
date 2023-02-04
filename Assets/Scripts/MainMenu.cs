using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public Button loadGameButton;

    public Button clearSavedDataButton;

    [SerializeField]
    private Dropdown resolutionsDropdown;

    [SerializeField]
    private Dropdown qualitiesDropdown;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private Slider soundSlider;

    [SerializeField]
    private GameObject optionsPanel;

    [SerializeField]
    private Toggle fullScreenToggle;

    public static bool loadSavedData;

    void Start()
    {
        // Initialisation du slider de volume
        audioMixer.GetFloat("Volume", out float soundValueForSlider);
        soundSlider.value = soundValueForSlider;

        // Initialisation du bouton de chargement de données
        bool saveFileExist = System.IO.File.Exists(Application.persistentDataPath + "/SavedData.json");
        loadGameButton.interactable = saveFileExist;
        clearSavedDataButton.interactable = saveFileExist;

        // Init des qualités graphiques
        string[] qualities = QualitySettings.names;
        qualitiesDropdown.ClearOptions();

        List<string> qualityOptions = new List<string>();
        int currentQualityIndex = 0;

        for (int i = 0; i < qualities.Length; i++)
        {
            qualityOptions.Add(qualities[i]);

            if(i == QualitySettings.GetQualityLevel())
            {
                currentQualityIndex = i;
            }
        }

        qualitiesDropdown.AddOptions(qualityOptions);
        qualitiesDropdown.value = currentQualityIndex;
        qualitiesDropdown.RefreshShownValue();

        // Initialisation des différentes résolutions
        Resolution[] resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " (" + resolutions[i].refreshRate + " Hz)";
            resolutionOptions.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(resolutionOptions);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        // Init du Toggle Full Screen
        fullScreenToggle.isOn = Screen.fullScreen;
    }

    public void NewGameButton()
    {
        loadSavedData = false;
        SceneManager.LoadScene("Scene");
    }

    public void LoadGameButton()
    {
        loadSavedData = true;
        SceneManager.LoadScene("Scene");
    }

    public void LoadMainMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void ClearSavedData()
    {
        System.IO.File.Delete(Application.persistentDataPath + "/SavedData.json");
        loadGameButton.interactable = false;
        clearSavedDataButton.interactable = false;
    }

    public void EnableDisableOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

}
