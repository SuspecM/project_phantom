using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractShake : MonoBehaviour, IInteractable
{
    public FeelEffectToPlay shakeEvent;
    public float delay;
    private FeelEffectsManager feelManager;
    public bool playSound;
    public EventReference soundToPlay;
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        feelManager = player.GetComponent<FeelEffectsManager>();
        Invoke("HarlemShakeReference", delay);
    }

    public void HarlemShakeReference()
    {
        if (playSound)
        {
            FMODSoundManager.instance.PlayOneShot(soundToPlay, transform.position, 0f, 0f);
        }
        feelManager.PlayEvent(shakeEvent);
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
