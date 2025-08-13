using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact(PlayerPickupDrop player, Transform grapPoint);

    public void StopInteract();
}
