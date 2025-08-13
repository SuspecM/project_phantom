using System.ComponentModel;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Polybrush;

public class PickupObject : MonoBehaviour, IInteractable
{
    public Rigidbody rb;
    private bool isGrabbed;
    private Transform grabPoint;

    public Vector3 grabOffset;

    public float forceToBeThrown = 1000f;

    [SerializeField]
    [NaughtyAttributes.ReadOnly]
    private bool _isGrounded;

    private Collider _playerCollider;
    public Collider selfCollider;

    public Transform parentObject;

    private CameraLook _playerCam;

    [TextArea(2,5)]
    public string tutMessage = $"While holding an item, press to toss it forward\n<sprite name=Mouse_RIGHT>";

    private void Awake()
    {
        
    }

    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        if (TryGetComponent<ImpactSound>(out ImpactSound sound))
        {
            sound.PickupGrace();
            sound.Pickup(true);
        }

        this.grabPoint = player.grabPoint;
        isGrabbed = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearDamping = 5f;

        _playerCam = player.GetComponentInChildren<CameraLook>();

        player.isGrabbing = true;
        player.grabbedObject = this;
        
        _playerCollider = player.GetComponent<Collider>();
        Physics.IgnoreCollision(selfCollider, _playerCollider);

        TutorialManager tut = player.GetComponent<TutorialManager>();
        if (tut.physicsTutorialMessageShown == 0)
        {
            tut.QueueMessage(tutMessage, 5f);
            tut.physicsTutorialMessageShown++;
        }
    }

    public void StopInteract()
    {
        isGrabbed = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.linearDamping = 0f;

        if (TryGetComponent<ImpactSound>(out ImpactSound sound))
        {
            sound.Pickup(false);
        }

        _playerCollider.GetComponent<PlayerPickupDrop>().ResetGrabPoint();

        Physics.IgnoreCollision(selfCollider, _playerCollider, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") || (collision.transform.CompareTag("Grab Object") && collision.transform.GetComponent<PickupObject>().ObjectIsGrounded()))
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
    }

    private void Update()
    {
        //if (isGrabbed)
        //{
        //    
        //}

        if (isGrabbed)
        {
            Vector3 direction = (grabPoint.position - transform.position + grabOffset).normalized;
            float distance = Vector3.Distance(transform.position, grabPoint.position);
            Vector3 velocity = direction * distance * 5000f * Time.deltaTime;

            //rb.MovePosition(Vector3.Lerp(transform.position, grabPoint.position + grabOffset, Time.deltaTime * 25f));

            rb.linearVelocity = velocity;
        }
    }

    public bool ObjectIsGrounded()
    {
        return _isGrounded;
    }
}
