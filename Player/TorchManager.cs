using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TorchManager : MonoBehaviour
{
    public float currentBattery;
    public float maxBattery;
    public Image torchIndicator;
    public Image innerTorchIndicator;

    public bool torchIsOn;

    public NewCharacterController characterController;

    private void Update()
    {
        if (torchIsOn)
        {
            currentBattery -= Time.deltaTime * 1;
        }
        else
        {
            currentBattery += Time.deltaTime * 5;
        }

        torchIndicator.fillAmount = currentBattery / 100;

        currentBattery = Mathf.Clamp(currentBattery, 0, 100);
        if (currentBattery == 0)
        {
            characterController.Torch();
            currentBattery = 1f;
        }

        TorchColor();
    }

    private void TorchColor()
    {
        // no idea why theese need to be color32 but they just do
        if (currentBattery > 59)
        {
            torchIndicator.color = new Color32(255, 255, 255, 120);
            innerTorchIndicator.color = new Color32(255, 255, 255, 120);
        }
        else if (currentBattery > 29)
        {
            torchIndicator.color = new Color32(235, 117, 14, 120);
            innerTorchIndicator.color = new Color32(235, 117, 14, 120);
        }
        else
        {
            torchIndicator.color = new Color32(219, 4, 4, 120);
            innerTorchIndicator.color = new Color32(219, 4, 4, 120);
        }
    }


}
