using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLight : MonoBehaviour
{
    public Light eventLight;
    public float timeToMax;
    public float rangeToReach;
    public float timeToGone;
    public float minRange;
    
    public void PlayEventLight()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        //Debug.Log("Flash triggered");

        float startRange = eventLight.range;
        float elapsedTime = 0f;

        while (elapsedTime < timeToMax)
        {
            eventLight.range = Mathf.Lerp(startRange, rangeToReach, elapsedTime / timeToMax);
            elapsedTime += Time.deltaTime;

            //Debug.Log($"Current range: {eventLight.range}");

            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < timeToGone) 
        {
            eventLight.range = Mathf.Lerp(rangeToReach, minRange, elapsedTime / timeToGone);
            elapsedTime += Time.deltaTime;

            //Debug.Log($"Current range: {eventLight.range}");

            yield return null;
        }

    }
}
