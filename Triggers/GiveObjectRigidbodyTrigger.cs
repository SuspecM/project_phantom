using UnityEngine;

public class GiveObjectRigidbodyTrigger : MonoBehaviour
{
    public GameObject[] objectsToGive;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var obj in objectsToGive) 
        { 
            obj.AddComponent<Rigidbody>();
        }

        Destroy(gameObject);
    }
}
