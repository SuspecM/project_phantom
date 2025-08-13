using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public NewCharacterController player;

    public GameObject pauseFirstButton;
    public GameObject lastSelectedButton;
    public EventReference uiClick;

    public GameObject optionsButton;
    public bool optionsIsUp;

    private PersistentGameManager gameManager;

    public PlayerInput playerInput;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<PersistentGameManager>();
        playerInput = FindAnyObjectByType<PlayerInput>();
    }

    public void BringUpPauseMenu()
    {
        playerInput.actions.FindActionMap("UI").Enable();
        playerInput.actions["Cancel"].performed += OnCancel;

        EventSystem.current.SetSelectedGameObject(null);
        lastSelectedButton = pauseFirstButton;
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (optionsIsUp)
        {
            FindFirstObjectByType<SettingMenu>().CloseSettings();
        }
        else
        {
            Resume();
        }
    }

    void Update()
    {
        // Check if no button is currently selected
        if (EventSystem.current.currentSelectedGameObject == null && gameManager.lastUsedDevice is Gamepad)
        {
            if (!optionsIsUp)
            {
                EventSystem.current.SetSelectedGameObject(lastSelectedButton);
            }
        }
        else
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                lastSelectedButton = EventSystem.current.currentSelectedGameObject;
            }
        }

        //Debug.Log(optionsIsUp);
    }

    public void Resume()
    {
        PlayTheSound();
        playerInput.actions.FindActionMap("Movement").Enable();
        playerInput.actions.FindActionMap("UI").Disable();
        player.Pause();
    }

    public void OpenOptions()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<PersistentGameManager>();
        }
        optionsIsUp = true;
        PlayTheSound();
        gameManager.settingMenu.GetComponent<SettingMenu>().fromMainMenu = false;
        gameManager.settingMenu.GetComponent<SettingMenu>().menuToOpenAfterClose = this.gameObject;
        gameManager.OpenSettings();

        this.gameObject.SetActive(false);
    }

    public void CloseOptions() 
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<PersistentGameManager>();
        }
        PlayTheSound();
        gameManager.CloseSettings();
        optionsIsUp = false;
    }

    public void ReturnToMainMenu()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<PersistentGameManager>();
        }
        PlayTheSound();

        StartGame startGame = FindFirstObjectByType<StartGame>();
        gameManager.BackToMenu(startGame.levelName, startGame.hasHub, startGame.hubName);

        Destroy(player.gameObject);
    }

    public void QuitGame()
    {
        PlayTheSound();
        Application.Quit();
    }

    public void PlayTheSound()
    {
        FMODSoundManager.instance.PlayOneShot(uiClick, transform.position, 0f, 0f);
    }
}
