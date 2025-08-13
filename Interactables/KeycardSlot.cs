using UnityEngine;

public enum KeycardColor
{
    Red = 0,
    Green = 1,
    Blue = 2
}

public class KeycardSlot : MonoBehaviour, IInteractable
{
    public KeycardColor keycardNeeded;
    public BankDoor doorToOpen;

    public GameObject[] shitToActivate;
    public GameObject[] shitToDeactivate;
    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        if (player.GetComponent<KeycardHolder>().heldKeycards[(int)keycardNeeded])
        {
            doorToOpen.OpenClose();

            foreach (var item in shitToActivate)
            {
                item.SetActive(true);
            }

            foreach (var item in shitToDeactivate)
            {
                item.SetActive(false);
            }

            Destroy(this);
        }
        else
        {
            player.GetComponent<KeycardTextManager>().ShowText();
            Debug.Log("Wrong keycard or no keycard present");
        }
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
