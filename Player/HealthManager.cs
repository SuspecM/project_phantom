using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public GameObject ui;
    public float currentHealth;
    public float maxHealth;

    public Image healthIndicator;
    public GameObject[] heart1;
    public GameObject[] heart2;
    public GameObject[] heart3;
    public GameObject criticalInjuryText;
    public GameObject criticalInjuryText2;
    public bool crit2;
    public GameObject criticalInjuryText3;
    public bool crit3;
    public GameObject criticalInjuryText4;
    public bool crit4;
    public bool finalCrit;
    public bool criticalInjury;
    private float _criticalInjuryTimer;

    private bool _appliedEffect;
    private bool _shouldShowUp;

    private bool _firstTimeAnimation;

    public GameObject cursor;

    public Image hurt;
    public Image heal;

    public string levelName;

    public NewCharacterController characterController;
    public CameraLook characterLook;

    public EventReference collapseOnGround;
    public EventReference spiderEvent;

    public CharacterController characterCollider;

    public CameraFade cameraFade;

    private float desiredPercent;
    private float currentPercent;

    private float healAlpha;
    private float desiredHealAlpha;

    private float hurtAlpha;
    private float desiredHurtAlpha;

    public Animator cameraAnimator;

    private bool _immunityFrames;
    private PersistentGameManager gameManager;

    public Light deconLight;
    private bool _isInDeathAnimation;
    public CinemachineVirtualCamera virtualCamera;
    private float _timeSinceDeathStarted;
    // decon event light shit
    private float _currentColors;
    private float _currentRange;

    private float _currentStatic;
    private float _currentStaticStrenght;

    public Transform placeEnemyOnDeat;

    public EventReference staticSounds;
    private EventInstance _playingStatic;

    private void Start()
    {
        gameManager = FindFirstObjectByType<PersistentGameManager>();

        criticalInjuryText.SetActive(false);
        criticalInjuryText2.SetActive(false);
        criticalInjuryText3.SetActive(false);
        criticalInjuryText4.SetActive(false);

        HealthIndicator();

        healAlpha = 0f;
        hurtAlpha = 0f;

        currentPercent = 0;
        desiredPercent = currentHealth / 100;

        if (!characterController.helmet)
        {
            ui.SetActive(false);
        }

        gameManager.GetComponent<Glitchcontroller>().noiseAmount = 0;
        gameManager.GetComponent<Glitchcontroller>().glitchStrenght = 2;
    }

    public void ActivateHUD()
    {
        ui.SetActive(true);
        cursor.SetActive(true);
    }

    public void DeactivateHUD()
    {
        ui.SetActive(false);
        cursor.SetActive(false);
    }

    public void DeactivateHUD(bool deactivateCrosshair)
    {
        ui.SetActive(false);
        if (deactivateCrosshair)
            cursor.SetActive(false);
    }

    [NaughtyAttributes.Button]
    public void AddHP(float value = 4)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, 100);
        desiredPercent = currentHealth / 100;

        heal.color = new Color(heal.color.r, heal.color.g, heal.color.b, 1f);
        healAlpha = 1f;
        desiredHealAlpha = 0f;

        HealthIndicator();
    }

    [NaughtyAttributes.Button]
    public void RemoveHP(float value = 97)
    {
        if (!_immunityFrames)
        {
            _immunityFrames = true;

            GetComponent<FeelEffectsManager>().PlayEvent(FeelEffectToPlay.BreakEvent);

            currentHealth = Mathf.Clamp(currentHealth - value, 0, 100);

            desiredPercent = currentHealth / 100;

            hurt.color = new Color(hurt.color.r, hurt.color.g, hurt.color.b, 1f);
            hurtAlpha = 1f;

            if (currentHealth > 0)
            {
                desiredHurtAlpha = 0f;
            }
            else
            {
                desiredHurtAlpha = 1f;
                PlayerDeath(DeathAnimation.none);
            }

            HealthIndicator();

            Invoke("ImmunityFramesGone", .75f);
        }

    }

    public enum DeathAnimation
    {
        none,
        spider
    }

    public void PlayerDeath(DeathAnimation deathAnimation)
    {
        characterController.DisablePlayerInput(true);
        characterController.characterController.height = 0.1f;
        characterLook.canLook = false;

        characterCollider.enabled = false;

        if (deathAnimation == DeathAnimation.none)
        {
            cameraAnimator.enabled = true;
            FMODSoundManager.instance.PlayOneShot(collapseOnGround, transform.position, 0f, 0f);
        }
        else if (deathAnimation == DeathAnimation.spider)
        {
            cameraAnimator.enabled = true;
            cameraAnimator.Play("spiderDeath");
            Invoke("FadeScreen", .3f);
            FMODSoundManager.instance.PlayOneShot(spiderEvent, transform.position, 0f, 0f);
        }

        Invoke("RestartLevel", 3f);
    }

    public void PlayerDeath(DeathAnimation deathAnimation, Transform objectToLookAt, NewSpiderDamager killer)
    {
        characterController.DisablePlayerInput(true);
        characterLook.canLook = false;
        characterLook.transform.LookAt(objectToLookAt);

        characterCollider.enabled = false;

        _currentColors = 1;
        _currentRange = 0;

        _isInDeathAnimation = true;

        deconLight.intensity = 0;
        deconLight.range = 0;
        deconLight.enabled = true;

        killer.GetComponentInParent<NewSpiderBrain>().LookAtThenDisable(virtualCamera.transform, this);
        Invoke("FadeScreen", 3f);
        Invoke("RestartLevel", 5f);
    }

    public void FadeScreen()
    {
        cameraFade.QuickFade();
    }

    public void RestartLevel()
    {
        StartGame startGame = FindFirstObjectByType<StartGame>();
        gameManager.CopyPlayerVariables();
        //Debug.Log($"Has hub: {startGame.hasHub}, hub name:{startGame.hubName}");
        gameManager.RestartLevel(startGame.levelName, startGame.hasHub, startGame.hubName);
    }

    private void Update()
    {
        currentPercent = Mathf.MoveTowards(currentPercent, desiredPercent, Time.deltaTime);
        healthIndicator.fillAmount = currentPercent;

        healAlpha = Mathf.MoveTowards(healAlpha, desiredHealAlpha, Time.deltaTime);
        heal.color = new Color(heal.color.r, heal.color.g, heal.color.b, healAlpha);

        hurtAlpha = Mathf.MoveTowards(hurtAlpha, desiredHurtAlpha, Time.deltaTime);
        hurt.color = new Color(hurt.color.r, hurt.color.g, hurt.color.b, hurtAlpha);

        if (_isInDeathAnimation)
        {
            characterController.playerDead = true;

           _currentStatic = Mathf.MoveTowards(_currentStatic, 200, Time.deltaTime * 100);
            _currentStaticStrenght = Mathf.MoveTowards(_currentStatic, 2, Time.deltaTime * 1f);

            gameManager.GetComponent<Glitchcontroller>().noiseAmount = _currentStatic;
            gameManager.GetComponent<Glitchcontroller>().glitchStrenght = _currentStaticStrenght;

            virtualCamera.m_Lens.FieldOfView = Mathf.MoveTowards(virtualCamera.m_Lens.FieldOfView, 25, Time.deltaTime * 5);
            _timeSinceDeathStarted += Time.deltaTime;

            _currentColors = Mathf.MoveTowards(_currentColors, 0, Time.deltaTime);
            _currentRange = Mathf.MoveTowards(_currentRange, 1, Time.deltaTime);

            deconLight.color = new Color(deconLight.color.r, _currentColors, _currentColors);
            deconLight.intensity = _currentRange;
            deconLight.range = _currentRange;
        }

        if (criticalInjury)
        {
            if (currentHealth > 0)
            {
                _criticalInjuryTimer -= Time.deltaTime;

                if (!_appliedEffect)
                {
                    _appliedEffect = true;
                }

                if (_criticalInjuryTimer <= 0)
                {
                    _criticalInjuryTimer = 1.25f;
                    _shouldShowUp = !_shouldShowUp;

                    criticalInjuryText.SetActive(_shouldShowUp);

                    if (crit2) criticalInjuryText2.SetActive(_shouldShowUp);
                    if (crit3) criticalInjuryText3.SetActive(_shouldShowUp);
                    if (crit4) criticalInjuryText4.SetActive(_shouldShowUp);

                    if (_firstTimeAnimation && !_shouldShowUp)
                    {
                        if (crit4) finalCrit = true;
                        if (crit3) crit4 = true;
                        if (crit2) crit3 = true;
                        if (!crit2)
                        {
                            crit2 = true;
                        }

                        if (crit4 && finalCrit)
                        {
                            _firstTimeAnimation = false;

                            criticalInjuryText2.SetActive(false);
                            criticalInjuryText3.SetActive(false);
                            criticalInjuryText4.SetActive(false);
                            crit2 = false;
                            crit3 = false;
                            crit4 = false;
                            finalCrit = false;

                            //gameManager.GetComponent<Glitchcontroller>().noiseAmount = 0;
                            //gameManager.GetComponent<Glitchcontroller>().glitchStrenght = 0;
                        }
                    }

                }
            }
            else
            {
                criticalInjuryText2.SetActive(true);
            }
            
        }
        else
        {
            _firstTimeAnimation = true;
            _appliedEffect = false;
            criticalInjuryText.SetActive(false);
            criticalInjuryText2.SetActive(false);
            criticalInjuryText3.SetActive(false);
            criticalInjuryText4.SetActive(false);
        }
    }

    private void ImmunityFramesGone()
    {
        _immunityFrames = false;
    }

    private void HealthIndicator()
    {
        if (currentHealth > 99)
        {
            heart1[0].gameObject.SetActive(true);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(true);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(true);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 89)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(true);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(true);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(true);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 79)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(true);

            heart2[0].gameObject.SetActive(true);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(true);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 69)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(true);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(true);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 59)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(false);
            heart2[1].gameObject.SetActive(true);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(true);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 49)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(false);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(true);

            heart3[0].gameObject.SetActive(true);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 39)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(false);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(true);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 29)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(false);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(false);
            heart3[1].gameObject.SetActive(true);
            heart3[2].gameObject.SetActive(false);
        }
        else if (currentHealth > 10)
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(false);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(false);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(true);
        }
        else
        {
            heart1[0].gameObject.SetActive(false);
            heart1[1].gameObject.SetActive(false);
            heart1[2].gameObject.SetActive(false);

            heart2[0].gameObject.SetActive(false);
            heart2[1].gameObject.SetActive(false);
            heart2[2].gameObject.SetActive(false);

            heart3[0].gameObject.SetActive(false);
            heart3[1].gameObject.SetActive(false);
            heart3[2].gameObject.SetActive(false);
        }

        if (currentHealth < 11) criticalInjury = true;
        else criticalInjury = false;
    }
}
