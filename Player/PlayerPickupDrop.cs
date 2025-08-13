using System.Linq;
using UnityEngine;

public class PlayerPickupDrop : MonoBehaviour
{
    public InputManager inputManager;
    public Transform playerCameraTransform;
    public Transform grabPoint;

    public NewCharacterController playerMovement;

    public float pickupDistance;
    public float minimumGrabDistance;
    public float maximumGrabDistance;

    public LayerMask pickupLayerMask;
    public LayerMask interactLayerMask;

    [HideInInspector]
    public bool isGrabbing;
    [HideInInspector]
    public PickupObject grabbedObject;

    [NaughtyAttributes.ReadOnly]
    public Vector3 startGrabPoint;
    private float currentDistance = 2f;

    private float _noInteractTimer;

    public float throwTimer;

    public float pushForce;

    private void Start()
    {
        inputManager.inputMaster.CameraLook.Interact.performed += _ => Interact();
        inputManager.inputMaster.CameraLook.Interact.started += _ => OpenTablet();
        inputManager.inputMaster.CameraLook.Interact.canceled += _ => StopInteract();
        inputManager.mainInput.Movement.ADS.performed += _ => PushItem();

        grabPoint.position = (transform.position + playerCameraTransform.forward * currentDistance) + (playerCameraTransform.up * 2f);
        startGrabPoint = new Vector3(0, -1, 3);
    }

    public void Interact()
    {
        if (!GetComponent<TabletLogic>().tabletIsOpen && _noInteractTimer == 0 && !playerMovement.ads)
        {
            _noInteractTimer = .25f;

            //Debug.Log("Interaction triggered");

            RaycastHit[] hits = Physics.RaycastAll(playerCameraTransform.position, playerCameraTransform.forward, 4f, interactLayerMask);
            hits = hits.OrderBy(h => h.distance).ToArray();

            RaycastHit[] throwableHits = Physics.RaycastAll(playerCameraTransform.position, playerCameraTransform.forward, 4f, pickupLayerMask);
            throwableHits = throwableHits.OrderBy(h => h.distance).ToArray();

            if (hits.Length > 0)
            {
                hits[0].transform.GetComponent<IInteractable>().Interact(this.GetComponent<PlayerPickupDrop>(), playerCameraTransform);
            }
            else if (throwableHits.Length > 0 && !playerMovement.hasObjectInHand)
            {
                throwableHits[0].transform.GetComponentInChildren<IInteractable>().Interact(this, grabPoint);
            }
            else
            {
                if (playerMovement.hasObjectInHand && 
                    !HasComponentInArray<PickupObject>(hits)
                    )
                {
                    if (throwTimer == 0)
                    {
                        GameObject thrownObject = Instantiate(playerMovement.objectInHand, playerCameraTransform.position + playerCameraTransform.forward * 1.5f, Quaternion.identity, null);
                        thrownObject.SetActive(true);
                        thrownObject.GetComponent<Rigidbody>().AddForce(playerCameraTransform.forward * 10, ForceMode.Impulse);
                        playerMovement.hasObjectInHand = false;
                        playerMovement.inHandObjects[playerMovement.indexOfActiveObject].SetActive(false);
                        throwTimer = .25f;
                        _noInteractTimer = .25f;
                        //playerMovement.ResetHandAfterThrowing();
                    }

                }
                else if (!playerMovement.ads && _noInteractTimer == 0f)
                {
                    if (isGrabbing)
                    {
                        grabbedObject.StopInteract();
                        isGrabbing = false;
                        grabbedObject.rb.linearVelocity = Vector3.zero;
                        grabbedObject.rb.angularVelocity = Vector3.zero;
                        grabbedObject = null;
                    }
                }
                else
                {
                    if (isGrabbing)
                    {
                        grabbedObject.StopInteract();
                        isGrabbing = false;
                        grabbedObject = null;
                    }
                }
            }
        }
    }

    public bool HasComponentInArray<T>(RaycastHit[] hits) where T : Component
    {
        if (hits.Length == 0) return false;
        Debug.Log($"Object {hits[0].transform.gameObject.name} has component {hits[0].transform.gameObject.TryGetComponent<T>(out _)}");
        return hits[0].transform.gameObject.TryGetComponent<T>(out _);
    }

    public void StopInteract()
    {
        if (isGrabbing)
        {
            grabbedObject.StopInteract();
            isGrabbing = false;
            grabbedObject = null;
        }
    }

    public void DropCurrentlyHeldItem()
    {
        if (!isGrabbing && playerMovement._throwObjectInHandPreventTimer == 0)
        {
            GameObject thrownObject = Instantiate(playerMovement.objectInHand, playerCameraTransform.position + playerCameraTransform.forward * 1, Quaternion.identity, null);
            thrownObject.SetActive(true);
            playerMovement.hasObjectInHand = false;
            playerMovement.inHandObjects[playerMovement.indexOfActiveObject].SetActive(false);
            throwTimer = 1f;
            _noInteractTimer = 1f;
        }
    }

    public void PushItem()
    {
        if (isGrabbing && !playerMovement.ads)
        {
            playerMovement._throwObjectInHandPreventTimer = .5f;
            playerMovement._performAdsPreventTimer = .5f;

            grabbedObject.StopInteract();
            isGrabbing = false;

            Vector3 direction = grabbedObject.transform.position - playerCameraTransform.position;
            direction.Normalize();

            grabbedObject.rb.AddForce(direction * grabbedObject.forceToBeThrown);

            grabbedObject = null;

            _noInteractTimer = .25f;
        }
    }

    public void OpenTablet()
    {
        if (GetComponent<TabletLogic>().tabletIsOpen)
        {
            GetComponent<TabletLogic>().CloseTablet();
            _noInteractTimer = .5f;
        }
        else
        {
            Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit ray, interactLayerMask);

            //Debug.Log($"The distance between the player and {ray.transform.name} was: {Vector3.Distance(playerCameraTransform.position, ray.transform.position)}");
            if (Vector3.Distance(playerCameraTransform.position, ray.transform.position) < 4f && ray.transform.CompareTag("Tablet") && _noInteractTimer <= 0)
            {
                ray.transform.GetComponent<IInteractable>().Interact(this.GetComponent<PlayerPickupDrop>(), playerCameraTransform);
                Debug.Log(ray.transform.name);
            }
        }

    }

    private void Update()
    {
        _noInteractTimer = Mathf.Clamp(_noInteractTimer - Time.deltaTime, 0f, 3f);

        throwTimer = Mathf.Clamp(throwTimer - Time.deltaTime, 0f, 1f);

        if ((playerMovement.GetRunning() || playerMovement.ads) && isGrabbing)
        {
            grabbedObject.StopInteract();
            isGrabbing = false;
            grabbedObject = null;
        }

        if (isGrabbing)
        {
            //currentDistance = Vector3.Distance(transform.position, grabPoint.position);

            /*if (inputManager.mainInput.CameraLook.ObjectZoom.ReadValue<float>() != 0)
                currentDistance += inputManager.mainInput.CameraLook.ObjectZoom.ReadValue<float>() * .85f;
            else
                currentDistance += inputManager.mainInput.CameraLook.ObjectZoomGamepad.ReadValue<float>() * .01f;
            // minimum distance so that we can't move it inside the player, maximum distance which more or less the same as the interaction distance
            currentDistance = Mathf.Clamp(currentDistance, 1f, 4f);

            Vector3 newPosition = (transform.position) + (playerCameraTransform.forward * currentDistance) + (playerCameraTransform.up * 2f);
            grabPoint.transform.position = newPosition;*/

            if (inputManager.inputMaster.CameraLook.Interact.ReadValue<float>() == 0f)
                Interact();
        }
        else
        {
            currentDistance = 3f;
        }

    }

    public void ResetGrabPoint()
    {
        //grabPoint.position = startGrabPoint;
    }
}
