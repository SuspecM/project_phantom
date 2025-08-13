using FMODUnity;
using System;
using UnityEngine;

public class HealingItem : MonoBehaviour, IInteractable
{
    public float healing;
    public EventReference soundToPlay;
    public HealthManager healthManager;

    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        healthManager.AddHP(healing);
        FMODSoundManager.instance.PlayOneShot(soundToPlay, transform.position, 0f, 0f);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        try
        {
            if (healthManager == null)
            {
                healthManager = FindAnyObjectByType<HealthManager>();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("No player found!");
        }
        
    }

    void IInteractable.StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
