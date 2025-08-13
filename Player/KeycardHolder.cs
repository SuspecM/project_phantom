using UnityEngine;

public class KeycardHolder : MonoBehaviour
{
    public bool[] heldKeycards;

    public GameObject keycardHolderElement;
    public GameObject[] keycards;

    private void Awake()
    {
        foreach (var keycard in keycards)
        {
            keycard.SetActive(false);
        }
        keycardHolderElement.SetActive(false);
    }

    public void EnableKeycard(int keycardId)
    {
        heldKeycards[keycardId] = true;
        keycardHolderElement.SetActive(true);
        keycards[keycardId].SetActive(true);
    }
}
