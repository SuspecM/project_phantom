using FMODUnity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [HideInInspector]
    public string settingsPath;
    [HideInInspector]
    public string profilesPath;
    private PersistentGameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<PersistentGameManager>();
        settingsPath = Application.persistentDataPath + "/systemSettings.ghostsave";
        profilesPath = Application.persistentDataPath + "/campaign.ghostsave";
    }

    #region settings
    public void SaveSettings(string reason)
    {
        Debug.Log($"Settings Save initiated - {reason}");
        try
        {
            RuntimeManager.GetBus("bus:/").getVolume(out float master);
            RuntimeManager.GetBus("bus:/Sfx").getVolume(out float sfx);
            RuntimeManager.GetBus("bus:/Music").getVolume(out float music);
            SettingsSerialization settingsToSave = new SettingsSerialization(
            QualitySettings.GetQualityLevel()
            , gameManager.fullScreen
            , Mathf.Clamp(master, 0f, 1f)
            , Mathf.Clamp(music, 0f, 1f)
            , Mathf.Clamp(sfx, 0f, 1f)
            , gameManager.loadedProfile
            , gameManager.verticalSensitivity
            , gameManager.horizontalSensitivity
            );

            File.WriteAllText(settingsPath, settingsToSave.ToString());
            Debug.Log("Settings saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error saving settings: {e.Message}");
        }
    }

    public void LoadSettings(string reason)
    {
        Debug.Log($"Settings Load initiated - {reason}");
        //try
        //{
        if (!File.Exists(settingsPath))
        {
            Debug.Log($"No settings fille found, creating defaults!");
            QualitySettings.SetQualityLevel(2);
            gameManager.qualityLevel = 2;

            Screen.fullScreen = true;
            gameManager.fullScreen = true;

            RuntimeManager.GetBus("bus:/").setVolume(.8f);
            RuntimeManager.GetBus("bus:/Music").setVolume(.8f);
            RuntimeManager.GetBus("bus:/Sfx").setVolume(.8f);

            //gameManager.resolution = Screen.currentResolution;

            gameManager.verticalSensitivity = 35f;
            gameManager.horizontalSensitivity = 25f;

            SaveSettings("System initiated, no default settings were found!");
        }
        else
        {
            Debug.Log("Settings file found, loading settings!");
            StreamReader sr = new StreamReader(settingsPath);
            string[] read;
            sr.ReadLine();

            //Quality setting
            read = sr.ReadLine().Split(':');
            gameManager.qualityLevel = int.Parse(read[1]);
            Debug.Log($"Quality settings: {read[1]}");
            QualitySettings.SetQualityLevel(gameManager.qualityLevel);

            //Is fullscreen
            read = sr.ReadLine().Split(':');
            gameManager.fullScreen = bool.Parse(read[1]);
            Debug.Log($"Fullscreen: {read[1]}");
            Screen.fullScreen = gameManager.fullScreen;

            //Master volume
            read = sr.ReadLine().Split(':');
            RuntimeManager.GetBus("bus:/").setVolume(Mathf.Clamp(float.Parse(read[1]), 0f, 1f));
            Debug.Log($"Master volume: {read[1]}");

            //Music volume
            read = sr.ReadLine().Split(':');
            RuntimeManager.GetBus("bus:/Music").setVolume(Mathf.Clamp(float.Parse(read[1]), 0f, 1f));
            Debug.Log($"Music volume: {read[1]}");

            //Sfx volume
            read = sr.ReadLine().Split(':');
            RuntimeManager.GetBus("bus:/Sfx").setVolume(Mathf.Clamp(float.Parse(read[1]), 0f, 1f));
            Debug.Log($"Sfx volume: {read[1]}");

            //Vertical sensy
            read = sr.ReadLine().Split(':');
            gameManager.verticalSensitivity = int.Parse(read[1]);
            Debug.Log($"Vertical sensitivity: {read[1]}");

            //Horizontal sensy
            read = sr.ReadLine().Split(':');
            gameManager.horizontalSensitivity = int.Parse(read[1]);
            Debug.Log($"Horizontal sensitivity: {read[1]}");

            sr.Close();
        }
        //}
        //catch (Exception e)
        //{
        //    Debug.LogWarning($"Loading failed: {e.Message}");
        //}
    }
    #endregion
    #region profile
    public void SaveProfile(string reason)
    {
        Debug.Log($"Profile Save initiated - {reason}");

        if (File.Exists(profilesPath))
        {
            File.Delete(profilesPath );
        }

        ProfileSerialization profileToSave = new ProfileSerialization();

        File.WriteAllText(profilesPath, profileToSave.ToString());
        Debug.Log($"Profile saved successfully!");
    }

    public void SaveNonDefaultProfile(string reason, string levelName, int checkpoint, int menuBg, int loadedAmmo, int reserveAmmo, int hasSrynge, int srynges, int hasThrowable, int throwableId, int hasRedCard, int hasBluCard, int phxTut, int hndTut)
    {
        Debug.Log($"Profile Save initiated - {reason}");
        try
        {
            ProfileSerialization profileToSave = new ProfileSerialization(levelName, checkpoint, menuBg, loadedAmmo, reserveAmmo, hasSrynge, srynges, hasThrowable, throwableId, hasRedCard, hasBluCard, phxTut, hndTut);

            File.WriteAllText(profilesPath, profileToSave.ToString());
            Debug.Log($"Profile saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error saving profile: {e.Message}");
        }
    }

    public void LoadProfile(string reason)
    {
        Debug.Log($"Profile Load - {reason}");
        if (File.Exists(profilesPath))
        {
            try
            {
                gameManager.hasProfile = true;

                Debug.Log($"Profile file found, loading profile!");
                StreamReader sr = new StreamReader($"{profilesPath}");
                string[] read;
                string line;
                //skip the first two lines of fluff in the text file
                sr.ReadLine();
                sr.ReadLine();
                gameManager.profileData.Clear();

                //current level
                line = sr.ReadLine();
                read = line.Split(':');
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.currentLevel = read[1];
                Debug.Log($"Current level: {gameManager.currentLevel}");

                //checkpoint
                line = sr.ReadLine();
                read = line.Split(':');
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.currentCheckpoint = int.Parse(read[1]);
                Debug.Log($"Current checkpoint: {gameManager.currentCheckpoint}");

                //menu bg
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.mainMenuBg = int.Parse(read[1]);
                Debug.Log($"Current main menu background: {gameManager.mainMenuBg}");

                //loaded ammo
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.loadedAmmo = int.Parse(read[1]);
                Debug.Log($"Currently loaded ammo in gun: {gameManager.loadedAmmo}");

                //reserve ammo
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.reserveAmmo = int.Parse(read[1]);
                Debug.Log($"Current reserve ammo: {gameManager.reserveAmmo}");

                //has srynge
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.hasSrynge = int.Parse(read[1]);
                Debug.Log($"Has srynge gun: {gameManager.hasSrynge}");

                //srynges
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.srynges = int.Parse(read[1]);
                Debug.Log($"Current reserve srynges: {gameManager.srynges}");

                //has throwable
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                if (int.Parse(read[1]) == 1) gameManager.hasThrowable = true;
                else gameManager.hasThrowable = false;
                Debug.Log($"Has throwable in hand: {gameManager.hasThrowable}");

                //throwable id in hand
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.throwableId = int.Parse(read[1]);
                Debug.Log($"Throwable's id: {gameManager.throwableId}");

                //has red keycard
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.hasRedKeycard = int.Parse(read[1]);
                Debug.Log($"Has red keycard: {gameManager.hasRedKeycard}");

                //has blue keycard
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.hasBlueKeycard = int.Parse(read[1]);
                Debug.Log($"Has blue keycard: {gameManager.hasBlueKeycard}");

                //physics tutorial
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.physicsTutorialMessageShown = int.Parse(read[1]);
                Debug.Log($"Physics objects tutorial messages shown: {gameManager.physicsTutorialMessageShown}");

                //hand object tutorial
                line = sr.ReadLine();
                read = line.Split(":");
                gameManager.profileData.Add($"{read[0]}: {read[1]}");
                gameManager.handObjectTutorialMessageShown = int.Parse(read[1]);
                Debug.Log($"Hand object tutorial messages shown: {gameManager.handObjectTutorialMessageShown}");

                sr.Close();

                Debug.Log($"Loading profile successful!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Loading failed: {e.Message}");
            }
        }
        else
        {
            gameManager.currentLevel = "Level 1";
            gameManager.currentCheckpoint = 0;
            gameManager.mainMenuBg = 0;
            gameManager.loadedAmmo = 0;
            gameManager.reserveAmmo = 0;
            gameManager.hasSrynge = 0;
            gameManager.srynges = 0;
            gameManager.hasThrowable = false;
            gameManager.throwableId = 0;
            gameManager.hasRedKeycard = 0;
            gameManager.hasBlueKeycard = 0;
            gameManager.physicsTutorialMessageShown = 0;
            gameManager.handObjectTutorialMessageShown = 0;
        }
        
    }

    public void DeleteProfile(string profile)
    {
        try
        {
            File.Delete($"{profilesPath}/{profile}.ghostsave");
            Debug.Log($"{profile} has been deleted!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Profile deletion failed: {e.Message}");
        }
    }
    #endregion
}

[Serializable]
public class SettingsSerialization
{
    public int QualitySetting { get; set; }
    public bool IsFullscreen { get; set; }
    public float MasterVolume { get; set; }
    public float MusicVolume { get; set; }
    public float SfxVolume { get; set; }
    public Resolution Resolution { get; set; }
    public float verticalSensitivity { get; set; }
    public float horizontalSensitivity { get; set; }

    public SettingsSerialization(int qualitySetting, bool isFullscreen, float masterVolume, float musicVolume, float sfxVolume, int profile, float verticalSensitivity, float horizontalSensitivity)
    {
        this.QualitySetting = qualitySetting;
        this.IsFullscreen = isFullscreen;
        this.MasterVolume = masterVolume;
        this.MusicVolume = musicVolume;
        this.SfxVolume = sfxVolume;
        this.verticalSensitivity = verticalSensitivity;
        this.horizontalSensitivity = horizontalSensitivity;
    }

    public override string ToString()
    {
        return $"If you want to change anything here for any reason make sure there is no space after the ':' (max volume is 1, minimum is 0)" +
            $"\nQuality setting:{QualitySetting}\nIs fullscreen:{IsFullscreen}\nMaster volume:{MasterVolume}" +
            $"\nMusic volume:{MusicVolume}\nSfx volume:{SfxVolume}\nVertical sensitivity:{verticalSensitivity}\nHorizontal sensiticity:{horizontalSensitivity}";
    }
}

[Serializable]
public class ProfileSerialization
{
    public string CurrentLevel { get; set; }
    public int CurrentCheckpoint { get;set; }
    public int MenuBackground { get; set; }
    public int LoadedAmmo { get; set; }
    public int ReserveAmmo { get; set; }
    public int HasSrynge { get; set; }
    public int Srynges { get; set; }
    public int HasThrowable { get; set; }
    public int Throwable { get; set; }
    public int HasRedKeycard { get; set; }
    public int HasBluKeycard { get; set; }
    public int PhysicsTutorialMessageShown { get; set; }
    public int HandObjectTutorialMessageShown { get;set; }

    //for creating a new, default profile
    public ProfileSerialization()
    {
        this.CurrentLevel = "Level 1";
        this.CurrentCheckpoint = 0;
        this.MenuBackground = 0;
        this.LoadedAmmo = 0;
        this.ReserveAmmo = 0;
        this.HasSrynge = 0;
        this.Srynges = 0;
        this.HasThrowable = 0;
        this.Throwable = 0;
        this.HasRedKeycard = 0;
        this.HasBluKeycard = 0;
        this.PhysicsTutorialMessageShown = 0;
        this.HandObjectTutorialMessageShown = 0;
    }

    //for saving an existing save
    public ProfileSerialization(string levelName, int checkpoint, int background, int loadedAmmo, int reserveAmmo, int hasSrynge, int srynges, int hasThrowable, int throwable, int hasRedCard, int hasBluCard, int phxTut, int hndTut)
    {
        this.CurrentLevel = levelName;
        this.CurrentCheckpoint = checkpoint;
        this.MenuBackground = background;
        this.LoadedAmmo = loadedAmmo;
        this.ReserveAmmo = reserveAmmo;
        this.HasSrynge = hasSrynge;
        this.Srynges = srynges;
        this.HasThrowable = hasThrowable;
        this.Throwable = throwable;
        this.HasRedKeycard = hasRedCard;
        this.HasBluKeycard = hasBluCard;
        this.PhysicsTutorialMessageShown = phxTut;
        this.HandObjectTutorialMessageShown = hndTut;
    }

    public override string ToString()
    {
        return $"Everything here has been designed to be changeable with a simple text editor. Take this as an official seal of approval to cheating ;)\n" +
            $"Make sure after the ':' there is no space if you want to change anything here otherwise the game can't read these values and the save is kaput!\n" +
            $"Current level:{CurrentLevel}\n" +
            $"Current checkpoint:{CurrentCheckpoint}\n" +
            $"Background to load:{MenuBackground}\n" +
            $"Loaded ammo:{LoadedAmmo}\n" +
            $"Reserve ammo:{ReserveAmmo}\n" +
            $"Has srynge:{HasSrynge}\n" +
            $"Srynges:{Srynges}\n" +
            $"Has throwable in hand:{HasThrowable}\n" +
            $"Dedicated id of throwable in hand:{Throwable}\n" +
            $"Has red keycard:{HasRedKeycard}\n" +
            $"Has blue keycard:{HasBluKeycard}\n" +
            $"Physics tutorial messages shown:{PhysicsTutorialMessageShown}\n" +
            $"Hand object tutorial messages shown:{HandObjectTutorialMessageShown}"
            ;
    }
}


