using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPikcup : MonoBehaviour, IInteractable
{
    public EventReference pickupSound;
    public int loadedAmmo = 2;
    public int reservAmmo = 0;

    [TextArea(2,5)]
    public string tutMessage = $"To equip your pistol\n<sprite name=Keyboard_1";

    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        NewCharacterController playerController = player.GetComponent<NewCharacterController>();

        FMODSoundManager.instance.PlayOneShot(pickupSound, transform.position, 0f, 0f);

        playerController.canGun = true;
        playerController.Gun();
        playerController.GetComponent<Gun>().loadedAmmo = loadedAmmo;
        playerController.GetComponent<Gun>().reserveAmmo += reservAmmo;
        playerController.GetComponent<Gun>().ammoCounter.SetActive(true);

        player.GetComponent<TutorialManager>().QueueMessage(tutMessage, 5f);

        this.gameObject.SetActive(false);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
