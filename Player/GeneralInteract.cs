using System;
using UnityEngine;

public class GeneralInteract : MonoBehaviour
{
    public InputManager inputManager;
    public Transform playerCameraTransform;

    public LayerMask interactLayerMask;

    private void Start()
    {
        inputManager.inputMaster.CameraLook.Interact.performed += _ => Interact();
    }

    public void Interact()
    {
        try
        {
            Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit ray, interactLayerMask);
            //Debug.Log(ray.transform.name);
            ray.transform.GetComponent<IInteractable>().Interact(this.GetComponent<PlayerPickupDrop>(), playerCameraTransform);

        }
        catch (Exception)
        {
            Debug.LogWarning("Interact was pressed but no interactable object was found!");
        }
    }
}
