using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlaySoundLoop : MonoBehaviour
{
    public float loopTime;
    public EventReference soundToPlay;

    private void Start()
    {
        PlaySound();
    }

    public void PlaySound()
    {
        FMODSoundManager.instance.PlayOneShot(soundToPlay, transform.position, 0f, 0f);
        Invoke("PlaySound", loopTime);
    }
}
