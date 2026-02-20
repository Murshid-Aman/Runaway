using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Android;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;

    public Button mainMenuDefault;
    public Slider optionsMenuDefault;

    public Slider sfxSlider;
    public Slider musicSlider;

    public TMP_Text scoreText;
    private int score = 0;
    private string scoreKey = "PlayerScore";

    public AudioMixer audioMixer; // Reference to the Audio Mixer
    public Toggle customAudioToggle; // Reference to the Toggle UI element
    private float menumode = 1;

    // PlayerPrefs keys for storing volume settings
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Load the player's score from PlayerPrefs
        score = PlayerPrefs.GetInt(scoreKey);

        ShowMainMenu();
        // Check if the WRITE_EXTERNAL_STORAGE permission is granted
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            // If not granted, request the permission
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        
        // Set the main menu as active and the options menu as inactive initially
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);

        // If it's the first time the game is launched (no PlayerPrefs key), set playCustomAudio to false (0)
        if (!PlayerPrefs.HasKey("playCustomAudio"))
        {
            PlayerPrefs.SetInt("playCustomAudio", 0);
            PlayerPrefs.Save();
        }

        // Load the PlayerPrefs value for playCustomAudio and set the Toggle accordingly
        customAudioToggle.isOn = PlayerPrefs.GetInt("playCustomAudio", 1) == 1;

        // Load and set the volume settings from PlayerPrefs
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.75f);
    }

    void Update() 
    {
        scoreText.text = score.ToString();
        
        if(Input.GetButtonDown("Cancel"))
        {
            if(menumode == 1)//this is main menu
            {
                Application.Quit();
            }
            else if(menumode == 2)//this is options menu
            {
                ShowMainMenu();
            }
        }
    }

    public void ToggleCustomAudio()
    {
        // When the toggle state changes, update the PlayerPrefs value accordingly
        PlayerPrefs.SetInt("playCustomAudio", customAudioToggle.isOn ? 1 : 0);
        PlayerPrefs.Save(); // Save PlayerPrefs to make the changes permanent
    }

    public void ShowMainMenu()
    {
        menumode = 1;
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        mainMenuDefault.Select();
    }

    public void ShowOptionsMenu()
    {
        menumode = 2;
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        optionsMenuDefault.Select();
    }
    
    public void StartGame()
    {
        menumode = 1;
        SceneManager.LoadScene(1);
    }

    public void Quit() 
    {
        Application.Quit();
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
