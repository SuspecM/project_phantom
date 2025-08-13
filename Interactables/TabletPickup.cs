using UnityEngine;

public class TabletPickup : MonoBehaviour, IInteractable
{
    public string title;
    [TextArea]
    public string description;

    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        player.GetComponent<TabletLogic>().OpenTablet(title, description);
    }

    void IInteractable.StopInteract()
    {
        
    }
}
