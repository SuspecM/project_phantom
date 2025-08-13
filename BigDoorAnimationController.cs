using UnityEngine;

public class BigDoorAnimationController : MonoBehaviour
{
    public bool startOpen;
    public Animator animator;

    private void Start()
    {
        if (startOpen)
        {
            animator.Play("Open");
        }
        else
        {
            animator.Play("Close");
        }
    }

    public void OpenDoor()
    {
        animator.Play("Open");
    }

    public void CloseDoor()
    {
        animator.Play("Close");
    }
}
