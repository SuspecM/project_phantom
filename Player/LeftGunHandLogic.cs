
using UnityEngine;

public class LeftGunHandLogic : MonoBehaviour
{
    public Animator gunSlideAnimator;
    public Gun gun;

    public void SlideGunSlider()
    {
        gunSlideAnimator.Play("gunSlideMove");
    }

    public void AddAmmoToGun()
    {
        gun.AddAmmo();
    }

    public void FinishReload()
    {
        gun.FinishReload();
    }
}
