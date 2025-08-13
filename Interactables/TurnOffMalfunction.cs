using UnityEngine;

public class TurnOffMalfunction : MonoBehaviour
{
    public LeverOnOff switchToFix;
    public GameObject offLight;
    public GameObject onLight;

    private void Awake()
    {
        switchToFix.malfunctioned = true;
        offLight.SetActive(true);
        onLight.SetActive(false);
        gameObject.SetActive(false);
    }

    // used as "on activated" or something like that
    private void OnEnable()
    {
        switchToFix.malfunctioned = false;
        offLight.SetActive(false);
        onLight.SetActive(true);
    }
}
