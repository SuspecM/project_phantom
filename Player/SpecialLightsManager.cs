using UnityEngine;

public class SpecialLightsManager : MonoBehaviour
{
    public Light waterLight;
    public bool startOn;
    public bool isOn;

    private void Start()
    {
        if (!startOn)
        {
            waterLight.range = 0f;
        }
        else
        {
            waterLight.range = 10f;
        }
    }

    private void FixedUpdate()
    {
        if (isOn)
        {
            waterLight.range = Mathf.MoveTowards(waterLight.range, 10, .1f);
        }
        else
        {
            waterLight.range = Mathf.MoveTowards(waterLight.range, 0, -.1f);
        }
    }
}
