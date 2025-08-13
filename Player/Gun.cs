using FMOD.Studio;
using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;

public enum GunOut
{
    Pistol = 0,
    Srynge = 1
}

public class Gun : MonoBehaviour
{
    public InputManager inputManager;

    public GunOut currentGun;

    public FeelEffectsManager feelEffectsManager;

    [Header("Pistol")]
    public float fireCooldown;

    public FeelEffectToPlay gunShootEffect;
    public GameObject smokeEffectForGun;

    public int damage;

    public ParticleSystem particles;
    public ParticleSystem impactParticle;
    private TrailRenderer bulletTrail;

    public GameObject ammoCounter;
    public bool showAmmoCounter;
    public int loadedAmmo;
    public int reserveAmmo;
    public TextMeshProUGUI loadedAmmoText;
    public TextMeshProUGUI maxAmmoText;

    public Animator sliderAnimator;

    public LayerMask layerMask;

    public Light shootLight;
    public float shootLightRange;

    public NewCharacterController player;

    public EventReference gunShotSound;
    public EventReference reloadSound;
    public EventReference gunEmpty;

    private EventInstance _playingReloadSound;

    [Header("Srynge")]
    public int srynges;
    public int healing;

    public FeelEffectToPlay sryngeEffect;

    public float sryngeCooldown;

    public HealthManager healthManager;

    public GameObject sryngeUIElement;
    public TextMeshProUGUI numberOfSryngesText;

    public EventReference sryngeUse;

    [NaughtyAttributes.ReadOnly]public float _currentCooldown;

    private void Start()
    {
        loadedAmmoText.text = loadedAmmo.ToString();
        maxAmmoText.text = reserveAmmo.ToString();
        ammoCounter.SetActive(showAmmoCounter);

        if (player.canSrynge)
        {
            sryngeUIElement.SetActive(true);
        }
        else
        {
            sryngeUIElement.SetActive(false);
        }

        _currentCooldown = fireCooldown;

        shootLight.range = 0f;

        inputManager.inputMaster.CameraLook.Interact.performed += _ => Shoot();
    }

    public void Shoot()
    {
        if (currentGun == GunOut.Pistol)
        {
            if (player.ads && _currentCooldown <= 0)
            {
            
                if (loadedAmmo > 0)
                {
                    sliderAnimator.Play("gunSlideShot");

                    feelEffectsManager.PlayEvent(gunShootEffect);

                    smokeEffectForGun.GetComponent<ParticleSystem>().Play();  

                    loadedAmmo--;
                    loadedAmmoText.text = loadedAmmo.ToString();

                    particles.Play();

                    FMODSoundManager.instance.PlayOneShot(gunShotSound, particles.transform.position, 150f, 150f);

                    if (Physics.Raycast(player.virtualCamera.transform.position, GetDirection(), out RaycastHit hit, 100f, layerMask, QueryTriggerInteraction.Collide))
                    {
                        //TrailRenderer trail = Instantiate(bulletTrail, particles.transform.position, Quaternion.identity);
                        //StartCoroutine(SpawnTrail(trail, hit));

                        if (hit.transform.CompareTag("Turret"))
                        {
                            hit.transform.gameObject.GetComponent<TurretHealth>().Damage(damage);
                            Debug.Log("Turret was hit");
                        }
                        else if (hit.transform.CompareTag("Angler"))
                        {
                            hit.transform.gameObject.GetComponent<BrainThinker>().damageTaker.Damage(hit.transform.gameObject.GetComponent<BrainThinker>(), damage);
                            Debug.Log("Angler was hit");
                        }
                        else if (hit.transform.CompareTag("Slime"))
                        {
                            hit.transform.gameObject.GetComponent<EnemyHPManager>().Damage(damage);
                            Debug.Log("Slime was hit");
                        }
                        else if (hit.transform.CompareTag("Morana"))
                        {
                            hit.transform.gameObject.GetComponent<DissolusionSpawner>().SpawnDissolusion();
                            Debug.Log("Morana was hit");
                        }
                        else if (hit.transform.CompareTag("Jackinabox"))
                        {
                            hit.transform.GetComponentInChildren<JackInABox>().Die();
                            Debug.Log("Jack in a box was hit");
                        }
                        else if (hit.transform.CompareTag("Grab Object"))
                        {
                            hit.transform.GetComponentInChildren<PickupObject>().rb.AddForce(GetDirection() * 1500);
                            Debug.Log("Grab object was hit");
                        }
                        else if (hit.transform.CompareTag("In hand object"))
                        {
                            hit.transform.GetComponent<Rigidbody>().AddForce(GetDirection() * 1500);
                            Debug.Log("In hand object was hit");
                        }
                        else if (hit.transform.CompareTag("Orange door") && hit.transform.GetComponent<DoorOpen>() != null)
                        {
                            hit.transform.gameObject.GetComponent<DoorOpen>().ForceOpen();
                            Instantiate(impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
                            Debug.Log("Orange door was hit");
                        }
                        else
                        {
                            Instantiate(impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
                            Debug.Log("Ground was hit");
                        }
                    }

                    shootLight.gameObject.SetActive(true);
                    shootLight.range = shootLightRange;

                    player.cameraTransform.GetComponent<CameraLook>().shot = true;

                    _currentCooldown = fireCooldown;
                }
                else
                {
                    FMODSoundManager.instance.PlayOneShot(gunEmpty, transform.position, 4, 20f);
                }
            }
        }
        else if (currentGun == GunOut.Srynge)
        {
            if (player.ads && _currentCooldown <= 0)
            {
                if (srynges > 0)
                {
                    srynges--;

                    Invoke("DelayedHeal", .3f);

                    _currentCooldown = sryngeCooldown;
                }
                else
                {
                    FMODSoundManager.instance.PlayOneShot(gunEmpty, transform.position, 4, 20f);
                }
            }
            
        }
    }

    private void DelayedHeal()
    {
        healthManager.AddHP(healing);
        FMODSoundManager.instance.PlayOneShot(sryngeUse, transform.position, 15f, 10f);
        feelEffectsManager.PlayEvent(sryngeEffect);
    }

    bool torchWasOn = false;

    public void ActivateSryngeUI()
    {
        sryngeUIElement.SetActive(true);
    }

    public void Reload()
    {
        if (reserveAmmo > 0 && loadedAmmo != 6f && !player.reloading)
        {
            player.reloading = true;
            //FMODSoundManager.instance.PlayOneShot(reloadSound, transform.position, 5f, 5f);
            _playingReloadSound = RuntimeManager.CreateInstance(reloadSound);
            _playingReloadSound.start();

            if (player.torchIsOn)
            {
                torchWasOn = true;
                player.Torch();
            }
            else
                torchWasOn = false;

            player.canTorch = false;

            player.CancelAds();
            player.PreventAds();

            Invoke("FinishReload", 2.05f);
        }
    }

    public void AddAmmo()
    {

        int ammoNeeded = 6 - loadedAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        loadedAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        maxAmmoText.text = reserveAmmo.ToString();
        loadedAmmoText.text = loadedAmmo.ToString();
    }

    public void FinishReload()
    {
        player.reloading = false;
        player.gunHandAnimator.SetBool("reload", false);
        player.rightHandDefaultAnimator.SetBool("reload", false);
        player.PermitAds();

        player.canTorch = true;

        if (torchWasOn)
        {
            player.Torch();
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        float distance = Vector3.Distance(trail.transform.position, hit.point);
        float startingDistance = distance;

        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * 5;

            yield return null;
        }

        trail.transform.position = hit.point;
        Instantiate(impactParticle, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = player.virtualCamera.transform.forward;
        direction.Normalize();
        return direction;
    }

    private void Update()
    {
        _currentCooldown -= Time.deltaTime;

        shootLight.range = Mathf.MoveTowards(shootLight.range, 0f, Time.deltaTime * 50f);

        if (shootLight.range == 0f)
            shootLight.gameObject.SetActive(false);

        if (player.reloading)
        {
            _playingReloadSound.set3DAttributes(RuntimeUtils.To3DAttributes(player.transform.position));
        }

        loadedAmmoText.text = loadedAmmo.ToString();
        maxAmmoText.text = reserveAmmo.ToString();

        numberOfSryngesText.text = srynges.ToString();
    }
}
