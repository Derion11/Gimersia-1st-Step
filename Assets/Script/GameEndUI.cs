using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The main panel containing all UI elements")]
    public GameObject endGamePanel;
    
    [Tooltip("Text showing which player won (e.g., 'Player 1 Wins!')")]
    public Text winnerText;
    
    [Tooltip("Optional: Display winning player's sprite/icon")]
    public Image winnerIcon;
    
    [Header("Optional Buttons")]
    [Tooltip("Button to restart the current game")]
    public Button restartButton;
    
    [Tooltip("Button to return to main menu")]
    public Button mainMenuButton;
    
    [Tooltip("Button to quit the game")]
    public Button quitButton;

    [Header("Audio Settings")]
    [Tooltip("Sound effect to play when game ends")]
    public string victorySFX = "victory";

    [Header("Animation Settings")]
    [Tooltip("Scale animation duration")]
    public float animationDuration = 0.5f;

    private void Start()
    {
        // Hide the panel at start
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    /// <summary>
    /// Shows the end game UI with the winner's information
    /// </summary>
    /// <param name="playerNumber">The player number that won (1 or 2)</param>
    /// <param name="playerSprite">Optional sprite of the winning player</param>
    public void ShowWinner(int playerNumber, Sprite playerSprite = null)
    {
        if (endGamePanel == null)
        {
            Debug.LogError("GameEndUI: endGamePanel is not assigned!");
            return;
        }

        // Set winner text
        if (winnerText != null)
        {
            winnerText.text = $"Player {playerNumber} Wins!";
        }

        // Set winner icon if available
        if (winnerIcon != null && playerSprite != null)
        {
            winnerIcon.sprite = playerSprite;
            winnerIcon.enabled = true;
        }
        else if (winnerIcon != null)
        {
            winnerIcon.enabled = false;
        }

        // Play victory sound
        if (AudioManaging.Instance != null && !string.IsNullOrEmpty(victorySFX))
        {
            AudioManaging.Instance.PlaySFX(victorySFX);
        }

        // Show the panel with animation
        endGamePanel.SetActive(true);
        StartCoroutine(AnimatePanel());
    }

    /// <summary>
    /// Animates the panel appearing with a scale effect
    /// </summary>
    private System.Collections.IEnumerator AnimatePanel()
    {
        endGamePanel.transform.localScale = Vector3.zero;
        
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Smooth scale up with slight overshoot
            float scale = Mathf.Lerp(0f, 1.1f, t);
            if (t > 0.8f)
            {
                scale = Mathf.Lerp(1.1f, 1f, (t - 0.8f) / 0.2f);
            }
            
            endGamePanel.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        
        endGamePanel.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Restarts the current game scene
    /// </summary>
    public void RestartGame()
    {
        if (AudioManaging.Instance != null) AudioManaging.Instance.PlaySFX("button_push");
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    public void LoadMainMenu()
    {
        if (AudioManaging.Instance != null) AudioManaging.Instance.PlaySFX("button_push");
        // Hentikan BGM game sebelum kembali ke menu
        if (AudioManaging.Instance != null)
        // Load the MainMenu scene
        // Make sure your MainMenu scene is added to Build Settings!
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the game application
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Hides the end game panel (in case you need to reset)
    /// </summary>
    public void Hide()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }
}
