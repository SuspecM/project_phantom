using FMODUnity;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    public int ammoToGive;
    public EventReference pickupSound;
    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        player.GetComponent<Gun>().reserveAmmo += ammoToGive;
        FMODSoundManager.instance.PlayOneShot(pickupSound, transform.position, 0f, 0f);
        Destroy(gameObject);
    }

    void IInteractable.StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
