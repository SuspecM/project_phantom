using FMODUnity;
using UnityEngine;

public class SryngeAmmoPickup : MonoBehaviour, IInteractable
{
    public EventReference pickupSound;
    public int ammo;
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        player.GetComponent<Gun>().srynges += ammo;
        FMODSoundManager.instance.PlayOneShot(pickupSound, grapPoint.position, 0f, 0f);
        Destroy(this.gameObject);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
