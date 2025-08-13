using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class ImpactSound : MonoBehaviour
{
    public EventReference impactHigh;
    public EventReference impactLow;
    public EventReference scrapeSound;
    public float highImpactTreshold;
    public float scrapeTreshold;

    public float impactSoundRange;
    public float impactSoundAggro;

    public float scrapeSoundRange;
    public float scrapeSoundAggro;

    private float _scrapeTimer;
    private float _pickupGrace;

    private bool _isPickedUp;
    private bool _collidedWhilePickedUp;

    private void OnCollisionEnter(Collision collision)
    {
        if (_scrapeTimer <= 0 && !collision.transform.CompareTag("Player") && _pickupGrace <= 0)
        {
            if (_isPickedUp)
                _collidedWhilePickedUp = true;
            if (collision.relativeVelocity.magnitude > highImpactTreshold)
            {
                FMODSoundManager.instance.PlayOneShot(impactHigh, transform.position, impactSoundRange, impactSoundAggro);

                if (collision.transform.CompareTag("Jackinabox"))
                {
                    collision.transform.GetComponentInChildren<JackInABox>().Die();
                }
            }
            else if (collision.relativeVelocity.magnitude > scrapeTreshold)
            {
                FMODSoundManager.instance.PlayOneShot(impactLow, transform.position, impactSoundRange, impactSoundAggro);
            }
            else
            {
                FMODSoundManager.instance.PlayOneShot(scrapeSound, transform.position, scrapeSoundRange, scrapeSoundAggro);
            }

            _scrapeTimer = .65f;

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _collidedWhilePickedUp = false;
        _scrapeTimer = 0f;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (_scrapeTimer <= 0 && !collision.transform.CompareTag("Player") && _pickupGrace <= 0)
        {
            if (_isPickedUp)
                _collidedWhilePickedUp = true;
            if (collision.relativeVelocity.magnitude > highImpactTreshold)
            {
                FMODSoundManager.instance.PlayOneShot(impactHigh, transform.position, impactSoundRange, impactSoundAggro);
            }
            else if (collision.relativeVelocity.magnitude > scrapeTreshold)
            {
                FMODSoundManager.instance.PlayOneShot(impactLow, transform.position, impactSoundRange, impactSoundAggro);
            }
            else
            {
                FMODSoundManager.instance.PlayOneShot(scrapeSound, transform.position, scrapeSoundRange, scrapeSoundAggro);
            }

            _scrapeTimer = .65f;
        }
    }

    private void Update()
    {
        if (_scrapeTimer > 0 && !_isPickedUp && !_collidedWhilePickedUp)
        {
            _scrapeTimer -= Time.deltaTime;
        }

        if (_pickupGrace > 0)
        {
            _pickupGrace -= Time.deltaTime;
        }
    }

    public void PickupGrace()
    {
        _pickupGrace = .25f;
    }

    public void Pickup(bool yes)
    {
        _isPickedUp = yes;

        if (!yes)
        {
            _collidedWhilePickedUp = false;
            _scrapeTimer = 0;
        }
    }
}
