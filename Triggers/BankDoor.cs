using FMODUnity;
using UnityEngine;

public class BankDoor : MonoBehaviour
{
    public Animator leftDoor;
    public Animator rightDoor;
    public bool isOpen;
    public Transform soundSource;

    public EventReference doorSound;

    private void Start()
    {
        DoTheDoors();
    }

    [NaughtyAttributes.Button]
    public void OpenClose()
    {
        isOpen = !isOpen;
        DoTheDoors();
    }

    public void DoTheDoors()
    {
        if (isOpen)
        {
            leftDoor.Play("left bank open");
            rightDoor.Play("right bank open");
        }
        else
        {
            leftDoor.Play("left bank close");
            rightDoor.Play("right bank close");
        }

        FMODSoundManager.instance.PlayOneShot(doorSound, soundSource.position, 10f, 10f);
    }
}
