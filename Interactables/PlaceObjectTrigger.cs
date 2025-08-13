using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class PlaceObjectTrigger : MonoBehaviour, IPuzzle
{
    public GameObject objectToAppearOnTrigger;
    public bool activeState;
    public string tagToTrigger;

    public GameObject[] wiresToDeactivate;
    public GameObject[] wiresToActivate;

    public bool playNoise;
    public EventReference noiseToPlay;

    private bool _moveObject;
    private float _startYCoord;
    private float _moveYCoord = .79f;

    public bool PuzzleIsDone { get; set; }

    private void Start()
    {
        foreach (var item in wiresToDeactivate)
        {
            item.SetActive(true);
        }

        foreach (var item in wiresToActivate)
        {
            item.SetActive(false);
        }

        _startYCoord = objectToAppearOnTrigger.transform.position.y;
        objectToAppearOnTrigger.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == tagToTrigger)
        {
            PuzzleIsDone = true;

            other.gameObject.SetActive(false);
            objectToAppearOnTrigger.SetActive(true);
            _moveObject = true;
            activeState = true;

            FMODSoundManager.instance.PlayOneShot(noiseToPlay, transform.position, 50f, 10f);

            foreach (var item in wiresToDeactivate)
            {
                item.SetActive(false);
            }

            foreach (var item in wiresToActivate)
            {
                item.SetActive(true);
            }

            Destroy(this.GetComponent<Collider>());
        }
    }

    [Button("Complete puzzle")]
    public void PuzzleDone()
    {
        PuzzleIsDone = true;

        objectToAppearOnTrigger.SetActive(true);
        _moveObject = true;
        activeState = true;

        FMODSoundManager.instance.PlayOneShot(noiseToPlay, transform.position, 0f, 0f);

        foreach (var item in wiresToDeactivate)
        {
            item.SetActive(false);
        }

        foreach (var item in wiresToActivate)
        {
            item.SetActive(true);
        }
    }

    private void Update()
    {
        if (_moveObject)
        {
            objectToAppearOnTrigger.transform.position = new Vector3(
                objectToAppearOnTrigger.transform.position.x,
                Mathf.MoveTowards(objectToAppearOnTrigger.transform.position.y, _startYCoord - _moveYCoord, Time.deltaTime),
                objectToAppearOnTrigger.transform.position.z
                );
        }
    }
}
