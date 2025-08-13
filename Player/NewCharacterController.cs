using Cinemachine;
using Cinemachine.PostFX;
using FMODUnity;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum SurfaceType
{
    generic = 0,
    metal = 1,
    wood = 2,
    grass = 3,
    silent = 4,
    dirt = 5,
    water = 6
}

public enum LeftHandPosition
{
    walk = 0,
    crouch = 1,
    run = 2,
    gun = 3,
    ads = 4,
    pickupObject = 5
}

public enum RightHandPosition
{
    walk = 0,
    crouch = 1,
    run = 2,
    stand = 3,
    torch = 4
}

public class NewCharacterController : MonoBehaviour
{
    [Header("Puzzle stuff")]
    public int tickets;

    [NaughtyAttributes.ReadOnly]
    public bool playerDead;

    public bool hasObjectInHand;
    public GameObject objectInHand;
    public int indexOfActiveObject;
    public GameObject[] inHandObjects;

    [NaughtyAttributes.ReadOnly]
    public float _throwObjectInHandPreventTimer;


    [Header("Basic references")]
    public InputManager inputManager;

    public Animator cameraAnimator;
    public GameObject arms;

    public bool canMove;

    public Transform cameraTransform;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineVolumeSettings _volumeSettings;
    private DepthOfField _depthOfField;
    public VolumeProfile defaultVolume;
    public VolumeProfile adsVolume;

    private bool _collisionIsWithGrabObject;

    [Header("Movement")]
    public float walkSpeed = 3;
    public float runSpeed = 9;
    public float crouchSpeed = 1.5f;

    private float _stepTimer;

    public CharacterController characterController;

    private Vector3 _moveDirection;
    private Vector2 _currentInput;

    [Header("Sprint")]
    public bool canRun;
    private bool IsRunning => canRun && inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1;

    [Header("Jump")]
    public bool canJump;
    public float jumpForce = 8f;
    public float gravity = 9.81f;
    private bool _wasGrounded = true;

    [Header("Crouch")]
    public bool canCrouch;
    public float crouchHeight = .5f;
    public float standHeight = 2f;
    public bool isCrouching;
    private bool _inCrouchAnimation;
    public float timeToCrouch = .25f;
    public Vector3 standingCenterPoint = Vector3.zero;
    public Vector3 crouchingCenterPoint = new Vector3(0, .5f, 0);

    [Header("Footstep sounds for different materials")]
    [NaughtyAttributes.ReadOnly] public SurfaceType currentSurface;
    public EventReference[] metalSteps;
    public EventReference[] woodSteps;
    public EventReference[] grassSteps;
    public EventReference[] crouchSteps;
    public EventReference[] silent;
    public EventReference[] dirtSteps;
    public EventReference[] waterSteps;
    public bool isInWater;
    public GameObject waterRipple;
    public float yCoordForRipple;

    [Header("Headbob")]
    public bool headBob = true;
    public float walkBobSpeed = 14f;
    public float walkBobAmmount = 0.05f;
    public float RunBobSpeed = 18f;
    public float RunBobAmmount = .1f;
    public float crouchBobSpeed = 8f;
    public float crouchBobAmmount = 0.025f;
    public float waterWalkBobSpeed = 18f;
    public float waterWalkBobAmmount = 0.05f;

    private float _defaultYPos = 2f;
    private float _bobTimer;

    [Header("Hand movement")]
    public bool moveHands;
    public float waterSpeed = .75f;

    public LeftHandPosition currentLeftHandPosition;
    public RightHandPosition currentRightHandPosition;
    [SerializeField][NaughtyAttributes.ReadOnly] private float _leftHandTrasitionTime;
    [SerializeField][NaughtyAttributes.ReadOnly] private float _rightHandTrasitionTime;
    public float timeToTransitionHand;

    public TwoBoneIKConstraint rightHandIK;
    public TwoBoneIKConstraint leftHandIK;

    public Transform targetLeftHand;
    public Transform targetRightHand;

    [SerializeField][NaughtyAttributes.ReadOnly] private Transform _currentLeftHandTransform;
    [SerializeField][NaughtyAttributes.ReadOnly] private Transform _currentRightHandTransform;

    public Transform defaultRightHandTarget;
    public Transform defaultLeftHandTarget;
    public Transform runRightHandTarget;
    public Transform runLeftHandTarget;
    public Transform crouchRightHandTarget;
    public Transform crouchLeftHandTarget;

    public Transform pickupObjectLeftHandTarget;

    public Animator leftHandDefaultAnimator;
    public Animator rightHandDefaultAnimator;
    public Animator leftHandRunAnimator;
    public Animator rightHandRunAnimator;

    public bool block;

    [Header("Torch")]
    public bool canTorch;
    public bool torchIsOn;
    public GameObject torch;
    public GameObject torchLight;
    public GameObject torchLightRay;

    public EventReference torchClick;

    public Transform torchHandTarget;

    public Animator torchHandAnimator;

    private float _torchOnTimer;

    public TorchManager torchManager;

    [Header("HUD")]
    public bool helmet;
    public GameObject helmetObject;

    public bool paused;
    public GameObject pauseMenu;
    public PauseMenu pauseMenuLogic;

    [Header("Gun")]
    public bool canGun;
    public bool canSrynge;
    public bool gunIsOn;
    public GameObject[] guns;
    private float _gunIsOnTimer;

    public bool canAds = true;
    public bool ads;
    private bool _adsPressed;

    public Light waterLight;

    [NaughtyAttributes.ReadOnly]
    public float _performAdsPreventTimer;

    public Gun gunLogic;

    public bool reloading;

    public Transform gunHandTarget;
    public Animator gunHandAnimator;

    public Animator objectHandAnimator;

    public Transform gunAdsTarget;
    public Animator gunAdsAnimator;

    private float _swappingGuns;
    private bool _isSwappingGuns;


    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        _currentLeftHandTransform = defaultLeftHandTarget;
        _currentRightHandTransform = defaultRightHandTarget;

        _volumeSettings = virtualCamera.GetComponent<CinemachineVolumeSettings>();
        _volumeSettings.m_Profile.TryGet<DepthOfField>(out _depthOfField);
        //Debug.Log(_depthOfField.ToString());
    }

    private void Start()
    {
        pauseMenuLogic.playerInput.actions.FindActionMap("UI").Disable();

        inputManager.inputMaster.Movement.Jump.started += _ => Jump();
        inputManager.inputMaster.Movement.Crouch.started += _ => Crouch();
        inputManager.inputMaster.Movement.Run.performed += _ => PreventAds();
        inputManager.inputMaster.Movement.Run.canceled += _ => PermitAds();
        inputManager.mainInput.Movement.DeployGun.performed += _ => Gun();
        inputManager.mainInput.Movement.DeploySrynge.performed += _ => Srynge();

        inputManager.mainInput.Movement.Pause.performed += _ => Pause();

        inputManager.mainInput.CameraLook.Reload.performed += _ => Reload();

        inputManager.mainInput.Movement.ADS.performed += _ => PerformAds();
        inputManager.mainInput.Movement.ADS.canceled += _ =>  CancelAds();

        inputManager.mainInput.Movement.DebugHideUI.performed += _ => HideUI();
        inputManager.mainInput.Movement.DebugEnableUI.performed += _ => EnableUI();

        torchLight.SetActive(torchIsOn);
        torch.SetActive(torchIsOn);
        torchLightRay.SetActive(torchIsOn);

        pauseMenu.SetActive(false);

        TurnOnHelmet(helmet);

        //we skip the first object as it's undefined for now
        for (int i = 1; i < inHandObjects.Length; i++)
        {
            inHandObjects[i].SetActive(false);
        }

        if (hasObjectInHand)
            inHandObjects[indexOfActiveObject].SetActive(true);
    }

    public void Pause()
    {
        if (paused)
        {
            pauseMenuLogic.playerInput.actions.FindActionMap("Movement").Disable();
            paused = false;
            pauseMenu.SetActive(false);

            Time.timeScale = 1f;
            DisablePlayerInput(false);
            cameraTransform.GetComponent<CameraLook>().canLook = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            paused = true;
            pauseMenu.SetActive(true);

            Time.timeScale = 0f;
            DisablePlayerInput(true);
            cameraTransform.GetComponent<CameraLook>().canLook = false;
            Cursor.lockState = CursorLockMode.None;

            pauseMenuLogic.BringUpPauseMenu();
        }
    }

    private void HideUI()
    {
        GetComponent<HealthManager>().DeactivateHUD();
        TurnOnHelmet(false);
    }

    private void EnableUI()
    {
        GetComponent<HealthManager>().ActivateHUD();
        TurnOnHelmet(true);
    }

    void Update()
    {
        if (canMove)
        {
            HandleMovementInput();

            ApplyFinalMovements();

            if (headBob)
                HandleHeadBob(_moveDirection);

            GroundCheck();

            PlayMoveSound();

            if (!_wasGrounded && characterController.isGrounded)
            {
                PlayStepSound(0);
            }

            //Debug.Log($"was grounded {_wasGrounded} is grounded {characterController.isGrounded}");

            _wasGrounded = characterController.isGrounded;

        }

        if (moveHands)
            HandleArmMovement();

        if (block)
        {
            CancelAds();
        }

        if (_performAdsPreventTimer > 0)
        {
            _performAdsPreventTimer = Mathf.Clamp(_performAdsPreventTimer - Time.deltaTime, 0, 10);
        }

        if (_throwObjectInHandPreventTimer > 0)
        {
            _throwObjectInHandPreventTimer = Mathf.Clamp(_throwObjectInHandPreventTimer - Time.deltaTime, 0, 10);
        }

        if (!playerDead)
        {
            if(ads && gunLogic.currentGun != GunOut.Srynge)
        {
                virtualCamera.m_Lens.FieldOfView = Mathf.MoveTowards(virtualCamera.m_Lens.FieldOfView, 60, Time.deltaTime * 60);

                _depthOfField.gaussianStart = new MinFloatParameter(40, 40, true);
                _depthOfField.gaussianEnd = new MinFloatParameter(75, 75, true);
            }
        else
            {
                virtualCamera.m_Lens.FieldOfView = Mathf.MoveTowards(virtualCamera.m_Lens.FieldOfView, 85, Time.deltaTime * 85);

                _depthOfField.gaussianStart = new MinFloatParameter(10, 10, true);
                _depthOfField.gaussianEnd = new MinFloatParameter(15, 15, true);
            }
        }

        if (_isSwappingGuns)
        {
            _swappingGuns = Mathf.Clamp(_swappingGuns - Time.deltaTime, 0f, 1f);
            if (_swappingGuns == 0)
            {
                _isSwappingGuns = false;
                DeactivateAllGunsExcept(gunLogic.currentGun);
                gunIsOn = !gunIsOn;
                StartCoroutine(OnOffGun(gunIsOn));
            }
        }
    }

    void HandleMovementInput()
    {
        _currentInput = new Vector2((isCrouching ? crouchSpeed : IsRunning ? runSpeed : walkSpeed) * inputManager.inputMaster.Movement.Forward.ReadValue<float>(), (isCrouching ? crouchSpeed : IsRunning ? runSpeed : walkSpeed) * inputManager.inputMaster.Movement.Right.ReadValue<float>());

        float moveDirectionY = _moveDirection.y;

        _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + (transform.TransformDirection(Vector3.right) * _currentInput.y);

        // preventing sideways movement from being faster
        _moveDirection = _moveDirection.normalized * Mathf.Clamp(_moveDirection.magnitude, 0, isCrouching ? crouchSpeed : IsRunning ? runSpeed : walkSpeed);
        _moveDirection.y = moveDirectionY;
    }

    void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(_moveDirection * Time.deltaTime);
    }

    public void Reload()
    {
        if (gunIsOn)
            gunLogic.Reload();
    }

    void Jump()
    {
        if (characterController.isGrounded && canJump)
        {
            _moveDirection.y = jumpForce;
        }
    }

    void Crouch()
    {
        if (characterController.isGrounded && canCrouch && !_inCrouchAnimation)
        {
            StartCoroutine(CrouchStand());
        }
    }

    public void AddGrabObjectToHand(GameObject grabbedObject, int indexOfObject)
    {
        hasObjectInHand = true;
        objectInHand = grabbedObject;
        GetComponent<PlayerPickupDrop>().throwTimer = 1f;
        inHandObjects[indexOfObject].SetActive(true);
        indexOfActiveObject = indexOfObject;

        //_currentLeftHandTransform = pickupObjectLeftHandTarget;
        //currentLeftHandPosition = LeftHandPosition.pickupObject;
    }

    public void ResetHandAfterThrowing()
    {
        _currentLeftHandTransform = defaultLeftHandTarget;
        currentLeftHandPosition = LeftHandPosition.walk;
        targetLeftHand = defaultLeftHandTarget;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        #region pushing over physics objects
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push
        body.linearVelocity = pushDir * (isCrouching ? crouchSpeed : IsRunning ? runSpeed : walkSpeed);
        #endregion

    }

    public void Bounce(float bounceForce)
    {
        _moveDirection.y =+ bounceForce;
    }


    private IEnumerator CrouchStand()
    {
        _inCrouchAnimation = true;

        float timeElapsed = 0f;
        float targetHeight = isCrouching ? standHeight : crouchHeight;
        float currentHeigt = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenterPoint : crouchingCenterPoint;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeigt, targetHeight, timeElapsed / timeToCrouch);
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, characterController.height, cameraTransform.localPosition.z);
            _defaultYPos = characterController.height;
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, characterController.height, cameraTransform.localPosition.z);
        _defaultYPos = characterController.height;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        _stepTimer = 0;

        _inCrouchAnimation = false;
    }

    public void InstantStand()
    {
        characterController.height = standHeight;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, characterController.height, cameraTransform.localPosition.z);
        _defaultYPos = characterController.height;
        characterController.center = standingCenterPoint;

        isCrouching = !isCrouching;
    }

    private void HandleHeadBob(Vector3 move)
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(move.x) > .1f || Mathf.Abs(move.z) > .1f)
        {
            _bobTimer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isInWater ? waterWalkBobSpeed : inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? walkBobSpeed : RunBobSpeed);
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                _defaultYPos + Mathf.Sin(_bobTimer) * (isCrouching ? crouchBobAmmount : isInWater ? waterWalkBobAmmount : inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? walkBobAmmount : RunBobAmmount),
                cameraTransform.localPosition.z
                );
        }
        else
        {
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, Mathf.MoveTowards(cameraTransform.localPosition.y, _defaultYPos, Time.deltaTime * 2), cameraTransform.localPosition.z);
        }
    }

    public void Gun()
    {
        if (!gunIsOn && hasObjectInHand)
        {
            GetComponent<PlayerPickupDrop>().DropCurrentlyHeldItem();
        }

        if (canGun && !_isSwappingGuns)
        {
            if (!gunIsOn)
                DeactivateAllGunsExcept(GunOut.Pistol);
            else
            {
                DeactivateAllGuns();

                if (gunLogic.currentGun != GunOut.Pistol)
                {
                    _isSwappingGuns = true;
                    _swappingGuns = .25f;
                    gunLogic.currentGun = GunOut.Pistol;
                }
            }
            gunIsOn = !gunIsOn;
            StartCoroutine(OnOffGun(gunIsOn));
        }
        
    }

    public void Srynge()
    {
        if (!gunIsOn && hasObjectInHand)
        {
            GetComponent<PlayerPickupDrop>().DropCurrentlyHeldItem();
        }

        if (canSrynge && !_isSwappingGuns)
        {
            if (!gunIsOn)
                DeactivateAllGunsExcept(GunOut.Srynge);
            else
            {
                DeactivateAllGuns();

                if (gunLogic.currentGun != GunOut.Srynge)
                {
                    _isSwappingGuns = true;
                    _swappingGuns = .25f;
                    gunLogic.currentGun = GunOut.Srynge;
                }
            }
            gunIsOn = !gunIsOn;
            StartCoroutine(OnOffGun(gunIsOn));
        }
        
    }

    private void DeactivateAllGuns()
    {
        foreach (GameObject gun in guns)
        {
            gun.SetActive(false);
        }
    }

    private void DeactivateAllGunsExcept(GunOut gunOut)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            if (i == (int)gunOut)
                guns[i].SetActive(true);
            else
                guns[i].SetActive(false);
        }
        gunLogic.currentGun = gunOut;
        gunAdsAnimator.SetInteger("gunOut", (int)gunLogic.currentGun);

    }

    public IEnumerator OnOffGun(bool gunOn)
    {
        if (canMove)
        {
            if (!gunIsOn)
            {
                //put away gun sound?
            }
            else
            {
                while (_gunIsOnTimer < .25f && gunIsOn)
                {
                    if (hasObjectInHand)
                    {
                        GetComponent<PlayerPickupDrop>().DropCurrentlyHeldItem();
                    }
                    _gunIsOnTimer++;
                    yield return null;
                }
            }
        }
    }

    public void PerformAds()
    {
        if (canAds && _performAdsPreventTimer == 0)
        {
            if (hasObjectInHand)
            {
                GetComponent<PlayerPickupDrop>().DropCurrentlyHeldItem();
            }

            if (gunIsOn && !IsRunning)
            {
                ads = true;
                _adsPressed = true;
            }
            else
            {
                ads = false;
                _adsPressed = true;
            }
        }
        
    }

    public void CancelAds()
    {
        if (gunIsOn)
        {
            ads = false;
            _adsPressed = false;
        }
    }

    public void PreventAds()
    {
        canAds = false;
        ads = false;
    }

    public void PermitAds()
    {
        canAds = true;
    }

    public void Torch()
    {
        if (canTorch)
        {
            torchIsOn = !torchIsOn;
            _torchOnTimer = 0;
            torch.SetActive(torchIsOn);
            StartCoroutine(OnOffTorch(torchIsOn));

            torchManager.torchIsOn = torchIsOn;
        }
    }


    public IEnumerator OnOffTorch(bool torchOn)
    {
        if (!torchOn)
        {
            torchLight.SetActive(false);
            torchLightRay.SetActive(false);
            FMODSoundManager.instance.PlayOneShot(torchClick, transform.position, 10f, 5f);
        }
        else
        {
            while (_torchOnTimer < .25f && torchIsOn)
            {
                _torchOnTimer += Time.deltaTime;
                yield return null;
            }

            if (torchIsOn && _torchOnTimer >= .25f)
            {
                torchLight.SetActive(true);
                torchLightRay.SetActive(true);
                FMODSoundManager.instance.PlayOneShot(torchClick, transform.position, 10f, 5f);
            }
            
        }
    }

    public void TurnOnHelmet(bool on)
    {
        helmet = on;
        helmetObject.SetActive(on);
    }

    public void HandleArmMovement()
    {
        #region right hand
        if (block)
        {
            rightHandDefaultAnimator.SetBool("block", true);
            torchHandAnimator.SetBool("block", true);
        }
        else if (torchIsOn)
        {
            rightHandDefaultAnimator.SetBool("block", false);
            torchHandAnimator.SetBool("block", false);

            if (currentRightHandPosition != RightHandPosition.torch)
            {
                currentRightHandPosition = RightHandPosition.torch;
                _currentRightHandTransform = torchHandTarget;

                _rightHandTrasitionTime = 0f;
            }

            if (!ads)
            {
                torchHandAnimator.SetBool("ads", false);
            }
            else
            {
                if (gunLogic.currentGun != GunOut.Srynge)
                    torchHandAnimator.SetBool("ads", true);
            }

            if (reloading)
            {
                torchHandAnimator.SetBool("ads", false);
            }
            else if ((Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f) && characterController.isGrounded && !isCrouching)
            {
                torchHandAnimator.speed = isInWater ? waterSpeed : 1f;

                if (inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1)
                {
                    torchHandAnimator.SetBool("run", true);
                    torchHandAnimator.SetBool("walk", false);
                }
                else
                {
                    torchHandAnimator.SetBool("run", false);
                    torchHandAnimator.SetBool("walk", true);
                }
            }
            else
            {
                torchHandAnimator.SetBool("run", false);
                torchHandAnimator.SetBool("walk", false);
            }
        }
        else
        {

            if (currentRightHandPosition != RightHandPosition.walk)
            {
                currentRightHandPosition = RightHandPosition.walk;
                _rightHandTrasitionTime = 0f;

                _currentRightHandTransform = defaultRightHandTarget;
            }

            if (reloading)
            {
                rightHandDefaultAnimator.SetBool("reload", true);
            }
            else if (isCrouching)
            {
                rightHandDefaultAnimator.SetBool("walk", false);
                rightHandDefaultAnimator.SetBool("run", false);
            }
            else if (inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1)
            {
                rightHandDefaultAnimator.SetBool("walk", false);
                rightHandDefaultAnimator.SetBool("run", true);
            }
            else if ((Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f) && characterController.isGrounded)
            {
                rightHandDefaultAnimator.SetBool("walk", true);
                rightHandDefaultAnimator.SetBool("run", false);
            }
            else
            {
                rightHandDefaultAnimator.SetBool("walk", false);
                rightHandDefaultAnimator.SetBool("run", false);
            }
        }

        if (_rightHandTrasitionTime < timeToTransitionHand)
        {
            targetRightHand.position = Vector3.Lerp(targetRightHand.position, _currentRightHandTransform.position, _rightHandTrasitionTime / timeToTransitionHand);
            targetRightHand.rotation = Quaternion.Lerp(targetRightHand.rotation, _currentRightHandTransform.rotation, _rightHandTrasitionTime / timeToTransitionHand);

            _rightHandTrasitionTime += Time.deltaTime;
        }
        else
        {
            targetRightHand.position = _currentRightHandTransform.position;

            targetRightHand.rotation = _currentRightHandTransform.rotation;
        }
        #endregion

        #region left hand
        if (ads)
        {
            if (currentLeftHandPosition != LeftHandPosition.ads)
            {
                currentLeftHandPosition = LeftHandPosition.ads;
                _currentLeftHandTransform = gunAdsTarget;

                _leftHandTrasitionTime = 0f;
            }

            if (gunLogic._currentCooldown > .6f)
            {
                gunAdsAnimator.SetBool("shot", true);
                gunAdsAnimator.speed = 1f;
            }
            else if ((Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f) && characterController.isGrounded && !isCrouching)
            {
                gunAdsAnimator.speed = isInWater ? waterSpeed : 1f;
                gunAdsAnimator.SetBool("walk", true);
                gunAdsAnimator.SetBool("shot", false);
            }
            else
            {
                gunAdsAnimator.speed = 1f;
                gunAdsAnimator.SetBool("walk", false);
                gunAdsAnimator.SetBool("shot", false);
            }
        }
        else if (gunIsOn)
        {
            if (currentLeftHandPosition != LeftHandPosition.gun)
            {
                currentLeftHandPosition = LeftHandPosition.gun;
                _currentLeftHandTransform = gunHandTarget;

                _leftHandTrasitionTime = 0f;
            }

            if (reloading)
            {
                gunHandAnimator.speed = 1f;
                gunHandAnimator.SetBool("reload", true);
            }
            else if ((Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f) && characterController.isGrounded && !isCrouching)
            {
                if (inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1)
                {
                    gunHandAnimator.speed = 1f;
                    gunHandAnimator.SetBool("run", true);
                    gunHandAnimator.SetBool("walk", false);
                }
                else
                {
                    gunHandAnimator.speed = isInWater ? waterSpeed : 1f;
                    gunHandAnimator.SetBool("run", false);
                    gunHandAnimator.SetBool("walk", true);
                }
            }
            else
            {

                gunHandAnimator.speed = 1f;
                gunHandAnimator.SetBool("run", false);
                gunHandAnimator.SetBool("walk", false);
            }
        }
        else if (hasObjectInHand)
        {
            if (currentLeftHandPosition != LeftHandPosition.pickupObject)
            {
                currentLeftHandPosition = LeftHandPosition.pickupObject;
                _currentLeftHandTransform = pickupObjectLeftHandTarget;

                _leftHandTrasitionTime = 0f;
            }

            if ((Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f) && characterController.isGrounded && !isCrouching)
            {
                if (inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1)
                {
                    objectHandAnimator.speed = 1f;
                    objectHandAnimator.SetBool("run", true);
                    objectHandAnimator.SetBool("walk", false);
                }
                else
                {
                    objectHandAnimator.speed = isInWater ? waterSpeed : 1f;
                    objectHandAnimator.SetBool("run", false);
                    objectHandAnimator.SetBool("walk", true);
                }
            }
            else
            {
                objectHandAnimator.speed = 1f;
                objectHandAnimator.SetBool("run", false);
                objectHandAnimator.SetBool("walk", false);
            }
        }
        else if (isCrouching || !characterController.isGrounded)
        {
            if (currentLeftHandPosition != LeftHandPosition.crouch)
            {
                currentLeftHandPosition = LeftHandPosition.crouch;
                _rightHandTrasitionTime = 0f;

                _currentLeftHandTransform = crouchLeftHandTarget;

            }
        }
        else
        {
            if (currentLeftHandPosition != LeftHandPosition.walk)
            {
                currentLeftHandPosition = LeftHandPosition.walk;
                _leftHandTrasitionTime = 0f;

                _currentLeftHandTransform = defaultLeftHandTarget;

            }

            else if (isCrouching)
            {
                leftHandDefaultAnimator.SetBool("walk", false);
                leftHandDefaultAnimator.SetBool("run", false);
            }
            else if (inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1)
            {
                leftHandDefaultAnimator.SetBool("walk", false);
                leftHandDefaultAnimator.SetBool("run", true);
            }
            else if ((Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f) && characterController.isGrounded)
            {
                leftHandDefaultAnimator.SetBool("walk", true);
                leftHandDefaultAnimator.SetBool("run", false);
            }
            else
            {
                leftHandDefaultAnimator.SetBool("walk", false);
                leftHandDefaultAnimator.SetBool("run", false);
            }
        }

        if (_leftHandTrasitionTime < timeToTransitionHand)
        {
            targetLeftHand.position = Vector3.Lerp(targetLeftHand.position, _currentLeftHandTransform.position, _leftHandTrasitionTime / timeToTransitionHand);
            targetLeftHand.rotation = Quaternion.Lerp(targetLeftHand.rotation, _currentLeftHandTransform.rotation, _leftHandTrasitionTime / timeToTransitionHand);

            _leftHandTrasitionTime += Time.deltaTime;
        }
        else
        {
            targetLeftHand.position = _currentLeftHandTransform.position;

            targetLeftHand.rotation = _currentLeftHandTransform.rotation;
        }
        #endregion
    }


    public void GroundCheck()
    {
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 2.1f))
        {
            if (isInWater)
            {
                currentSurface = SurfaceType.water;
            }
            else if (groundHit.collider.CompareTag("Ground") && groundHit.transform.TryGetComponent<GroundMaterial>(out GroundMaterial material))
            {
                currentSurface = material.surfaceType;
            }
            else
            {
                currentSurface = SurfaceType.generic;
            }
        }
        else
        {
            groundHit = default; // Reset groundHit if no ground detected
        }
    }

    public void PlayMoveSound()
    {
        _stepTimer -= Time.deltaTime;

        if (!characterController.isGrounded) return;
        if (!(Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f)) return;

        int rand = 0;
        if (isCrouching)
            rand = UnityEngine.Random.Range(0, crouchSteps.Length);
        else if (currentSurface == SurfaceType.metal)
            rand = UnityEngine.Random.Range(0, metalSteps.Length);
        else if (currentSurface == SurfaceType.wood)
            rand = UnityEngine.Random.Range(0, woodSteps.Length);
        else if (currentSurface == SurfaceType.grass)
            rand = UnityEngine.Random.Range(0, grassSteps.Length);
        else if (currentSurface == SurfaceType.dirt)
            rand = UnityEngine.Random.Range(0, dirtSteps.Length);
        else if (currentSurface == SurfaceType.water)
            rand = UnityEngine.Random.Range(0, waterSteps.Length);
        else
            rand = UnityEngine.Random.Range(0, woodSteps.Length);
        
        if (_stepTimer <= 0)
        {
            if (isCrouching)
            {
                FMODSoundManager.instance.PlayOneShot(crouchSteps[rand], transform.position, 0f, 0f);

                _stepTimer = 1.5f;
            }
            else
            {
                PlayStepSound(rand);
            }

        }
    }

    private void PlayStepSound(int randy)
    {
        if (currentSurface == SurfaceType.metal)
            FMODSoundManager.instance.PlayOneShot(metalSteps[randy], transform.position, IsRunning ? 50f : 25f, IsRunning ? 10f : 3f);
        else if (currentSurface == SurfaceType.wood)
            FMODSoundManager.instance.PlayOneShot(woodSteps[randy], transform.position, IsRunning ? 50f : 25f, IsRunning ? 10f : 3f);
        else if (currentSurface == SurfaceType.grass)
            FMODSoundManager.instance.PlayOneShot(grassSteps[randy], transform.position, IsRunning ? 50f : 25f, IsRunning ? 10f : 3f);
        else if (currentSurface == SurfaceType.silent) 
            FMODSoundManager.instance.PlayOneShot(silent[0], transform.position, 0, 0);
        else if (currentSurface == SurfaceType.dirt)
            FMODSoundManager.instance.PlayOneShot(dirtSteps[randy], transform.position, IsRunning ? 50f : 25f, IsRunning ? 10f : 3f);
        else if (currentSurface == SurfaceType.water)
            FMODSoundManager.instance.PlayOneShot(waterSteps[randy], transform.position, IsRunning ? 50f : 25f, IsRunning ? 10f : 3f);
        else
            FMODSoundManager.instance.PlayOneShot(woodSteps[randy], transform.position, IsRunning ? 50f : 25f, IsRunning ? 10f : 3f);

        if (!isInWater)
            _stepTimer = inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? .5f : .25f;
        else
        {
            Instantiate(waterRipple,
                new Vector3(transform.position.x, yCoordForRipple, transform.position.z) + transform.forward * .5f,
                Quaternion.identity);
            _stepTimer = .75f;
        }
    }

    public void DisablePlayerInput(bool ye, bool handMovement = false)
    {
        canMove = !ye;
        canRun = !ye;
        canCrouch = !ye;
        canTorch = !ye;

        moveHands = handMovement ? true : !ye;
    }

    public bool GetRunning()
    {
        return inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1;
    }

}
