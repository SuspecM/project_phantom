using UnityEngine;

public class InputManager : MonoBehaviour
{

    public InputMaster inputMaster;
    public MainInput mainInput;
    void Awake()
    {
        inputMaster = new InputMaster();
        mainInput = new MainInput();
    }

    private void OnEnable()
    {
        inputMaster.Enable();
        mainInput.Enable();
    }

    private void OnDisable()
    {
        inputMaster.Disable();
        mainInput.Disable();
    }
}
