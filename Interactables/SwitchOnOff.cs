using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

public class SwitchOnOff : MonoBehaviour, IInteractable
{
    public Animator animator;
    public bool onState;

    public EventReference switchSound;

    public GameObject[] objectsToSwitch;
    public TurretHealth[] turretsToSwitch;

    private void Start()
    {
        if (onState)
        {
            animator.Play("switch turn on");
            
        }
        else
        {
            animator.Play("switch turn off");
        }

        TurnObjects();
    }

    public void TurnObjects()
    {
        foreach (var obj in objectsToSwitch)
        {
            obj.SetActive(onState);
        }

        foreach (var item in turretsToSwitch)
        {
            if (item.health > 0)
            {
                if(onState)
                {
                    item.TurnOn();
                }
                else
                {
                    item.TurnOff(true);
                }
            }
        }
    }
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        onState = !onState;

        if (onState)
        {
            animator.Play("switch turn on");

        }
        else
        {
            animator.Play("switch turn off");
        }
        FMODSoundManager.instance.PlayOneShot(switchSound, transform.position, 10f, 25f);
        TurnObjects();
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
