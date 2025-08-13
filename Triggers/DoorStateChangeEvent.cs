using FMODUnity;
using UnityEngine;

public class DoorStateChangeEvent : MonoBehaviour
{
    public TriggerType triggerType;
    public DoorOpen[] panelsToChangeState;
    public DoorState doorState;
    public GameObject[] objectsToDeactivate;
    public GameObject[] objectsToActivate;

    public bool playSound;
    [NaughtyAttributes.EnableIf("playSound")]
    public EventReference[] soundsToPlay;
    [NaughtyAttributes.EnableIf("playSound")]
    public Transform soundSource;

    public PlayerPickupDrop player;

    public bool destroyOnTrigger;

    private void OnTriggerEnter(Collider collision)
    {
        player = FindAnyObjectByType<PlayerPickupDrop>();

        foreach (var item in panelsToChangeState)
        {
            if (item.GetState() != doorState)
                item.Interact(player, transform);
        }

        foreach (var item in objectsToDeactivate)
        {
            item.SetActive(false);
        }

        foreach (var item in objectsToActivate)
        {
            item.SetActive(true);
        }

        if (playSound)
            foreach (var item in soundsToPlay)
            {
                FMODSoundManager.instance.PlayOneShot(item, soundSource.position, 0f, 0f);
            }

        if (destroyOnTrigger)
            Destroy(this);
    }
}
