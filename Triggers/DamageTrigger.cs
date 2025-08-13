using FMODUnity;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    TriggerType Type { get; set; }

    public float damage;

    public float delay;

    public bool destroy;

    public EventReference damageSound;

    public bool playFeelEffect;
    [NaughtyAttributes.EnableIf("playFeelEffect")]
    public FeelEffectToPlay feelEffect;

    private NewCharacterController _player;
    

    void OnTriggerEnter(Collider player)
    {
        _player = player.GetComponent<NewCharacterController>();
        Invoke("TriggerDamage", delay);
    }

    private void TriggerDamage()
    {
        if (_player.transform.TryGetComponent<HealthManager>(out HealthManager hpMan))
        {
            if (hpMan.currentHealth > damage)
                hpMan.RemoveHP(damage);
            else
            {
                hpMan.RemoveHP(hpMan.currentHealth - 1);
            }
        }

        if (playFeelEffect)
        {
            _player.GetComponent<FeelEffectsManager>().PlayEvent(feelEffect);
        }

        FMODSoundManager.instance.PlayOneShot(damageSound, _player.transform.position, 0f, 0f);

        if (destroy)
            Destroy(gameObject);
    }
}
