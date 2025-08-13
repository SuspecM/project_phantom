using NaughtyAttributes;
using UnityEngine;

public class ObjectSpawnTrigger : MonoBehaviour
{
    [field: SerializeField]
    public TriggerType Type { get; set; }

    public GameObject[] objects;

    [Tooltip("Wether the given objects should be activeated or deactivated.")]
    public bool activate;
    [Tooltip("Wether the given objects should be destroyed.")]
    public bool destroy;

    public bool pushObject;
    [EnableIf("pushObject")]
    public Vector3 pushForce;

    public bool cameraShake;
    [EnableIf("cameraShake")]
    public FeelEffectToPlay effectToPlay;
    public FeelEffectsManager feelEffectsManager;

    public bool destroyOnTrigger;

    private void OnTriggerEnter(Collider collision)
    {
        feelEffectsManager = FindAnyObjectByType<FeelEffectsManager>();

        foreach (var item in objects)
        {
            if (destroy)
                Destroy(item);
            else
                item.SetActive(activate);

            if (pushObject)
                item.GetComponent<Rigidbody>().AddForce(pushForce);
        }

        if (cameraShake)
            feelEffectsManager.PlayEvent(effectToPlay);

        if (destroyOnTrigger)
            Destroy(gameObject);
    }

    private void OnTriggerExit(Collider collision)
    {
    }

    private void OnTriggerStay(Collider collision)
    {

    }
}
