using UnityEngine;

public class CloseOffBankDoorTrigger : MonoBehaviour
{
    public BankDoor door;

    private void OnTriggerEnter(Collider other)
    {
        door.OpenClose();
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}
