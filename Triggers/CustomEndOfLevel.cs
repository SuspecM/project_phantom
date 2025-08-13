using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomEndOfLevel : MonoBehaviour
{
    public EventReference elevatorStarting;

    public GameObject button;

    public NewCharacterController player;
    public CameraLook playerCam;

    private PersistentGameManager gameManager;

    public string nextLevelName;
    public string currentLevelName;

    public bool hasHub;
    [NaughtyAttributes.EnableIf("hasHub")]
    public string hubName;

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager = FindObjectOfType<PersistentGameManager>();
        gameObject.SetActive(false);

        player = FindAnyObjectByType<NewCharacterController>();
        playerCam = player.GetComponentInChildren<CameraLook>();

        player.GetComponent<FeelEffectsManager>().PlayEvent(FeelEffectToPlay.BigElevatorStartEvent);

        FMODSoundManager.instance.PlayOneShot(elevatorStarting, player.transform.position, 0f, 0f);

        Invoke("Fade", 2f);
    }

    private void Fade()
    {
        playerCam.transform.GetComponent<CameraFade>().Fade(true);

        Invoke("LoadNextLevel", 4f);
    }

    void LoadNextLevel()
    {
        if (hasHub)
        {
            SceneManager.UnloadSceneAsync(hubName);
        }
        gameManager.LoadLevel(nextLevelName, currentLevelName, nextLevelName);
    }
}
