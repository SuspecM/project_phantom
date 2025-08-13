using FMODUnity;
using UnityEngine;

public class BullshitPuzzle : MonoBehaviour, IInteractable
{
    public bool isTurnedOn;
    public GameObject offIndicator;
    public BullshitPuzzle[] neighbourSwitches;
    public EventReference clickSound;
    private float interactionDelay;
    private bool isPressed;

    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        interactionDelay = .15f;
        isPressed = true;
    }

    private void Press()
    {
        interactionDelay = 0;
        isTurnedOn = !isTurnedOn;
        offIndicator.SetActive(!isTurnedOn);
        FMODSoundManager.instance.PlayOneShot(clickSound, transform.position, 0f, 0f);

        foreach (var item in neighbourSwitches)
        {
            item.isTurnedOn = !item.isTurnedOn;
            item.offIndicator.SetActive(!item.isTurnedOn);
        }
    }

    private void Update()
    {
        interactionDelay -= Time.deltaTime;
        if (interactionDelay <= 0 && isPressed)
        {
            isPressed = false;
            Press();
        }
    }

    private void OnEnable()
    {
        if (isTurnedOn)
        {
            offIndicator.SetActive(false);
        }
    }

    void IInteractable.StopInteract()
    {
        
    }
}
