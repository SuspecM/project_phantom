using FMODUnity;
using UnityEngine;

public class ObjectIntoHand : MonoBehaviour, IInteractable
{
    public GameObject objectToGiveInHand;
    public int indexOfObject;
    private bool alreadyClicked = false;
    public EventReference pickupSound;

    private TutorialManager tut;

    [TextArea(2,5)]
    public string[] tuetMessages = { $"When an object is in your hand, you can interact with buttons and hand scanners", $"If you do not look at buttons or hand scanners, press to throw throw the item\n<sprite name=Mouse_LEFT>", $"To gently place the item\n<sprite name=Mouse_RIGHT>" };
    private int _currentMessage;
    void IInteractable.Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        if (!alreadyClicked)
        {
            if (player.GetComponent<NewCharacterController>().gunIsOn)
            {
                if (player.GetComponent<Gun>().currentGun == GunOut.Pistol)
                    player.GetComponent<NewCharacterController>().Gun();
                else if (player.GetComponent<Gun>().currentGun == GunOut.Srynge)
                    player.GetComponent<NewCharacterController>().Srynge();
            }
            alreadyClicked = true;
            player.GetComponent<NewCharacterController>().AddGrabObjectToHand(objectToGiveInHand, indexOfObject);
            FMODSoundManager.instance.PlayOneShot(pickupSound, player.transform.position, 5, 1);

            tut = player.GetComponent<TutorialManager>();
            if (tut.handObjectTutorialMessageShown == 0)
            {
                EnqueTutorialMessages();
                tut.handObjectTutorialMessageShown++;
            }
            else
            {
                this.gameObject.SetActive(false);
            }

            //Destroy(this.gameObject);
        }

    }

    private void EnqueTutorialMessages()
    {
        if (_currentMessage < tuetMessages.Length)
        {
            tut.QueueMessage(tuetMessages[_currentMessage], 5f);
            _currentMessage++;
            Invoke("EnqueTutorialMessages", .1f);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        

    }

    void IInteractable.StopInteract()
    {
        
    }
}
