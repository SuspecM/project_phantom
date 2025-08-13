using UnityEngine;

public class EnableAggro : MonoBehaviour
{
    public GameObject enemy;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with: {other.name}");
        if (other.CompareTag("Player"))
            if (enemy.TryGetComponent<BrainSpider>(out BrainSpider spider))
                spider.buildAnger = true;

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Exited collision with: {other.name}");
        if (other.CompareTag("Player"))
            if (enemy.TryGetComponent<BrainSpider>(out BrainSpider spider))
                spider.buildAnger = false;
    }
}
