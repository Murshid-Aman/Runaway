using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class MP3Player : MonoBehaviour
{
    private string musicFolderPath = "/storage/emulated/0/GameMusic";
    public AudioSource audioSource;
    public AudioClip fallbackClip; // Reference to the fallback audio clip
    private bool notMainMenuScene;

    // Singleton pattern
    public static MP3Player instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Check if the current scene is the main menu
        notMainMenuScene = SceneManager.GetActiveScene().buildIndex == 1;

        // If playCustomAudio is true (1), do something
        if (PlayerPrefs.GetInt("playCustomAudio", 1) == 1)
        {
            StartCoroutine(PlayRandomMP3());
        }
        else // If playCustomAudio is false (0), do something else
        {
            if (fallbackClip != null)
            {
                // Play the fallback audio clip
                audioSource.clip = fallbackClip;
            }
            else
            {
                Debug.LogError("Fallback audio clip is not assigned.");
            }
        }

        // Play background music if not in the main menu
        if (notMainMenuScene)
        {
            PlayBackgroundMusic();
        }
    }

    IEnumerator PlayRandomMP3()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Adjust the delay between each play

            // Check if the specified folder exists
            if (Directory.Exists(musicFolderPath))
            {
                // Get all MP3 files in the folder
                string[] mp3Files = Directory.GetFiles(musicFolderPath, "*.mp3");

                if (mp3Files.Length > 0)
                {
                    Debug.Log(mp3Files.Length + " Mp3 Files Found");
                    // At least one MP3 file found, choose one randomly or implement your selection logic
                    int randomIndex = Random.Range(0, mp3Files.Length);
                    string chosenMP3FilePath = mp3Files[randomIndex];

                    // Load and play the chosen MP3 file
                    yield return StartCoroutine(LoadAudio(chosenMP3FilePath));
                }
                else
                {
                    audioSource.clip = fallbackClip;
                    Debug.Log("No MP3 files found in the specified folder.");
                }
            }
            else
            {
                audioSource.clip = fallbackClip;
                Debug.Log("The specified folder does not exist.");
            }
        }
    }

    // Coroutine to load audio asynchronously
    IEnumerator LoadAudio(string filePath)
    {
        // Load the audio file from the specified file path using UnityWebRequest
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            // Request and wait for the desired file to download
            yield return www.SendWebRequest();

            // Check if there was an error downloading the audio file
            if (www.isNetworkError || www.isHttpError)
            {
                audioSource.clip = fallbackClip;
                Debug.LogError("Error loading audio file: " + www.error);
                yield break;
            }

            // Convert the downloaded data to audio clip
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

            // Assign the loaded audio clip to the audio source
            audioSource.clip = audioClip;

            // Wait until the clip finishes playing
            yield return new WaitForSeconds(audioClip.length);
        }
    }

    private void OnEnable()
    {
        // Listen to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from scene change events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) // MainMenuScene
        {
            StopBackgroundMusic();
        }
        else if (scene.buildIndex == 1) // GameScene or any other scene
        {
            PlayBackgroundMusic();
        }
    }

    private void PlayBackgroundMusic()
    {
        // Check if background music is set
        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopBackgroundMusic()
    {
        audioSource.Stop();
    }
}
