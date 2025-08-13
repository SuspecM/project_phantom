using FMODUnity;
using System;
using UnityEngine;

public class InteractionCounter : MonoBehaviour
{
    public int counter;
    public InteractionCounter connectedCounter;
    public Button[] connectedObjects;
    public bool activateConnectedObjects;
    public int numberToReach;

    public EventReference soundToPlayOnCounterIncrease;
    public EventReference soundToPlayOnCompletion;
    public Transform counterIncreaseSource;
    public Transform completionSource;

    public GameObject[] objectsToDisableOnCompletion;
    public GameObject[] objectsToEnableOnCompletion;

    private void Start()
    {
        completionSource = FindAnyObjectByType<NewCharacterController>().transform;
    }

    private void Update()
    {
        CheckForCompletion();
    }

    public void CheckForCompletion()
    {
        counter = 0;
        foreach (var item in connectedObjects)
        {
            if (item.active) counter++;
        }

        if (counter >= numberToReach)
        {
            FMODSoundManager.instance.PlayOneShot(soundToPlayOnCompletion, completionSource.position, 0f, 0f);

            foreach (var item in objectsToDisableOnCompletion)
            {
                item.SetActive(false);
            }

            foreach (var item in objectsToEnableOnCompletion)
            {
                item.SetActive(true);
            }

            Destroy(this);
        }
    }
}
