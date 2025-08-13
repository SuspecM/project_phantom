using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoopEmitter : MonoBehaviour
{
    public EventReference[] possibleSounds;
    [Range(0, 300)]
    public float minTime;
    [Range(0, 300)]
    public float maxTime;
    private float currentTime;
    public bool oneShot;

    private void Start()
    {
        RollATime();
    }

    private void Update()
    {
        if (currentTime <= 0)
        {
            int random = UnityEngine.Random.Range(0, possibleSounds.Length);

            FMODSoundManager.instance.PlayOneShot(possibleSounds[random], transform.position, 0f, 0f);

            //Debug.Log($"Played {possibleSounds[random]}");

            if (oneShot )
            {
                Destroy(gameObject);
            }
            else
            {
                RollATime();
            }
        }
        currentTime -= Time.deltaTime;
    }

    public void RollATime()
    {
        currentTime = UnityEngine.Random.Range(minTime, maxTime);
        //Debug.Log($"Rolled {currentTime} seconds");
    }
}
