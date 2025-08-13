using UnityEngine;

public class DelayedEventTrigger : MonoBehaviour
{
    public float delayTime;

    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    public bool destroyThis;

    private void OnEnable()
    {
        Invoke("DelayedEvent", delayTime);
    }

    void DelayedEvent()
    {
        foreach (GameObject obj in objectsToActivate) 
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }

        if (destroyThis)
        {
            Destroy(gameObject);
        }
    }
}
