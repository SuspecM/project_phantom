using FMOD.Studio;
using FMODUnity;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject[] mainMenuBackgroundCamera;
    public GameObject[] menuBackgrounds;

    private PlayerInput playerInput;

    public EventReference[] mainMenuMusic;
    public EventInstance playingMusic;

    public EventReference uiClick;

    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject popupDialog;
    public GameObject playButton;
    public GameObject newPlayButton;
    public GameObject quitButton;

    public bool dialogueBox;
    public bool optionsBox;

    public PersistentGameManager gameManager;

    public GameObject WarningPopup;
    public TextMeshProUGUI WarningText;
    public GameObject DialogPopup;
    public TextMeshProUGUI DialogText;

    public GameObject yesUiElement;
    public GameObject noUiElement;
    public GameObject lastSelectedUiElement;

    public bool alreadyHasSave;
    public bool cont;
    public bool quit;

    public GameObject defaultButton; // Assign your default button in the Inspector

    public void Awake()
    {
        playerInput = FindAnyObjectByType<PlayerInput>();
        playerInput.actions.FindActionMap("UI").Enable();
        playerInput.actions["Cancel"].performed += OnCancel;

        optionsBox = false;
        dialogueBox = false;

        //optionsMenu.SetActive(false);
        popupDialog.SetActive(false);

        playButton.SetActive(true);
        newPlayButton.SetActive(true);

        DialogPopup.SetActive(false);
        WarningPopup.SetActive(false);

        lastSelectedUiElement = newPlayButton;
    }

    private void Start()
    {
        gameManager = FindFirstObjectByType<PersistentGameManager>();

        playingMusic = RuntimeManager.CreateInstance(mainMenuMusic[gameManager.mainMenuBg]);
        playingMusic.start();

        for (int i = 0; i < mainMenuBackgroundCamera.Length; i++)
        {
            if (i == gameManager.mainMenuBg)
            {
                mainMenuBackgroundCamera[i].SetActive(true);
                menuBackgrounds[i].SetActive(true);
            }
            else
            {
                mainMenuBackgroundCamera[i].SetActive(false);
                menuBackgrounds[i].SetActive(false);
            }
        }

        try
        {
            //gameManager.saveManager.LoadSettings("Startup");
            gameManager.saveManager.LoadProfile("Startup");
            alreadyHasSave = gameManager.hasProfile;
        }
        catch (Exception e)
        {
            Debug.LogError("No game manager found! " + e.Message);
        }

        playButton.SetActive(alreadyHasSave);

        EventSystem.current.SetSelectedGameObject(newPlayButton);
    }

    void Update()
    {
        // Check if no button is currently selected
        if (EventSystem.current.currentSelectedGameObject == null && gameManager.lastUsedDevice is Gamepad)
        {
            if (dialogueBox)
            {
                EventSystem.current.SetSelectedGameObject(noUiElement);
            }
            else if (!optionsBox)
            {
                if (lastSelectedUiElement != null)
                    EventSystem.current.SetSelectedGameObject(lastSelectedUiElement);
                else
                {
                    lastSelectedUiElement = newPlayButton;
                    EventSystem.current.SetSelectedGameObject(lastSelectedUiElement);
                }
            }
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (dialogueBox)
        {
            CancelDialogue();
        }
    }

    public void StartNewGame()
    {
        PlayTheSound();
        DialogPopup.SetActive(true);

        dialogueBox = true;

        lastSelectedUiElement = newPlayButton;

        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(noUiElement);

        if (!alreadyHasSave)
        {
            DialogText.text = "You are about to start a new game. Proceed?";
        }
        else
        {
            DialogText.text = "You are about to start a new game. Any existing save and progress will be deleted! Achievement progress will be kept. Proceed?";
        }
    }

    public void ContinueGame()
    {
        PlayTheSound();
        cont = true;
        DialogPopup.SetActive(true);

        dialogueBox = true;

        lastSelectedUiElement = playButton;

        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(yesUiElement);

        DialogText.text = "Are you sure you want to continue your existing game?";
    }

    public void PopupDialogueForNewGame()
    {
        PlayTheSound();
        DialogPopup.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        if (quit)
        {
            Application.Quit();
        }
        else if (cont)
        {
            playerInput.actions.FindActionMap("UI").Disable();
            playingMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            gameManager.saveManager.LoadProfile("Continue game button pressed");
            gameManager.playerCopied = true;
            gameManager.LoadLevel(gameManager.currentLevel, "MainMenu", gameManager.currentLevel);
        }
        else
        {
            playingMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            try
            {
                playerInput.actions.FindActionMap("UI").Disable();
                gameManager.saveManager.SaveProfile("Starting new game");
                gameManager.saveManager.LoadProfile("Starting new game, refreshing game manager data");
                gameManager.LoadLevel(gameManager.currentLevel, "MainMenu", gameManager.currentLevel);
            }
            catch (Exception e)
            {
                Debug.LogError("No game manager found! " + e.Message);
            }
        }
    }

    public void CancelDialogue()
    {
        PlayTheSound();
        DialogPopup.SetActive(false);
        quit = false;

        dialogueBox = false;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastSelectedUiElement);
    }

    public void SaveandBack()
    {
        gameManager.saveManager.SaveSettings("Player initiated save");
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        PlayTheSound();

        optionsBox = false;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastSelectedUiElement);
    }

    public void SaventandBack()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        gameManager.saveManager.LoadSettings("Revert settings");
        PlayTheSound();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastSelectedUiElement);
    }

    public void OpenOptions()
    {
        PlayTheSound();
        optionsBox = true;
        EventSystem.current.SetSelectedGameObject(null);
        gameManager.settingMenu.GetComponent<SettingMenu>().fromMainMenu = true;
        gameManager.OpenSettings();

        lastSelectedUiElement = optionsMenu;

    }

    public void QuitGame()
    {
        PlayTheSound();
        DialogPopup.SetActive(true);
        quit = true;
        DialogText.text = "Are you sure you want to quit to desktop?";

        lastSelectedUiElement = quitButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(noUiElement);
    }

    public void PlayTheSound()
    {
        FMODSoundManager.instance.PlayOneShot(uiClick, new Vector3(0, 0, 0), 0f, 0f);
    }
}
