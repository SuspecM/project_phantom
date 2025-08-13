using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class LoopingSoundEmitter : MonoBehaviour
{
    public EventReference sound;
    private EventInstance playingSound;

    private void Start()
    {
        playingSound = RuntimeManager.CreateInstance(sound);
        playingSound.start();
        playingSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
    }

    private void OnDisable()
    {
        playingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnDestroy()
    {
        playingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
