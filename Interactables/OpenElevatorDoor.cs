using FMODUnity;
using UnityEngine;

public class OpenElevatorDoor : MonoBehaviour, IInteractable
{
    public GameObject leftElevatorDoor;
    public GameObject rightElevatorDoor;
    public EventReference elevatorSound;
    public Transform soundSource;

    public bool startClosed;

    private void Start()
    {
        if (startClosed)
        {
            leftElevatorDoor.GetComponent<Animator>().Play("Close left door");
            rightElevatorDoor.GetComponent<Animator>().Play("Close right door");
        }
        else
        {
            leftElevatorDoor.GetComponent<Animator>().Play("Open left door");
            rightElevatorDoor.GetComponent<Animator>().Play("Open right door");
        }
    }
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        FMODSoundManager.instance.PlayOneShot(elevatorSound, soundSource.position, 0f, 0f);
        if (!startClosed)
        {
            leftElevatorDoor.GetComponent<Animator>().Play("Close left door");
            rightElevatorDoor.GetComponent<Animator>().Play("Close right door");
        }
        else
        {
            leftElevatorDoor.GetComponent<Animator>().Play("Open left door");
            rightElevatorDoor.GetComponent<Animator>().Play("Open right door");
        }
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
