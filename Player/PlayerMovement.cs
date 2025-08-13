using FMODUnity;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;



public class PlayerMovement : MonoBehaviour
{
    /*[Header("Basic references")]
    public InputManager inputManager;

    public Transform standupTestTransform;
    public Transform playerCameraTransform;

    public Rigidbody rb;

    public CharacterController characterController;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 15;
    public float crouchMoveSpeed = 5;

    public bool canRun;
    public bool canMove;

    public float gravity = 30f;

    public float jumpForce = 200f;
    [SerializeField][NaughtyAttributes.ReadOnly] private float _jumpGrace = 1f;

    [SerializeField]
    [NaughtyAttributes.ReadOnly]
    private bool _isGrounded;
    [SerializeField]
    [NaughtyAttributes.ReadOnly]
    private float _coyoteTime;
    [SerializeField]
    [NaughtyAttributes.ReadOnly]
    private bool _canJump;

    [Header("Footstep sounds for different materials")]
    [HideInInspector] public SurfaceType currentSurface;
    public EventReference[] metalSteps;
    public EventReference[] woodSteps;

    private Vector3 _forceDirection;
    private bool _collisionIsWithGrabObject;

    private float _stepTimer;

    [Header("Crouching")]
    [SerializeField]
    [Tooltip("Relative height")]
    public float crouchHeight = .5f;
    public float standHeight = 1f; private bool _isCrouching;
    private bool _isInCrouchAnimation;
    private float timeToCrouch = .25f;
    public bool canCrouch;

    [Header("Headbob")]
    public bool headBob = true;
    public float walkBobSpeed = 14f;
    public float walkBobAmmount = 0.05f;
    public float RunBobSpeed = 18f;
    public float RunBobAmmount = .1f;
    public float crouchBobSpeed = 8f;
    public float crouchBobAmmount = 0.025f;

    private float defaultYPos = 0f;
    private float bobTimer;

    [Header("Hand movement")]
    public bool moveHands;

    public HandPosition currentHandPosition;
    [SerializeField][NaughtyAttributes.ReadOnly] private float _handTrasitionTime;
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

    public Animator leftHandDefaultAnimator;
    public Animator rightHandDefaultAnimator;
    public Animator leftHandRunAnimator;
    public Animator rightHandRunAnimator;

    private void Awake()
    {
        targetLeftHand.position = defaultLeftHandTarget.position;
        targetRightHand.position = defaultRightHandTarget.position;

        _currentLeftHandTransform = defaultLeftHandTarget;
        _currentRightHandTransform = defaultRightHandTarget;
    }

    private void Start()
    {
        defaultYPos = playerCameraTransform.transform.position.y;

        inputManager.inputMaster.Movement.Jump.started += _ => Jump();
        inputManager.inputMaster.Movement.Crouch.started += _ => Crouch();

        _coyoteTime = .25f;
    }

    private void Update()
    {
        if (!_isGrounded && _coyoteTime > 0)
        {
            _coyoteTime -= Time.deltaTime;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    PickupObject pickup = null;
    //    if (collision.transform.CompareTag("Grab Object"))
    //    {
    //        collision.transform.TryGetComponent<PickupObject>(out pickup);
    //    }
    //    else
    //    {
    //        pickup = null;
    //    }

    //    if (collision.transform.CompareTag("Ground") || (pickup != null && pickup.ObjectIsGrounded()))
    //    {
    //        if (_jumpGrace <= 0)
    //        {
    //            _isGrounded = true;
    //            _canJump = true;
    //            _coyoteTime = .25f;
    //        }

    //    }
    //}

    private void OnCollisionStay(Collision collision)
    {
        PickupObject pickup = null;
        _collisionIsWithGrabObject = collision.transform.CompareTag("Grab Object");
        if (collision.transform.CompareTag("Grab Object"))
        {
            collision.transform.TryGetComponent<PickupObject>(out pickup);
        }
        else
        {
            pickup = null;
        }

        // sometimes if the player spams the jump and crouch, the player can get stuck not being able to jump or stand up again, so we do this check here as well
        if (collision.transform.CompareTag("Ground") || (pickup != null && pickup.ObjectIsGrounded()))
        {

            if (collision.transform.TryGetComponent<GroundMaterial>(out GroundMaterial material))
            {
                currentSurface = material.surfaceType;
            }
            else
            {
                currentSurface = SurfaceType.generic;
            }
        }

        foreach (var contact in collision.contacts)
        {
            Vector3 collisionNormal = contact.normal;
            _forceDirection = Vector3.zero;

            if (IsSide(collisionNormal, Vector3.up) && _jumpGrace <= 0)
            {
                //Debug.Log("Collided with the top of the object");
                _forceDirection = Vector3.zero;
                _isGrounded = true;
                _canJump = true;
                _coyoteTime = .25f;
            }
            else if (IsSide(collisionNormal, Vector3.down))
            {
                //Debug.Log("Collided with the bottom of the object");
                _forceDirection = Vector3.up;
            }
            else if (IsSide(collisionNormal, Vector3.left))
            {
                //Debug.Log("Collided with the left side of the object");
                _forceDirection = Vector3.right;
            }
            else if (IsSide(collisionNormal, Vector3.right))
            {
                //Debug.Log("Collided with the right side of the object");
                _forceDirection = Vector3.left;
            }
            else if (IsSide(collisionNormal, Vector3.forward))
            {
                //Debug.Log("Collided with the front of the object");
                _forceDirection = Vector3.back;
            }
            else if (IsSide(collisionNormal, Vector3.back))
            {
                //Debug.Log("Collided with the back of the object");
                _forceDirection = Vector3.forward;
            }
        }

    }

    bool IsSide(Vector3 normal, Vector3 direction, float threshold = 0.5f)
    {
        return Vector3.Dot(normal, direction) > threshold;
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Grab Object"))
        {
            _isGrounded = false;
        }
    }

    void Jump()
    {
        if (_isGrounded || (_coyoteTime > 0 && _canJump))
        {
            if (_isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce);
            }
            else
            {
                float newJumpForce = jumpForce + (-Physics.gravity.y);
                rb.AddForce(Vector3.up * newJumpForce);
            }

            _jumpGrace = 1f;
            _canJump = false;
            _isGrounded = false;
        }
    }

    void Crouch()
    {

        if (!_isInCrouchAnimation && canCrouch)
            StartCoroutine(CrouchStand());
    }

    private IEnumerator CrouchStand()
    {
        //Debug.Log("Crouching started");

        if (_isCrouching && Physics.Raycast(standupTestTransform.position, Vector3.up, 1f))
        {
            yield break;
        }

        _isInCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = _isCrouching ? standHeight : crouchHeight;
        float currentHeight = transform.localScale.y;

        //Debug.Log($"Target height: {targetHeight}, current height: {currentHeight}");

        while (timeElapsed < timeToCrouch)
        {
            // Debug.Log($"Crouching while loop, time elapsed: {timeElapsed}, curren");
            transform.localScale = new Vector3(
                1f,
                Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch),
                1f);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.localScale = new Vector3(1f, targetHeight, 1f);

        _isCrouching = !_isCrouching;

        //Debug.Log("Crouching finished");

        _isInCrouchAnimation = false;
    }

    private void HandleHeadBob(Vector3 move)
    {
        if (!_isGrounded) return;

        if (Mathf.Abs(move.x) > .1f || Mathf.Abs(move.z) > .1f)
        {
            bobTimer += Time.deltaTime * (_isCrouching ? crouchBobSpeed : inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? walkBobSpeed : RunBobSpeed);
            playerCameraTransform.localPosition = new Vector3(
                playerCameraTransform.localPosition.x,
                defaultYPos + Mathf.Sin(bobTimer) * (_isCrouching ? crouchBobAmmount : inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? walkBobAmmount : RunBobAmmount),
                playerCameraTransform.localPosition.z
                );
        }
    }

    private void FixedUpdate()
    {
        _jumpGrace = Mathf.Clamp(_jumpGrace - Time.deltaTime, 0f, 1f);

        if (canMove)
        {
            float forward = inputManager.inputMaster.Movement.Forward.ReadValue<float>();
            float right = inputManager.inputMaster.Movement.Right.ReadValue<float>();
            Vector3 move = transform.right * right + transform.forward * forward;

            if (_isCrouching)
            {
                move *= crouchMoveSpeed;
            }
            else if (canRun && _isGrounded)
            {
                move *= inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? walkSpeed : runSpeed;
            }
            else
            {
                move *= walkSpeed;
            }

            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            // to prevent the player from going flying when walking down stairs
            rb.velocity = AdjustVelocityToSlope(rb.velocity);

            if (headBob)
                HandleHeadBob(move);

            // Apply force in the opposite direction of the collision normal so that the player doesn't get stuck mid air
            if (_forceDirection != Vector3.zero && rb.velocity.y > 0f && !_collisionIsWithGrabObject)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

        }

        PlayMoveSound();

        HandleArmMovement();
    }

    public Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, .2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0 && _jumpGrace <= 0)
            {
                return adjustedVelocity;
            }
        }
        // to prevent the player from getting launched up the fly from walking up a few stairs we do an extra check for grounded
        return _isGrounded ? rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z) : velocity;
    }

    public void HandleArmMovement()
    {
        if (inputManager.inputMaster.Movement.Run.ReadValue<float>() == 1 && (Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f) && _isGrounded && !_isCrouching)
        {
            if (currentHandPosition != HandPosition.run)
            {
                currentHandPosition = HandPosition.run;
                _handTrasitionTime = 0f;

                _currentLeftHandTransform = runLeftHandTarget;
                _currentRightHandTransform = runRightHandTarget;

            }
        }
        else if (_isCrouching || !_isGrounded)
        {
            if (currentHandPosition != HandPosition.crouch)
            {
                currentHandPosition = HandPosition.crouch;
                _handTrasitionTime = 0f;

                _currentLeftHandTransform = crouchLeftHandTarget;
                _currentRightHandTransform = crouchRightHandTarget;

                leftHandRunAnimator.StopPlayback();
                rightHandRunAnimator.StopPlayback();
            }
        }
        else
        {
            if (currentHandPosition != HandPosition.walk)
            {
                currentHandPosition = HandPosition.walk;
                _handTrasitionTime = 0f;

                _currentLeftHandTransform = defaultLeftHandTarget;
                _currentRightHandTransform = defaultRightHandTarget;
            }

            if ((Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f) && _isGrounded)
            {
                leftHandDefaultAnimator.Play("move");
                rightHandDefaultAnimator.Play("move");
            }
            else
            {
                leftHandDefaultAnimator.Play("idle");
                rightHandDefaultAnimator.Play("idle");
            }
        }

        if (_handTrasitionTime < timeToTransitionHand)
        {
            targetLeftHand.position = Vector3.Lerp(targetLeftHand.position, _currentLeftHandTransform.position, _handTrasitionTime / timeToTransitionHand);
            targetLeftHand.rotation = Quaternion.Lerp(targetLeftHand.rotation, _currentLeftHandTransform.rotation, _handTrasitionTime / timeToTransitionHand);
            targetRightHand.position = Vector3.Lerp(targetRightHand.position, _currentRightHandTransform.position, _handTrasitionTime / timeToTransitionHand);
            targetRightHand.rotation = Quaternion.Lerp(targetRightHand.rotation, _currentRightHandTransform.rotation, _handTrasitionTime / timeToTransitionHand);

            _handTrasitionTime += Time.deltaTime;
        }
        else
        {
            targetLeftHand.position = _currentLeftHandTransform.position;
            targetRightHand.position = _currentRightHandTransform.position;

            targetLeftHand.rotation = _currentLeftHandTransform.rotation;
            targetRightHand.rotation = _currentRightHandTransform.rotation;
        }

    }

    public void PlayMoveSound()
    {
        _stepTimer -= Time.deltaTime;

        if (!_isGrounded) return;
        if (!(Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f)) return;

        int rand = 0;
        if (currentSurface == SurfaceType.metal)
            rand = UnityEngine.Random.Range(0, metalSteps.Length);
        else if (currentSurface == SurfaceType.wood)
            rand = UnityEngine.Random.Range(0, woodSteps.Length);
        else
            rand = UnityEngine.Random.Range(0, woodSteps.Length);

        if (_stepTimer <= 0)
        {
            if (currentSurface == SurfaceType.metal)
                FMODSoundManager.instance.PlayOneShot(metalSteps[rand], transform.position);
            else if (currentSurface == SurfaceType.wood)
                FMODSoundManager.instance.PlayOneShot(woodSteps[rand], transform.position);
            else
                FMODSoundManager.instance.PlayOneShot(woodSteps[rand], transform.position);

            if (_isCrouching) _stepTimer = .5f;
            else _stepTimer = inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? .5f : .25f;
        }
    }

    public bool GetRunning()
    {
        return inputManager.inputMaster.Movement.Run.ReadValue<float>() == 0 ? false : true;
    }*/
}
