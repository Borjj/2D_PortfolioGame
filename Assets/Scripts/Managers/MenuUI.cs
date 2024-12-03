using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header ("Animations")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject player;
    
    private bool isPaused = false;
    private MenuManager menuManager;

// ---------------------------------------------------------------------------------------- //
    private void Start()
    {
        menuManager = GetComponent<MenuManager>();
        
        // Show appropriate menu based on scene
        bool isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
        mainMenuPanel.SetActive(isMainMenu);
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);

        // Setup enemy animation if it exists
        if (enemy != null)
        {
            Animator enemyAnimator = enemy.GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                enemyAnimator.Play("Idle");
            }
        }

        // Setup player animation if it exists
        if (player != null)
        {
            Animator playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.Play("Idle");
            }
        }

        // Set time scale and cursor state
        Time.timeScale = isMainMenu ? 0f : 1f;
        if (isMainMenu)
        {
            menuManager.UnlockCursor();
        }
    }

    private void Update()
    {
        // Only handle pause in game scene
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

// ---------------------------------------------------------------------------------------- //

    // Main Menu Functions
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level_001"); // Your game scene name
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);

    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Pause Menu Functions
    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            menuManager.UnlockCursor();
        }
        else
        {
            menuManager.LockCursor();
            optionsPanel.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Options Menu
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        
        // Return to appropriate menu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mainMenuPanel.SetActive(true);
        }
        else
        {
            pauseMenuPanel.SetActive(true);
        }
    }
}