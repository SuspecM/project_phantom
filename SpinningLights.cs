using UnityEngine;

public class SpinningLights : MonoBehaviour
{

    public float spinningAmountPerFrame = 3f;
    public Vector3 spinningAxis = Vector3.up;
    void Update()
    {
        transform.Rotate(spinningAxis.normalized * spinningAmountPerFrame, Space.Self);
    }
}
