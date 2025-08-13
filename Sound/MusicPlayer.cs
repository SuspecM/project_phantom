using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public EventReference music;
    public EventInstance playingMusic;
    public bool playMusic;

    public bool increaseTension;
    public string tensionVarName;
    public float startingValue;
    public float currentValue;
    public float tensionIncreasePerFrame;
    public float increasedTensionPerFrame;
    public float moreIncreasedTensionPerFrame;

    public bool alreadyPlaying;

    private bool _fadingMusicChange;
    private float _currentFade = 1;
    private EventReference _fadingMusic;
    private bool _swappedFadingChange;

    private void Start()
    {
        StartMusic();
        currentValue = startingValue;
    }

    public void InitialiseMusicPlayer(EventReference music, bool playMusic, bool increaseTension, string tensionVarName, float startingValue, float currentValue, float tensionIncreasePerFrame, bool alreadyPlaying)
    {
        this.music = music;
        this.playMusic = playMusic;
        this.increaseTension = increaseTension;
        this.currentValue = currentValue;
        this.tensionVarName = tensionVarName;
        this.startingValue = startingValue;
        this.currentValue = currentValue;
        this.tensionIncreasePerFrame = tensionIncreasePerFrame;
        this.alreadyPlaying = alreadyPlaying;

        StartMusic();
    }

    public void StartMusic()
    {
        if (playMusic && !alreadyPlaying)
        {
            playingMusic = RuntimeManager.CreateInstance(music);
            playingMusic.start();
            alreadyPlaying = true;
        }
    }

    private void Update()
    {
        if (_fadingMusicChange)
        {
            _currentFade -= Time.deltaTime / 5;
            playingMusic.setParameterByName("fade", _currentFade);

            if (_currentFade <= 0)
            {
                playingMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                playingMusic.release();
                playingMusic = RuntimeManager.CreateInstance(_fadingMusic);
                playingMusic.start();
                _fadingMusicChange = false;
                playingMusic.setParameterByName("fade", 0f);
                _swappedFadingChange = true;
            }
        }

        if (_swappedFadingChange)
        {
            _currentFade += Time.deltaTime / 5;
            playingMusic.setParameterByName("fade", _currentFade);

            if (_currentFade >= 1)
            {
                _swappedFadingChange = false;
            }
        }
    }

    public void StopMusic(bool fadeout)
    {
        if (fadeout)
            playingMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        else
            playingMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        playingMusic.release();
    }

    public void SwapMusic(EventReference newMusic, bool fadeout)
    {
        if (fadeout)
        {
            _currentFade = 1f;
            _fadingMusicChange = true;
            _fadingMusic = newMusic;
        }
        else
        {
            playingMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            playingMusic.release();

            playingMusic = RuntimeManager.CreateInstance(newMusic);
            playingMusic.start();
        }   

    }

    [NaughtyAttributes.ReadOnly]
    public bool stopCarnivalChange = true;
    [NaughtyAttributes.ReadOnly]
    public float excitementValue = 0f;
    [NaughtyAttributes.ReadOnly]
    public float woodwindValue = 0f;

    [NaughtyAttributes.ReadOnly]
    public bool woodwinding;
    [NaughtyAttributes.ReadOnly]
    public bool exciting;

    public void ChangeCarnivalMood(bool woodwinds, bool excitement)
    {
        if (woodwinds)
        {
            woodwinding = true;
            stopCarnivalChange = false;
        }
        else
        {
            woodwinding = false;
        }

        if (excitement)
        {
            exciting = true;
            stopCarnivalChange = false;
        }
        else
        {
            exciting = false;
        }
    }

    private void FixedUpdate()
    {
        if (!stopCarnivalChange)
        {
            if (woodwinding)
            {
                woodwindValue = Mathf.Clamp(woodwindValue + .05f, 0, 1);
                playingMusic.setParameterByName("woodwinds", woodwindValue);
            }
            else
            {
                woodwindValue = Mathf.Clamp(woodwindValue - .05f, 0, 1);
                playingMusic.setParameterByName("woodwinds", woodwindValue);
            }

            if (exciting)
            {
                excitementValue = Mathf.Clamp(excitementValue + .05f, 0, 1);
                playingMusic.setParameterByName("excitement", excitementValue);
            }
            else
            {
                excitementValue = Mathf.Clamp(excitementValue - .05f, 0, 1);
                playingMusic.setParameterByName("excitement", excitementValue);
            }

            if (woodwindValue == 0 && excitementValue == 0)
            {
                stopCarnivalChange = true;
            }
        }

        if (increaseTension)
        {
            if (currentValue < .5)
                currentValue = Mathf.Clamp(currentValue + moreIncreasedTensionPerFrame, 0, 1);
            else if (currentValue < .2)
             currentValue = Mathf.Clamp(currentValue + increasedTensionPerFrame, 0, 1);
            else
             currentValue = Mathf.Clamp(currentValue + tensionIncreasePerFrame, 0, 1);

             playingMusic.setParameterByName(tensionVarName, currentValue);
        }
    }

    public void PlayMusic(bool ye)
    {
        playMusic = ye;
    }

    private void OnDestroy()
    {
        playingMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        playingMusic.release();
    }
}
