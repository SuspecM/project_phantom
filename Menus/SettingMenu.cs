using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class SettingMenu : MonoBehaviour
{
    public AudioMixerGroup mainVolume;
    public AudioMixerGroup musicVolume;
    public AudioMixerGroup sfxVolume;
    public TextMeshProUGUI mainPercent;
    public TextMeshProUGUI musicPercent;
    public TextMeshProUGUI sfxPercent;
    public GameObject selectedLow;
    public GameObject selectedMed;
    public GameObject selectedHigh;
    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreen;
    public Slider verticalSensitivitySlider;
    public Slider horizontalSensitivitySlider;
    public TextMeshProUGUI vertValue;
    public TextMeshProUGUI horyValue;
    public PersistentGameManager gameManager;

    public bool fromMainMenu = true;
    public GameObject menuToOpenAfterClose;

    [SerializeField] private EventReference uiClick;

    public PlayerInput playerInput;

    public GameObject firstItem;
    public GameObject lastSelectedItem;
    

    private void Awake()
    {
        playerInput.actions["Cancel"].performed += OnCancel;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && gameManager.lastUsedDevice is Gamepad)
        {
            if (Mouse.current.delta.ReadValue() == Vector2.zero) // No mouse movement
            {
                if (lastSelectedItem != null)
                    EventSystem.current.SetSelectedGameObject(lastSelectedItem);
                else
                {
                    lastSelectedItem = firstItem;
                    EventSystem.current.SetSelectedGameObject(lastSelectedItem);
                }
            }
        }
        else
        {
            lastSelectedItem = EventSystem.current.currentSelectedGameObject;
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        CloseSettings();
    }

    private void OnEnable()
    {
        //gameManager.playerInput.SwitchCurrentActionMap("UI");

        switch (gameManager.qualityLevel)
        {
            case 0: selectedLow.SetActive(true); selectedMed.SetActive(false); selectedHigh.SetActive(false); break;
            case 1: selectedLow.SetActive(false); selectedMed.SetActive(true); selectedHigh.SetActive(false); break;
            case 2: selectedLow.SetActive(false); selectedMed.SetActive(false); selectedHigh.SetActive(true); break;
        }

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        RuntimeManager.GetBus("bus:/").getVolume(out float audio);
        SetVolume(audio);

        RuntimeManager.GetBus("bus:/Music").getVolume(out float music);
        SetMusicVolume(music);

        RuntimeManager.GetBus("bus:/Sfx").getVolume(out float sfx);
        SetSfxVolume(sfx);

        fullscreen.isOn = gameManager.fullScreen;

        verticalSensitivitySlider.value = gameManager.verticalSensitivity;
        horizontalSensitivitySlider.value = gameManager.horizontalSensitivity;

        lastSelectedItem = firstItem;
        EventSystem.current.SetSelectedGameObject(lastSelectedItem);
    }

    public void ChangeVerticalSensitivity(float value)
    {
        gameManager.verticalSensitivity = value;
        vertValue.text = value.ToString();
    }

    public void ChangeHorizontalSensitivity(float value)
    {
        gameManager.horizontalSensitivity = value;
        horyValue.text = value.ToString();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayTheSound();
    }

    public void SetVolume(float volume)
    {
        RuntimeManager.GetBus("bus:/").setVolume(volume);
        mainPercent.text = $"{(Mathf.Round(volume * 100)).ToString()}%";
    }

    public void SetMusicVolume(float volume)
    {
        RuntimeManager.GetBus("bus:/Music").setVolume(volume);
        musicPercent.text = $"{(Mathf.Round(volume * 100)).ToString()}%";
    }
    
    public void SetSfxVolume(float volume)
    {
        RuntimeManager.GetBus("bus:/Sfx").setVolume(volume);
        sfxPercent.text = $"{(Mathf.Round(volume * 100)).ToString()}%";
    }

    public void SetQuality(int qualityIndex)
    {
        switch (qualityIndex)
        {
            case 0: selectedLow.SetActive(true); selectedMed.SetActive(false); selectedHigh.SetActive(false); break;
            case 1: selectedLow.SetActive(false); selectedMed.SetActive(true); selectedHigh.SetActive(false); break;
            case 2: selectedLow.SetActive(false); selectedMed.SetActive(false); selectedHigh.SetActive(true); break;
        }
        gameManager.qualityLevel = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayTheSound();
    }

    public void SetFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        gameManager.fullScreen = fullscreen;
        PlayTheSound();
    }

    public void PlayTheSound()
    {
        FMODSoundManager.instance.PlayOneShot(uiClick, transform.position, 0f, 0f);
    }

    public void CloseSettings()
    {
        PlayTheSound();
        gameManager.saveManager.SaveSettings("Manual save");

        if (!fromMainMenu)
        {
            menuToOpenAfterClose.SetActive(true);

            FindFirstObjectByType<PauseMenu>().optionsIsUp = false;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(FindFirstObjectByType<PauseMenu>().optionsButton);
        }
        else
        {
            FindFirstObjectByType<MainMenu>().optionsBox = false;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(FindFirstObjectByType<MainMenu>().lastSelectedUiElement);
        }

        gameManager.CloseSettings();
    }

}
