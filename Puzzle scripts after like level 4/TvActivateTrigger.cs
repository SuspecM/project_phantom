using UnityEngine;

public class TvActivateTrigger : MonoBehaviour
{
    public TvOperator tv;
    public bool destroyAfter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            tv.Turn(true);
        }

        if (destroyAfter)
            Destroy(this.gameObject);
    }
}
