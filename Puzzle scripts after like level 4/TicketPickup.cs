using UnityEngine;

public class TicketPickup : MonoBehaviour, IInteractable
{
    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        player.GetComponent<NewCharacterController>().tickets += 1;
        Destroy(this.gameObject);
    }

    void IInteractable.StopInteract()
    {
        
    }
}
