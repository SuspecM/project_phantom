using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevelTrigger : MonoBehaviour
{
    public TriggerType triggerType;

    public bool skipElevator;

    public Animator elevatorDoorLeft;
    public Animator elevatorDoorRight;

    public EventReference elevatorDoorClose;
    public Transform elevatorSoundSource;

    public ParticleSystem[] showerParticles;
    public EventReference showerSound;
    public Transform showerSoundSource;

    public Transform playerLookAtTransform;
    public GameObject arms;

    public NewCharacterController player;
    public CameraLook playerCam;

    private PersistentGameManager gameManager;

    public string nextLevelName;
    public string currentLevelName;

    public bool hasHub;
    [EnableIf("hasHub")]
    public string hubName;

    private void OnTriggerEnter(Collider other)
    {
        player = FindAnyObjectByType<NewCharacterController>();
        playerCam = FindAnyObjectByType<CameraLook>();
        arms = player.arms;

        gameManager.CopyPlayerVariables();

        gameManager.currentCheckpoint = 0;

        player.DisablePlayerInput(true);
        playerCam.canLook = false;

        if (!skipElevator)
        {
            if (player.torchIsOn)
                player.Torch();

            Invoke("StartLook", .25f);
        }
        else
        {
            Fade();
        }
        
        
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<PersistentGameManager>();
    }

    private void StartLook()
    {
        StartCoroutine(LookAt());
    }

    private IEnumerator LookAt()
    {
        arms.SetActive(false);

        Vector3 direction = playerLookAtTransform.position - player.transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        targetRotation = Quaternion.Euler(
            0f,
            targetRotation.eulerAngles.y,
            targetRotation.eulerAngles.z
            );

        string iShouldvePaidAttentionOnCalculus = "I'm close to ending it if I have to work with Quaternions again. I didn't sign up for this shit (I 100% signed up for this shit and I'm doing this as a past time activity)";

        Debug.Log(iShouldvePaidAttentionOnCalculus);

        float time = 0f;

        while (time < 1.75f)
        {
            //player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, time / .75f);
            playerCam.transform.rotation = Quaternion.RotateTowards(playerCam.transform.rotation, targetRotation, 2f);

            time += Time.fixedDeltaTime;

            yield return null;
        }

        Invoke("CloseDoors", 1f);
    }

    private void CloseDoors()
    {
        elevatorDoorLeft.Play("Close left door");
        elevatorDoorRight.Play("Close right door");

        FMODSoundManager.instance.PlayOneShot(elevatorDoorClose, elevatorSoundSource.position, 0f, 0f);

        player.GetComponent<MusicPlayer>().StopMusic(true);

        Invoke("Shower", 2.25f);
    }

    private void Shower()
    {
        foreach (var item in showerParticles)
        {
            item.Play();
        }

        FMODSoundManager.instance.PlayOneShot(showerSound, showerSoundSource.position, 0f, 0f);

        Invoke("Heal", 1.25f);
    }

    private void Heal()
    {
        player.GetComponent<HealthManager>().AddHP(100);

        Invoke("Fade", 1.25f);
    }

    private void Fade()
    {
        playerCam.transform.GetComponent<CameraFade>().Fade(true);

        Invoke("LoadNextLevel", 4f);
    }

    void LoadNextLevel()
    {
        if (hasHub)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(hubName));

        gameManager.LoadLevel(nextLevelName, currentLevelName, nextLevelName);
    }
}
