using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeconEventTrigger : MonoBehaviour, IInteractable
{
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeActivate;

    public EventReference buttonPress;

    public string levelToUnload;
    public string thisLevel;
    public string nextLevel;
    private GameObject _player;

    public Vector3 positionToMoveThis;

    public DoorOpen doorToCloseOnInteract;

    public float firstDelay;

    public EventReference alarm;
    private EventInstance _playingAlarm;

    public GameObject[] delayedActivation;
    public GameObject[] delayedDeActivation;

    public float secondDelay;

    public GameObject[] delayed2Activation;
    public GameObject[] delayed2DeActivation;

    public float thirdDelay;

    public EventReference eventSound;

    public GameObject[] delayed3Activation;
    public GameObject[] delayed3DeActivation;

    public Animator leftDoor;
    public Animator rightDoor;

    public float fourthDelay;
    public GameObject[] delayed4Activation;
    public GameObject[] delayed4DeActivation;

    public EventReference smokeEventSound;

    public float fifthDelay;
    public GameObject[] delayed5Activation;
    public GameObject[] delayed5DeActivation;

    private PersistentGameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<PersistentGameManager>();
    }

    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        FMODSoundManager.instance.PlayOneShot(buttonPress, transform.position, 0f, 0f);

        transform.position = positionToMoveThis;

        Debug.Log("Decon event trigger has been moved back");

        doorToCloseOnInteract.ForceOpen();

        Debug.Log("Door has been force closed");

        foreach (var item in objectsToActivate)
        {
            item.SetActive(true);
            Debug.Log($"{item} has been activated");
        }

        foreach (var item in objectsToDeActivate)
        {
            item.SetActive(false);
            Debug.Log($"{item} has been deactivated");
        }

        _player = FindAnyObjectByType<NewCharacterController>().gameObject;

        Debug.Log("Started first delay");
        Invoke("Delay1", firstDelay);
    }

    void Delay1()
    {
        Debug.Log("First delay finished");

        SceneManager.MoveGameObjectToScene(_player, SceneManager.GetSceneByName(thisLevel));
        SceneManager.UnloadSceneAsync(levelToUnload);

        _playingAlarm = RuntimeManager.CreateInstance(alarm);
        _playingAlarm.start();
        _playingAlarm.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

        foreach (var item in delayedActivation)
        {
            item.SetActive(true);
            Debug.Log($"{item} has been activated");
        }

        foreach (var item in delayedDeActivation)
        {
            item.SetActive(false);
            Debug.Log($"{item} has been deactivated");
        }

        Invoke("Delay2", secondDelay);
    }

    void Delay2()
    {
        foreach (var item in delayed2Activation)
        {
            item.SetActive(true);
            Debug.Log($"{item} has been activated");
        }

        foreach (var item in delayed2DeActivation)
        {
            item.SetActive(false);
            Debug.Log($"{item} has been deactivated");
        }

        Invoke("Delay3", thirdDelay);
    }

    void Delay3()
    {
        _player.GetComponent<FeelEffectsManager>().PlayEvent(FeelEffectToPlay.DeconEvent);
        FMODSoundManager.instance.PlayOneShot(eventSound, transform.position, 0f, 0f);

        leftDoor.Play("left door being broken");
        rightDoor.Play("right door being broken");

        foreach (var item in delayed3Activation)
        {
            item.SetActive(true);
            Debug.Log($"{item} has been activated");
        }

        foreach (var item in delayed3DeActivation)
        {
            item.SetActive(false);
            Debug.Log($"{item} has been deactivated");
        }

        Invoke("Delay4", fourthDelay);
    }

    void Delay4()
    {
        foreach (var item in delayed4Activation)
        {
            item.SetActive(true);
            Debug.Log($"{item} has been activated");
        }

        foreach (var item in delayed4DeActivation)
        {
            item.SetActive(false);
            Debug.Log($"{item} has been deactivated");
        }

        FMODSoundManager.instance.PlayOneShot(smokeEventSound, transform.position, 0f, 0f);
        FindFirstObjectByType<HealthManager>().AddHP(99);

        _playingAlarm.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        Invoke("Delay5", fifthDelay);
    }

    void Delay5()
    {
        foreach (var item in delayed5Activation)
        {
            item.SetActive(true);
            Debug.Log($"{item} has been activated");
        }

        foreach (var item in delayed5DeActivation)
        {
            item.SetActive(false);
            Debug.Log($"{item} has been deactivated");
        }

        gameManager.placePlayerAtStart = false;
        gameManager.elevator = false;
        gameManager.cameFromHub = true;
        gameManager.startFromGround = false;

        gameManager.CopyPlayerVariables();

        SceneManager.LoadSceneAsync(nextLevel, LoadSceneMode.Additive);
        FindFirstObjectByType<StartGame>().gameObject.SetActive(false);
    }

    void IInteractable.StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
