using UnityEngine;

public class ObjectMoveScript : MonoBehaviour
{
    public Vector3 moveDirection;
    public float moveScale;

    void Update()
    {
        transform.position += moveDirection * Time.deltaTime * moveScale;
    }
}
