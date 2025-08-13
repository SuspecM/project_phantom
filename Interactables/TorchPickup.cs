using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchPickup : MonoBehaviour, IInteractable
{
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        NewCharacterController playerController = player.GetComponent<NewCharacterController>();

        playerController.canTorch = true;
        playerController.Torch();

        this.gameObject.SetActive(false);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
