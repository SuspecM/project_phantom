using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;
    public float delay;
    public bool destroyAfterActivation;
    public bool deactivateAfterActivation;

    public bool active;

    public bool playSoundInitially;
    [NaughtyAttributes.EnableIf("playSoundInitially")]
    public EventReference initialPlaySound;

    public bool playSoundAfterDelay;
    [NaughtyAttributes.EnableIf("playSoundAfterDelay")]
    public EventReference afterDelaySound;

    public bool rotateItems;
    [NaughtyAttributes.EnableIf("rotateItems")]
    public GameObject[] itemsToRotate;
    [NaughtyAttributes.EnableIf("rotateItems")]
    public Vector3 addedRotation;

    private bool _interactionDone;

    private bool _isDoingShit;

    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        if (!_isDoingShit && !_interactionDone)
        {
            if (playSoundInitially)
            {
                FMODSoundManager.instance.PlayOneShot(initialPlaySound, transform.position, 7f, 15f);
            }

            _interactionDone = true;
            _isDoingShit = true;
            Invoke("ActivateShit", delay);
        }
        
        if (destroyAfterActivation)
            GetComponent<Collider>().enabled = false;
    }

    public void ActivateShit()
    {
        if (playSoundAfterDelay)
        {
            FMODSoundManager.instance.PlayOneShot(afterDelaySound, transform.position, 7f, 15f);
        }

        foreach (var obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

        foreach (var item in objectsToDeactivate)
        {
            item.SetActive(false);
        }

        if (rotateItems)
        {
            foreach (var item in itemsToRotate)
            {
                item.transform.eulerAngles += addedRotation;
            }
        }

        _isDoingShit = false;

        active = true;

        if (destroyAfterActivation)
            Destroy(this.gameObject);
        if (deactivateAfterActivation)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
