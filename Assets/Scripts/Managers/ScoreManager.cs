using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [Header("Score Settings")]
    [SerializeField] private int scorePerCoin = 1;
    
    private int currentScore = 0;

// -------------------------------------------------------------------------------------- //

    // Singleton pattern to access ScoreManager from other scripts
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

// --------------------------------------------------------------------------- //

    public void AddCoin()
    {
        currentScore += scorePerCoin;
        UpdateScoreUI();
    }

    public void RemoveCoins(int amount)
    {
        currentScore = Mathf.Max(0, currentScore - amount); // Prevent negative score
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    // Get current score (useful for other systems that might need it)
    public int GetScore()
    {
        return currentScore;
    }

    // Reset score (useful for game over or new game)
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }
}