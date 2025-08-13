using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCrouchTrigger : MonoBehaviour
{
    [field: SerializeField]
    public TriggerType Type { get; set; }

    private void OnTriggerEnter(Collider collision)
    {
        collision.transform.GetComponent<NewCharacterController>().canCrouch = false;
        Debug.Log("Player entered no crouch trigger");
    }

    private void OnTriggerExit(Collider collision)
    {
        collision.transform.GetComponent<NewCharacterController>().canCrouch = true;
        Debug.Log("Player exited no crouch trigger");
    }

    private void OnTriggerStay(Collider collision)
    {
        
    }

}
