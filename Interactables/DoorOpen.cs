using FMODUnity;
using System.Collections;
using UnityEngine;

public enum DoorState
{
    close = 0,
    open = 1
}

public class DoorOpen : MonoBehaviour, IInteractable
{
    [Tooltip("The transform that will get moved upwards")]
    public Transform door;
    public Light interactionLight;

    public OcclusionPortal portal;

    public DoorOpen connectedPanel;

    public bool orangeDoor;
    [NaughtyAttributes.EnableIf("orangeDoor")]
    public EventReference soundToPlayOnForce;
    [NaughtyAttributes.EnableIf("orangeDoor")]
    public Transform forceSoundSource;
    private bool _isForced;

    public EventReference accessGranted;
    public EventReference accessDenied;

    public EventReference doorOpens;
    public EventReference doorCloses;

    public bool openAndClose;
    public DoorState startState;
    [NaughtyAttributes.ReadOnly] public DoorState _currentState;
    [NaughtyAttributes.ReadOnly] public bool _isOpencloseing;
    public float timeToOpenClose;
    public bool canBeInteractedWith;

    private bool _isFading;

    public float startHeight;
    public float openHeight;

    public bool helmetScanner;

    private NewCharacterController _player;

    private void Awake()
    {
        _currentState = DoorState.close;
        _isOpencloseing = false;
    }

    private void Start()
    {
        if (portal == null && door != null)
        {
            portal = door.GetComponentInParent<OcclusionPortal>();
        }
        if (startState == DoorState.open)
        {
            StartCoroutine(OpenClose());
            //_currentState = DoorState.open;
        }
    }

    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        _player = player.GetComponent<NewCharacterController>();

        if (canBeInteractedWith && !_isOpencloseing)
        {
            if (!helmetScanner)
                StartCoroutine(OpenClose());
            else if (helmetScanner && _player.helmet)
                StartCoroutine(OpenClose());

        }

        if (!_isFading)
            StartCoroutine(LightFade(0f));
    }

    public void ForceOpen()
    {
        _isForced = true;
        StartCoroutine(OpenClose());

        if (!soundToPlayOnForce.IsNull)
            FMODSoundManager.instance.PlayOneShot(soundToPlayOnForce, forceSoundSource.position, 50f, 25f);

        StartCoroutine(LightFade(0f));
    }

    private IEnumerator LightFade(float targetRange)
    {
        if (canBeInteractedWith)
        {
            if ((helmetScanner && !_player.helmet))
                FMODSoundManager.instance.PlayOneShot(accessDenied, transform.position, 50f, 5f);
            else
                FMODSoundManager.instance.PlayOneShot(accessGranted, transform.position, 50f, 25f);
        }
        else
            FMODSoundManager.instance.PlayOneShot(accessDenied, transform.position, 50f, 5f);

        _isFading = true;

        float startingRange = interactionLight.range;

        float targetTime = .5f;
        float elapsedTime = 0f;

        while (elapsedTime < targetTime)
        {
            interactionLight.range = Mathf.Lerp(startingRange, targetRange, elapsedTime / targetTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (!_isForced)
        {
            if (targetRange == 0f)
                StartCoroutine(LightFade(.5f));
            if (targetRange > 0f)
                _isFading = false;
        }
        
    }

    private IEnumerator OpenClose()
    {
        _isOpencloseing = true;

        if (_currentState == DoorState.close)
            door.gameObject.GetComponent<OcclusionPortal>().open = true;

        if (connectedPanel != null)
            connectedPanel.ChangeState(_currentState);

        FMODSoundManager.instance.PlayOneShot(_currentState == DoorState.close ? doorOpens : doorCloses, transform.position, 50f, 25f);

        float targetHeight = _currentState == DoorState.close ? door.localPosition.y + openHeight : door.localPosition.y - openHeight;
        Vector3 startPosition = door.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < timeToOpenClose)
        {
            door.localPosition = Vector3.Lerp(startPosition,
                new Vector3(door.localPosition.x, targetHeight, door.localPosition.z),
                elapsedTime / timeToOpenClose);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        door.localPosition = new Vector3(door.localPosition.x, targetHeight, door.localPosition.z);
        _currentState = _currentState == DoorState.close ? DoorState.open : DoorState.close;

        _isOpencloseing = false;

        if (connectedPanel != null)
            connectedPanel.ChangeState(_currentState);

        if (_currentState == DoorState.close)
            door.gameObject.GetComponent<OcclusionPortal>().open = false;

        if (_isForced)
        {
            Destroy(GetComponent<DoorOpen>());
        }

    }

    public void ChangeState(DoorState state)
    {
        _isOpencloseing = !_isOpencloseing;
        _currentState = state;
    }

    public DoorState GetState()
    {
        return _currentState;
    }

    void IInteractable.StopInteract()
    {

    }
}
