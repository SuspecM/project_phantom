using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraLook : MonoBehaviour
{
    public InputManager inputManager;
    [SerializeField, Range(1, 200)] public float lookSpeedX = 80f;
    [SerializeField, Range(1, 200)] public float lookSpeedy = 40f;

    [SerializeField, Range(1, 180)] public float upperLookLimit = 80f;
    [SerializeField, Range(1, 180)] public float lowerLookLimit = 80f;

    public bool canLook;

    public Transform body;

    private float xRotation = 0f;

    private Vector3 _cameraVelocity;
    private Quaternion _lastCameraRotation;

    private float _velocityGrace;

    public NewCharacterController controller;
    public PlayerPickupDrop pickupDrop;

    public LayerMask layersToRaycastForLookAtTrigger;

    public bool shot;

    public Image crosshair;
    public Image usehair;

    [Header("Sway")]
    public bool sway;

    public float smooth = 10f;
    public float smoothRot = 12f;

    public float swayStep = .01f;
    public float maxSwayStepDistance = .06f;
    private Vector3 _swayPos;
    private Vector2 _lookInput;

    public float swayRotationStep = 4f;
    public float maxSwayRotationStep = 5f;
    private Vector3 _swayEulerRot;

    public bool disableCrosshair;

    private PersistentGameManager gameManager;

    public PlayerKnockUpManager knockUpManager;
    public EventReference knockDown;
    public EventReference standUp;

    private void Start()
    {
        inputManager.mainInput.CameraLook.Flashlight.performed += _ => ToggleTorch();
        _lastCameraRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (canLook)
        {
            if (gameManager == null)
            {
                gameManager = FindAnyObjectByType<PersistentGameManager>();
            }

            lookSpeedX = gameManager.verticalSensitivity;
            lookSpeedy = gameManager.horizontalSensitivity;

            float mouseX = inputManager.inputMaster.CameraLook.MouseX.ReadValue<float>() * lookSpeedX * Time.deltaTime;
            float mouseY = inputManager.inputMaster.CameraLook.MouseY.ReadValue<float>() * lookSpeedy * Time.deltaTime;

            _lookInput = new Vector2(mouseX, mouseY);

            Vector3 calculatedVelocity = (transform.rotation.eulerAngles - _lastCameraRotation.eulerAngles) / Time.deltaTime;

            if (calculatedVelocity != Vector3.zero)
            {
                _cameraVelocity = calculatedVelocity;
                _lastCameraRotation = transform.rotation;
                _velocityGrace = .25f;
            }
            else
            {
                if (_velocityGrace > 0)
                {
                    _velocityGrace -= Time.deltaTime;
                }
                else
                {
                    _cameraVelocity = Vector3.zero;
                }
            }

            //mouseY += shot ? 10 : 0;
            if (shot) shot = false;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -lowerLookLimit, upperLookLimit);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            body.Rotate(Vector3.up * mouseX);
        }
        else
        {
            _cameraVelocity = Vector3.zero;
        }

        if (sway)
            Sway();

        //Debug.Log(transform.rotation);
        
        //Debug.Log($"{GetVelocity()}");
    }

    private void Sway() 
    {
        if (!sway) _swayPos = Vector3.zero;
        else
        {
            Vector3 invertLook = _lookInput * -swayStep;
            invertLook.x = Mathf.Clamp(invertLook.x, -maxSwayStepDistance, maxSwayStepDistance);
            invertLook.y = Mathf.Clamp(invertLook.y, -maxSwayStepDistance, maxSwayStepDistance);

            _swayPos = invertLook;
        }

        if (!sway) _swayEulerRot = Vector3.zero;
        else
        {
            Vector2 invertLookRot = _lookInput * -swayRotationStep;
            invertLookRot.x = Mathf.Clamp(invertLookRot.x, -maxSwayRotationStep, maxSwayRotationStep);
            invertLookRot.y = Mathf.Clamp(invertLookRot.y, -maxSwayRotationStep, maxSwayRotationStep);

            _swayEulerRot = new Vector3(invertLookRot.y, invertLookRot.x, invertLookRot.x);
        }

        if (controller.gunIsOn)
        {
            controller.gunHandTarget.transform.localPosition = Vector3.Lerp(
                controller.gunHandTarget.transform.localPosition,
                _swayPos,
                Time.deltaTime * smooth
                );

            controller.gunHandTarget.transform.localRotation = Quaternion.Slerp(
                controller.gunHandTarget.transform.localRotation,
                Quaternion.Euler(_swayEulerRot),
                Time.deltaTime * smooth
                );
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log($"{hit.transform.gameObject.name}");
            if (hit.transform.GetComponent<LookAtTrigger>() != null)
                hit.transform.GetComponent<LookAtTrigger>().Trigger();

            if (!controller.ads && !disableCrosshair)
            {
                if ((hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable) || 
                    hit.collider.GetComponentInChildren<IInteractable>() != null)
                    && Vector3.Distance(hit.transform.position, transform.position) <= 4.5f)
                {
                    crosshair.gameObject.SetActive(false);
                    usehair.gameObject.SetActive(true);
                }
                else
                {
                    crosshair.gameObject.SetActive(true);
                    usehair.gameObject.SetActive(false);
                }
            }
            else
            {
                crosshair.gameObject.SetActive(false);
                usehair.gameObject.SetActive(false);
            }
            
        }
    }

    public void ToggleTorch()
    {
        controller.Torch();
    }

    public Vector3 GetVelocity()
    {
        return transform.TransformDirection(_cameraVelocity);
    }

    public void PlayKnockedDownSound()
    {
        FMODSoundManager.instance.PlayOneShot(knockDown, transform.position, 0f, 0f);
    }

    public void PlayGetUpSound()
    {
        FMODSoundManager.instance.PlayOneShot(standUp, transform.position, 0f, 0f);
    }

    public void StopKnockUp()
    {
        knockUpManager.ReenablePlayer();
    }
}
