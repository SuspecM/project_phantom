using UnityEngine;

public class BullshitPuzzleChecker : MonoBehaviour
{
    public BullshitPuzzle[] connectedSwitches;
    public GameObject thingyToActivateOnSuccess;
    public int switches;
    public bool setBoolOnPlayerSpawner;

    private void Update()
    {
        switches = connectedSwitches.Length;
        foreach (var item in connectedSwitches)
        {
            if (item.isTurnedOn) switches--;
        }

        if (switches <= 0)
        {
            foreach (var item in connectedSwitches)
            {
                item.GetComponent<Collider>().enabled = false;
            }
            FindAnyObjectByType<BullshitPuzzleTag>().GetComponent<BigDoorAnimationController>().OpenDoor();
            thingyToActivateOnSuccess.SetActive(true);

            if (setBoolOnPlayerSpawner)
            {
                FindAnyObjectByType<Playerspawner>().openTheOtherDoor = true;
            }

            Destroy(this);
        }
    }
}
