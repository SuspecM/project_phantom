using FMODUnity;
using UnityEngine;

public class SryngePickup : MonoBehaviour, IInteractable
{
    public EventReference pickupSound;
    public GameObject tutorialToEnable;
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        player.GetComponent<NewCharacterController>().canSrynge = true;
        player.GetComponent<Gun>().sryngeUIElement.SetActive(true);
        FMODSoundManager.instance.PlayOneShot(pickupSound, grapPoint.position, 0f, 0f);
        tutorialToEnable.SetActive(true);
        Destroy(this.gameObject);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
