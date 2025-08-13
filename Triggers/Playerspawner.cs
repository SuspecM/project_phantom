using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playerspawner : MonoBehaviour, IInteractable
{
    public string[] connectedScenes;
    [Tooltip("Which scene is currently loaded")]
    public int currentSceneIndex;
    public string connectorScene;

    public bool openTheOtherDoor;

    public EventReference doorClose;
    public EventReference doorOpen;

    public BigDoorAnimationController[] doors;
    public int openDoorIndex;
    public int closeDoorIndex;

    public GameObject[] objectsToActivateOnPress;

    private bool _pressable = true;
    private bool _waitForSceneToLoad;

    public PersistentGameManager gameManager;

    private int canSrynge;

    private PlayerPickupDrop _player;

    private void Start()
    {
        gameManager = FindAnyObjectByType<PersistentGameManager>();
    }

    void IInteractable.Interact(PlayerPickupDrop player, UnityEngine.Transform grapPoint)
    {
        if (_pressable)
        {
            _player = player;
            _pressable = false;
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(connectorScene));

            FMODSoundManager.instance.PlayOneShot(doorClose, doors[openDoorIndex].transform.position, 0f, 0f);

            doors[openDoorIndex].CloseDoor();

            if (objectsToActivateOnPress.Length > 0)
            {
                foreach (var item in objectsToActivateOnPress)
                {
                    item.SetActive(true);
                }
            }

            gameManager.placePlayerAtStart = false;
            gameManager.elevator = false;
            gameManager.cameFromHub = true;
            gameManager.startFromGround = false;

            gameManager.CopyPlayerVariables();

            Invoke("TransitionLevel", 2.5f);
        }

    }

    public void TransitionLevel()
    {
        if (openDoorIndex == 0)
        {
            openDoorIndex = 1;
            closeDoorIndex = 0;
            currentSceneIndex = 1;
            gameManager.LoadLevel(connectedScenes[currentSceneIndex], connectedScenes[0], connectorScene);

            int hasThrowable = 0;
            if (gameManager.hasThrowable) hasThrowable = 1;

            gameManager.saveManager.SaveNonDefaultProfile("level hub transition", connectedScenes[currentSceneIndex], 0, gameManager.mainMenuBg, gameManager.loadedAmmo, gameManager.reserveAmmo, gameManager.hasSrynge, gameManager.srynges, hasThrowable, gameManager.throwableId, gameManager.hasRedKeycard, gameManager.hasBlueKeycard, gameManager.GetComponent<TutorialManager>().physicsTutorialMessageShown, gameManager.GetComponent<TutorialManager>().handObjectTutorialMessageShown);
        }
        else
        {
            openDoorIndex = 0;
            closeDoorIndex = 1;
            currentSceneIndex = 0;
            gameManager.LoadLevel(connectedScenes[currentSceneIndex], connectedScenes[1], connectorScene);

            int hasThrowable = 0;
            if (gameManager.hasThrowable) hasThrowable = 1;

            gameManager.saveManager.SaveNonDefaultProfile("level hub transition", connectedScenes[currentSceneIndex], 0, gameManager.mainMenuBg, gameManager.loadedAmmo, gameManager.reserveAmmo, gameManager.hasSrynge, gameManager.srynges, hasThrowable, gameManager.throwableId, gameManager.hasRedKeycard, gameManager.hasBlueKeycard, gameManager.GetComponent<TutorialManager>().physicsTutorialMessageShown, gameManager.GetComponent<TutorialManager>().handObjectTutorialMessageShown);
        }

        _waitForSceneToLoad = true;
    }

    private void Update()
    {
        if (gameManager != null)
        {
            if (gameManager.loadingDone)
            {
                gameManager.loadingDone = false;
                if (openTheOtherDoor)
                    OpenOtherDoor();
            }
        }
        
    }

    public void OpenOtherDoor()
    {
        FMODSoundManager.instance.PlayOneShot(doorClose, doors[openDoorIndex].transform.position, 0f, 0f);
        try
        {
            SceneManager.MoveGameObjectToScene(_player.gameObject, SceneManager.GetSceneByName(connectedScenes[currentSceneIndex]));
        }catch (Exception e)
        {
            Debug.LogError(e);
        }

        doors[openDoorIndex].OpenDoor();

        Invoke("ResetTrigger", 3f);
    }

    private void ResetTrigger()
    {
        _pressable = true;
    }

    void IInteractable.StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
