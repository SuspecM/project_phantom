using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using FMODUnity;
using UnityEngine.InputSystem;

public class PersistentGameManager : MonoBehaviour
{
    public bool loadMainMenu;

    public int mainMenuBg;

    public static PersistentGameManager gameManager;
    public GameObject loadingScreen;
    public Image loadingBar;
    public SaveManager saveManager;

    public float verticalSensitivity = 35f;
    public float horizontalSensitivity = 25f;

    public string profileName;
    public int loadedProfile;

    public List<string> profileData;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public PlayerInput playerInput;
    public InputDevice lastUsedDevice; // Tracks the last active input device

    public int targetFps = 120;

    public GameObject[] throwableObjects;

    #region in-game stuff
    public string currentLevel;
    public int currentCheckpoint;
    public bool hasProfile;

    public int loadedAmmo;
    public int reserveAmmo;

    public int hasSrynge;
    public int srynges;

    public bool hasThrowable;
    public int throwableId;

    public int hasRedKeycard;
    public int hasBlueKeycard;

    public int physicsTutorialMessageShown;
    public int handObjectTutorialMessageShown;
    #endregion

    #region Start game variables
    [Header("Player spawn conditions")]
    public bool elevator;
    public bool placePlayerAtStart;
    public bool cameFromHub;
    public bool startFromGround;

    public bool playerCopied;

    [Header("Player spawn variables")]
    public bool canGun;
    public bool canTorch;
    public bool hasHelmet;

    [Header("Music player")]
    public EventReference music;
    public bool playMusic;
    public bool increaseTension;
    public string tensionVarName;
    public float startingValue;
    public float currentValue;
    public float tensionIncreasePerFrame;
    public bool alreadyPlayig;
    #endregion

    #region settings
    public GameObject settingMenu;

    public int qualityLevel;
    public bool fullScreen;
    #endregion

    public bool loadingDone;

    private void Awake()
    {
        saveManager.LoadSettings("Initial settings load");
        saveManager.LoadProfile("Initial check if profile exists");

        saveManager.SaveSettings("Creating initial settings save file");

        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += OnControlsChanged;

        settingMenu.SetActive(false);

        if (loadMainMenu) LoadLevel("MainMenu", "", "MainMenu");

        Application.targetFrameRate = targetFps;
    }

    public bool IsControllerInput()
    {
        return lastUsedDevice != null && lastUsedDevice is Gamepad;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        // Update the last used device when controls change
        lastUsedDevice = input.devices[0];
        Debug.Log($"Last used device: {lastUsedDevice.displayName}");
    }

    public void CopyPlayerVariables()
    {
        GameObject player = FindAnyObjectByType<NewCharacterController>().gameObject;

        canGun = player.GetComponent<NewCharacterController>().canGun;
        canTorch = player.GetComponent<NewCharacterController>().canTorch;
        hasHelmet = player.GetComponent<NewCharacterController>().helmet;
        reserveAmmo = player.GetComponent<Gun>().reserveAmmo;
        loadedAmmo = player.GetComponent<Gun>().loadedAmmo;
        hasSrynge = player.GetComponent<NewCharacterController>().canSrynge ? 1 : 0;
        srynges = player.GetComponent<Gun>().srynges;

        hasThrowable = player.GetComponent<NewCharacterController>().hasObjectInHand;
        throwableId = hasThrowable ? player.GetComponent<NewCharacterController>().indexOfActiveObject : 0;

        hasRedKeycard = player.GetComponent<KeycardHolder>().heldKeycards[0] ? 1 : 0;
        hasBlueKeycard = player.GetComponent<KeycardHolder>().heldKeycards[1] ? 1 : 0;

        physicsTutorialMessageShown = player.GetComponent<TutorialManager>().physicsTutorialMessageShown;
        handObjectTutorialMessageShown = player.GetComponent<TutorialManager>().handObjectTutorialMessageShown;


        music = player.GetComponent<MusicPlayer>().music;
        playMusic = player.GetComponent<MusicPlayer>().playMusic;
        increaseTension = player.GetComponent<MusicPlayer>().increaseTension;
        tensionVarName = player.GetComponent<MusicPlayer>().tensionVarName;
        startingValue = player.GetComponent<MusicPlayer>().startingValue;
        currentValue = player.GetComponent<MusicPlayer>().currentValue;
        tensionIncreasePerFrame = player.GetComponent<MusicPlayer>().tensionIncreasePerFrame;
        alreadyPlayig = player.GetComponent<MusicPlayer>().alreadyPlaying;

        playerCopied = true;
    }

    public void OpenSettings()
    {
        settingMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingMenu.SetActive(false);
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    public void LoadLevel(string levelName, string levelToUnload, string levelToSetAsActive)
    {
        //loadingScreen.SetActive(true);
        //Reroll();

        scenesLoading.Clear();

        try
        {
            SceneManager.UnloadSceneAsync(levelToUnload);

        }
        catch (Exception e)
        {
            Debug.LogWarning("Scene to unload is invalid!");
        }
        scenesLoading.Add(SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress(SceneManager.GetSceneByName(levelToSetAsActive)));
    }

    public void LoadLevel(string levelName, string levelToUnload, string levelToSetAsActive, GameObject player)
    {
        scenesLoading.Clear();

        try
        {
            SceneManager.UnloadSceneAsync(levelToUnload);

        }catch (Exception e)
        {
            Debug.LogWarning("Scene to unload is invalid!");
        }
        scenesLoading.Add(SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive));

        StartCoroutine(PlacePlayerOnNewLevel(SceneManager.GetSceneByName(levelToSetAsActive), player, levelName));
    }

    public IEnumerator PlacePlayerOnNewLevel(Scene sceneToSetAsActive, GameObject player, string levelToPlacePlayerOn)
    {
        loadingDone = false;

        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach (AsyncOperation item in scenesLoading)
                {
                    totalSceneProgress += item.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesLoading.Count);

                //loadingBar.fillAmount = totalSceneProgress;

                yield return null;
            }
        }

        SceneManager.SetActiveScene(sceneToSetAsActive);

        if (sceneToSetAsActive.name != "MainMenu")
        {
            FindFirstObjectByType<StartGame>().StartDelay();
        }

        SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(levelToPlacePlayerOn));

        LoadingDone();
    }

    public void RestartLevel(string levelName, bool hasHub, string hubName)
    {
        if (!hasHub)
        {
            SceneManager.UnloadScene(levelName);

            scenesLoading.Clear();

            scenesLoading.Add(SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadProgress(SceneManager.GetSceneByName(levelName)));
        }
        else
        {
            SceneManager.UnloadScene(levelName);
            SceneManager.UnloadScene(hubName);

            scenesLoading.Clear();

            scenesLoading.Add(SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadProgress(SceneManager.GetSceneByName(levelName)));
        }
    }

    public void BackToMenu(string levelName, bool hasHub, string hubName)
    {
        if (!hasHub)
        {
            SceneManager.UnloadScene(levelName);

            scenesLoading.Clear();

            scenesLoading.Add(SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadProgress(SceneManager.GetSceneByName(levelName)));
        }
        else
        {
            SceneManager.UnloadScene(levelName);
            SceneManager.UnloadScene(hubName);

            scenesLoading.Clear();

            scenesLoading.Add(SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadProgress(SceneManager.GetSceneByName(levelName)));
        }
    }

    float totalSceneProgress;
    public IEnumerator GetSceneLoadProgress(Scene sceneToSetAsActive)
    {
        loadingDone = false;

        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                yield return null;
            }
        }

        if (sceneToSetAsActive.IsValid() && sceneToSetAsActive.isLoaded)
        {
            SceneManager.SetActiveScene(sceneToSetAsActive);

            yield return null;
        }
        else
        {
            Debug.LogError("Scene failed to load or is invalid!");
        }

        if (sceneToSetAsActive.name != "MainMenu")
        {
            FindFirstObjectByType<StartGame>().StartDelay();
        }


        LoadingDone();
    }

    private void LoadingDone()
    {
        Resources.UnloadUnusedAssets();
        loadingDone = true;
    }

    public void Continue()
    {
        
    }

    public void Reroll()
    {
        //randomTipNumber = Random.Range(0, tips.Count);
        //tipText.text = tips[randomTipNumber];
    }
}
