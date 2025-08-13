using UnityEngine;

public class WallMove : MonoBehaviour, IEvent
{
    public bool moveing;
    public Vector3 targetPoint;

    public void Trigger()
    {
        moveing = true;
    }

    private void FixedUpdate()
    {
        if (moveing)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime);
            if (transform.position == targetPoint)
            {
                moveing = false;
            }
        }
    }
}
