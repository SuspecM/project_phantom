using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ElectricityDamage : MonoBehaviour
{
    public float damagePerTick;
    public float timeBetweenDamageTick;
    private float _tickTimer;

    public EventReference electricity;
    public EventInstance playingElectricity;

    private void Start()
    {
        playingElectricity = RuntimeManager.CreateInstance(electricity);
        playingElectricity.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        playingElectricity.start();
    }

    private void Update()
    {
        _tickTimer = Mathf.Clamp(_tickTimer - Time.deltaTime, 0, 5);
    }

    private void OnEnable()
    {
        playingElectricity.setPaused(false);
    }

    private void OnDisable()
    {
        playingElectricity.setPaused(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (_tickTimer == 0)
        {
            other.TryGetComponent(out HealthManager player);
            if (player != null)
            {
                player.RemoveHP(damagePerTick);
                _tickTimer = timeBetweenDamageTick;
            }
        }
    }
}
