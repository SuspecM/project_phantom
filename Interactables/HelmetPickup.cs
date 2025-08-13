using FMODUnity;
using UnityEngine;

public class HelmetPickup : MonoBehaviour, IInteractable
{
    public bool playAnimation;
    public Transform playerStandPosition;
    public Animator helmetAnimator;
    public EventReference pickupSound;

    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        if (!playAnimation)
        {
            FMODSoundManager.instance.PlayOneShot(pickupSound, transform.position, 0f, 0f);
            AddHelmet(player);
        }
        else
        {
            player.transform.position = playerStandPosition.position;
            player.transform.rotation = playerStandPosition.rotation;

            player.GetComponent<NewCharacterController>().DisablePlayerInput(true, true);
            player.GetComponentInChildren<CameraLook>().canLook = false;
            player.GetComponent<HealthManager>().cameraAnimator.enabled = true;
            player.GetComponent<HealthManager>().cameraAnimator.Play("helmetPickup");

            helmetAnimator.Play("pickup");

            player.GetComponent<NewCharacterController>().leftHandDefaultAnimator.SetBool("walk", false);
            player.GetComponent<NewCharacterController>().leftHandDefaultAnimator.SetBool("run", false);
            player.GetComponent<NewCharacterController>().leftHandDefaultAnimator.SetTrigger("helmet");
            player.GetComponent<NewCharacterController>().rightHandDefaultAnimator.SetBool("walk", false);
            player.GetComponent<NewCharacterController>().rightHandDefaultAnimator.SetBool("run", false);
            player.GetComponent<NewCharacterController>().rightHandDefaultAnimator.SetTrigger("helmet");
        }
    }

    public void AddHelmet(PlayerPickupDrop player)
    {
        NewCharacterController playerController = player.GetComponent<NewCharacterController>();

        playerController.TurnOnHelmet(true);

        playerController.GetComponent<HealthManager>().ActivateHUD();
        playerController.GetComponent<HealthManager>().AddHP(99);

        this.gameObject.SetActive(false);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
