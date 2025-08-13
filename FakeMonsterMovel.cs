using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FakeMonsterMovel : MonoBehaviour
{
    public EventReference startingRoar;
    public EventReference moveSound;
    public EventInstance _playingMoveSound;

    public Vector3 positionToMoveTo;

    private void Start()
    {
        FMODSoundManager.instance.PlayOneShot(startingRoar, transform.position, 0f, 0f);

        _playingMoveSound = RuntimeManager.CreateInstance(moveSound);
        _playingMoveSound.start();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, positionToMoveTo, .025f);
        _playingMoveSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        if (transform.position == positionToMoveTo)
        {
            _playingMoveSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            gameObject.SetActive(false);
        }
    }
}
