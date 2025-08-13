using FMODUnity;
using UnityEngine;

public class TvInput : MonoBehaviour, IInteractable
{
    public GameObject[] textToDisappear;
    public GameObject[] textToAppear;
    public TvOperator tv;
    public float delay;
    public GameObject eventToTrigger;
    public GameObject[] enemiesToSpawn;
    public BankDoor door;

    public TextMesh endText;

    public EventReference interactSound;

    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        FMODSoundManager.instance.PlayOneShot(interactSound, transform.position, 0f, 0f);
        if (tv.currentPhase < 5)
            tv.NextPhase();

        //Invoke("DelayedOff", delay);
    }

    public void DelayedOff()
    {
        foreach (var item in textToAppear)
        {
            item.SetActive(false);
        }

        tv.Turn(false);
        eventToTrigger.GetComponent<IEvent>().Trigger();

        foreach (var item in enemiesToSpawn)
        {
            item.SetActive(true);
        }

        door.OpenClose();
    }

    void IInteractable.StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
