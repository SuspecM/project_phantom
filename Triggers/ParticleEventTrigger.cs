using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class ParticleEventTrigger : MonoBehaviour
{
    [field: SerializeField]
    public TriggerType Type { get; set; }

    public ParticleSystem[] particles;
    public bool[] particleHasSoundEmitter;

    public EventLight[] eventLights;

    public EventReference[] sounds;
    public Transform soundSource;

    public bool cameraShake;
    [EnableIf("cameraShake")]
    public FeelEffectToPlay effectToPlay;
    public FeelEffectsManager feelEffectsManager;


    private void OnTriggerEnter(Collider collision)
    {
        if (particles.Length > 0) 
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Play();

                if (particleHasSoundEmitter[i])
                {
                    particles[i].GetComponent<StudioEventEmitter>().Play();
                }
            }
        }

        
        
        if (eventLights.Length > 0)
        {
            foreach (var item in eventLights)
            {
                item.PlayEventLight();
            }
        }
        
        if (sounds.Length > 0)
        {
            foreach (var item in sounds)
            {
                FMODSoundManager.instance.PlayOneShot(item, soundSource.position, 0f, 0f);
            }
        }
        

        if (cameraShake)
            feelEffectsManager.PlayEvent(effectToPlay);

        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider collision)
    {
    }

    private void OnTriggerStay(Collider collision)
    {

    }

    private void Start()
    {
        Invoke("DelayedStart", 2f);
    }

    private void DelayedStart()
    {
        feelEffectsManager = FindAnyObjectByType<FeelEffectsManager>();
    }
}
