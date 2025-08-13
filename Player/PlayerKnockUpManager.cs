using NaughtyAttributes;
using UnityEngine;

public class PlayerKnockUpManager : MonoBehaviour
{
    public CameraLook cam;
    public Animator cameraAnimator;
    public NewCharacterController player;

    [Button]
    public void GetKnockedUp()
    {
        cam.canLook = false;

        if (player.isCrouching)
        {
            player.InstantStand();
        }

        if (player.torchIsOn)
        {
            cam.ToggleTorch();
        }

        if (player.gunIsOn)
        {
            if (player.ads)
            {
                player.PreventAds();
            }
            player.Gun();
        }

        player.DisablePlayerInput(true);

        cameraAnimator.enabled = true;
        cameraAnimator.Play("get knocked up");
    }

    public void ReenablePlayer()
    {
        cameraAnimator.enabled = false;
        cam.canLook = true;
        player.DisablePlayerInput(false);
        player.PermitAds();
    }
}
