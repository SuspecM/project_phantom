using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class LookAtTrigger : MonoBehaviour
{
    [Header("Look at settings")]
    public bool doNotInstaTrigger;
    [NaughtyAttributes.EnableIf("doNotInstaTrigger")]
    public float timeNeededForLookingToTrigger;
    private float _currentLookAtTime;
    private bool _isLooking;

    [Header("Everythin else")]
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    public bool playAnimation;
    [NaughtyAttributes.EnableIf("playAnimation")]
    public Animator[] animators;

    public bool playSound;
    [NaughtyAttributes.EnableIf("playSound")]
    public EventReference soundToPlay;
    [NaughtyAttributes.EnableIf("playSound")]
    public Transform soundSource;
    private EventInstance playingSound;

    public bool playExtraSound;
    [NaughtyAttributes.EnableIf("playExtraSound")]
    public EventReference extraSoundToPlay;

    public bool destroyAfterEvent;
    [NaughtyAttributes.EnableIf("destroyAfterEvent")]
    public float destroyTimer;
    [NaughtyAttributes.EnableIf("destroyAfterEvent")]
    public GameObject[] objectsToDestroyAfterTimer;

    public bool destroyTrigger;
    public void Trigger()
    {
        if (!doNotInstaTrigger)
        {
            TriggerForReal();
        }
        else
        {
            _isLooking = true;
        }
    }

    private void Update()
    {
        playingSound.set3DAttributes(RuntimeUtils.To3DAttributes(soundSource));
    }

    private void FixedUpdate()
    {
        if (_isLooking)
        {
            _currentLookAtTime += Time.deltaTime;
        }

        if (_currentLookAtTime >= timeNeededForLookingToTrigger)
        {
            TriggerForReal();
        }
    }

    private void TriggerForReal()
    {
        foreach (var item in objectsToActivate)
        {
            item.SetActive(true);
        }

        foreach (var item in objectsToDeactivate)
        {
            item.SetActive(false);
        }

        if (playAnimation)
        {
            foreach (var item in animators)
            {
                item.Play("Look at event");
            }
        }

        if (playSound)
        {
            playingSound = RuntimeManager.CreateInstance(soundToPlay);
            playingSound.set3DAttributes(RuntimeUtils.To3DAttributes(soundSource));
            playingSound.start();
        }

        if (playExtraSound)
        {
            FMODSoundManager.instance.PlayOneShot(extraSoundToPlay, Vector3.zero, 0f, 0f);
        }

        if (destroyAfterEvent)
        {
            foreach (var item in objectsToDestroyAfterTimer)
            {
                Destroy(item, destroyTimer);
            }
        }

        if (destroyTrigger)
        {
            Destroy(this);
        }

        //Debug.Log("I have been triggered");
    }
}
