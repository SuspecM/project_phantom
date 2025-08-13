using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeconEventEndTrigger : MonoBehaviour, IInteractable
{
    public string levelToPlacePlayerOn;
    public BigDoorAnimationController doorToOpen;
    public EventReference pressButton;
    public EventReference openDoor;
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    private PersistentGameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<PersistentGameManager>();
    }
    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        int canSrynge = 0;
        if (player.GetComponent<NewCharacterController>().canSrynge)
            canSrynge = 1;

        int hasThrowable = 0;
        if (player.GetComponent<NewCharacterController>().hasObjectInHand) hasThrowable = 1;

        gameManager.saveManager.SaveNonDefaultProfile("Auto save", levelToPlacePlayerOn, 0, 0, (int)player.GetComponent<Gun>().loadedAmmo, (int)player.GetComponent<Gun>().reserveAmmo, canSrynge, player.GetComponent<Gun>().srynges, hasThrowable, player.GetComponent<NewCharacterController>().indexOfActiveObject, player.GetComponent<KeycardHolder>().heldKeycards[0] ? 1 : 0, player.GetComponent<KeycardHolder>().heldKeycards[1] ? 1 : 0, player.GetComponent<TutorialManager>().physicsTutorialMessageShown, player.GetComponent<TutorialManager>().handObjectTutorialMessageShown);
        SceneManager.MoveGameObjectToScene(FindFirstObjectByType<NewCharacterController>().gameObject, SceneManager.GetSceneByName(levelToPlacePlayerOn));
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelToPlacePlayerOn));
        doorToOpen.OpenDoor();
        FMODSoundManager.instance.PlayOneShot(pressButton, transform.position, 0f, 0f);
        FMODSoundManager.instance.PlayOneShot(openDoor, doorToOpen.transform.position, 0f, 0f);

        foreach (var item in objectsToActivate)
        {
            item.SetActive(true);
        }

        foreach (var item in objectsToDeactivate)
        {
            item.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    void IInteractable.StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
