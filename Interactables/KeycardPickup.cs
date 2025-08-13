using FMODUnity;
using UnityEngine;

public class KeycardPickup : MonoBehaviour, IInteractable
{
    public KeycardColor keycardColor;
    public EventReference pickup;
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        player.GetComponent<KeycardHolder>().heldKeycards[(int)keycardColor] = true;
        player.GetComponent<KeycardHolder>().EnableKeycard((int)keycardColor);
        FMODSoundManager.instance.PlayOneShot(pickup, grapPoint.position, 0f, 0f);
        Destroy(this.gameObject);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
