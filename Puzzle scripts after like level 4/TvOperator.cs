using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;

public class TvOperator : MonoBehaviour
{
    public Animator animator;
    public bool isOn;
    public GameObject[] objectsToActivate;
    public GameObject loadingObject;
    public float activationDelay;

    public TvInput tvInput;

    public TextMeshPro questionText;
    public TextMeshPro option1Text;
    public TextMeshPro options2Text;

    public TextMeshPro endText;

    public EventReference onOffSound;
    public EventReference onNoise;
    public EventInstance playingOnNoise;

    [TextArea(2, 5)]
    public string[] phaseQuestions;
    public string[] firstOptions;
    public string[] secondOptions;

    public GameObject button;

    public int currentPhase;

    private void Start()
    {
        playingOnNoise = RuntimeManager.CreateInstance(onNoise);

        if (!isOn)
        {
            animator.Play("tv close");
            playingOnNoise.setPaused(true);
            foreach (var item in objectsToActivate)
            {
                item.SetActive(false);
            }
        }
        else
        {
            animator.Play("tv open");

            foreach (var item in objectsToActivate)
            {
                item.SetActive(true);
            }
        }
    }

    public void Turn(bool on)
    {
        FMODSoundManager.instance.PlayOneShot(onOffSound, transform.position, 0f, 0f);
        if (on)
        {
            animator.Play("tv open");
        }
        else
        {
            animator.Play("tv close");
        }

        isOn = on;

        Invoke("DelayedObjects", activationDelay);
    }

    public void DelayedObjects()
    {
        if (currentPhase < 3)
        {
            foreach (var item in objectsToActivate)
            {
                item.SetActive(isOn);
            }

            if (isOn)
                playingOnNoise.setPaused(false);
            else
                playingOnNoise.setPaused(true);

            PhaseText();
        }
        
    }

    public void PhaseText()
    {
        loadingObject.SetActive(false);

        button.SetActive(true);

        foreach (var item in objectsToActivate)
        {
            item.SetActive(true);
        }

        questionText.text = phaseQuestions[currentPhase];
        option1Text.text = firstOptions[currentPhase];
        options2Text.text = secondOptions[currentPhase];
    }

    public void NextPhase()
    {
        Debug.Log("God save you if you are trying to read this code because this script was meant to only handle a single question but I had to make it more interesting and stuff. This shit is on the same level as trying to break a door with two sticks duct taped toghether.");
        if (currentPhase < 3)
        {
            foreach (var item in objectsToActivate)
            {
                item.SetActive(false);
            }

            objectsToActivate[1].SetActive(true);

            button.SetActive(false);

            loadingObject.SetActive(true);

            currentPhase++;
            Invoke("PhaseText", 3f);
        }
        else
        {
            foreach (var item in objectsToActivate)
            {
                item.SetActive(false);
            }

            objectsToActivate[1].SetActive(true);

            button.SetActive(false);

            loadingObject.SetActive(true);

            endText.text = "Don't worry about it, Charlie is already in the facility ;)";

            Invoke("DelayedOuf", 3f);
        }


    }

    public void DelayedOuf()
    {
        foreach (var item in objectsToActivate)
        {
            item.SetActive(false);
        }

        tvInput.DelayedOff();
    }

}
