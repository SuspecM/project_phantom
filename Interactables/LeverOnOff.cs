using FMODUnity;
using UnityEngine;

public class LeverOnOff : MonoBehaviour, IInteractable
{
    public Animator animator;
    public bool onState;
    public bool malfunctioned;
    public GameObject malfunctionEffect;
    public EventReference sparkSound;

    public EventReference switchOnSound;
    public EventReference switchOffSound;

    public BankDoor[] doorToSwitch;

    private void Start()
    {
        if (onState)
        {
            animator.Play("lever turn on");

        }
        else
        {
            animator.Play("lever turn off");
        }

        //TurnObjects();
    }

    public void TurnObjects()
    {
        foreach (var obj in doorToSwitch)
        {
            obj.OpenClose();
        }
    }
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        onState = !onState;

        if (onState)
        {
            animator.Play("lever turn on");
            FMODSoundManager.instance.PlayOneShot(switchOnSound, transform.position, 10f, 25f);

        }
        else
        {
            animator.Play("lever turn off");
            FMODSoundManager.instance.PlayOneShot(switchOffSound, transform.position, 10f, 25f);
        }
        
        if (!malfunctioned)
            TurnObjects();
        else
        {
            Invoke("SparkBack", .25f);
        }
    }

    private void SparkBack()
    {
        animator.Play("lever turn off");
        onState = false;
        Instantiate(malfunctionEffect, transform.position, Quaternion.identity);
        FMODSoundManager.instance.PlayOneShot(sparkSound, transform.position, 10f, 5f);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}

