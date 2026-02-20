using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class GameMenu : MonoBehaviour
{
    public GameObject failmenu;
    public GameObject pausemenu;
    public GameObject optionsmenu;
    public float menumode;

    public PlayerMovement player;

    public Button pauseMenuDefault;
    public Slider optionsMenuDefault;

    public Slider sfxSlider;
    public Slider musicSlider;

    public AudioMixer audioMixer; // Reference to the Audio Mixer

    public VolumeProfile gameVolumeProfile;
    public VolumeProfile uiVolumeProfile;

    public Volume volume;

    // PlayerPrefs keys for storing volume settings
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Start() {
        volume.profile = gameVolumeProfile;
        menumode = 1;
        // Load and set the volume settings from PlayerPrefs
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.75f);
    }
    void Update() 
    {
        if(Input.GetButtonDown("Cancel")) 
        {
            if(menumode == 1)//this is game menu
            {
                PauseMenu();
            }
            else if(menumode == 2)//this is pause menu
            {
                player.stopInput = false;
                volume.profile = gameVolumeProfile;
                menumode = 1;
                pausemenu.SetActive(false);
                Time.timeScale = 1f;
            }
            else if(menumode == 3)//this is options menu
            {
                PauseMenu();
            }
        }
    }

    public void PauseMenu()
    {
        player.stopInput = true;
        volume.profile = uiVolumeProfile;
        menumode = 2;
        Time.timeScale = 0f;
        optionsmenu.SetActive(false);
        pausemenu.SetActive(true);
        pauseMenuDefault.Select();
    }

    public void OptionsMenu()
    {
        volume.profile = uiVolumeProfile;
        menumode = 3;
        Time.timeScale = 0f;
        optionsmenu.SetActive(true);
        pausemenu.SetActive(false);
        optionsMenuDefault.Select();
    }

    public void Restart()
    {
        player.stopInput = false;
        volume.profile = gameVolumeProfile;
        menumode = 1;
        failmenu.SetActive(false);
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }

    public void Menu()
    {   
        player.stopInput = false;
        menumode = 1;
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void SetSFXVolume(float volume)
    {
        // Set the SFX volume in the Audio Mixer
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20); 

        // Save the SFX volume setting to PlayerPrefs
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        // Set the music volume in the Audio Mixer
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20); 

        // Save the music volume setting to PlayerPrefs
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}
