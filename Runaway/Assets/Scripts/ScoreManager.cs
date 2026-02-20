using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText; // Reference to the UI Text component

    private int score = 0;
    private string scoreKey = "PlayerScore";
    public float multiply = 10f;
    private float timer = 0f;
    public bool stopCount = false;
    private bool multicmp = false;//indicating completion of incremented ultiplication of score in the start
    public PlayerMovement player;
    public TMP_Text failScore;
    private bool stopoverCount = false;
    public float animationDuration = 3f;
    private float currentScore = 0;


    // Start is called before the first frame update
    void Start()
    {
        // Load the player's score from PlayerPrefs
        score = PlayerPrefs.GetInt(scoreKey, 0);
        UpdateScoreUI(); // Update the UI text with the loaded score
    }

    // Update is called once per frame
    void Update()
    {
        if (player.Dead) 
        {
            if (!stopoverCount)
            {
                currentScore += Time.deltaTime * score / animationDuration;
            }
            if (currentScore >= score) 
            {
                currentScore = score;
                stopoverCount = true;
            }
            failScore.text = Mathf.RoundToInt(currentScore).ToString();
        }
        if (!multicmp) 
        {
            timer += Time.deltaTime;
        }
        if (timer >= multiply)
        {
            timer = multiply;
            multicmp = true;
        }
        // Calculate the score based on player's x-position
        float playerXPosition = transform.position.x;
        if (!stopCount) 
        {
            score = Mathf.RoundToInt(playerXPosition * timer); // Calculate score
        }
        UpdateScoreUI(); // Update the UI text with the new score
    }

    // Update the UI text with the current score
    void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }

    // Save the score to PlayerPrefs
    public void SaveScore()
    {
        PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.Save();
    }
}
