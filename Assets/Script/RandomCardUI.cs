using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI display and player interaction for random cards.
/// </summary>
public class RandomCardUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The main popup panel")]
    public GameObject cardPanel;

    [Tooltip("Title text (e.g., 'Move Forward!')")]
    public Text cardTitle;

    [Tooltip("Description text (e.g., 'Move 5 steps forward')")]
    public Text cardDescription;

    [Tooltip("Accept button")]
    public Button acceptButton;

    [Tooltip("Cancel button")]
    public Button cancelButton;

    // Current state
    private Pawn currentPawn;
    private RandomCard currentCard;
    private RandomCardManager cardManager;

    void Start()
    {
        // Set up button listeners
        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnAccept);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancel);

        // Hide panel initially
        HideCard();
    }

    /// <summary>
    /// Show the card UI to the player.
    /// </summary>
    public void ShowCard(Pawn pawn, RandomCard card, RandomCardManager manager)
    {
        currentPawn = pawn;
        currentCard = card;
        cardManager = manager;

        // Update UI elements
        if (cardTitle != null)
            cardTitle.text = card.GetTitle();

        if (cardDescription != null)
            cardDescription.text = card.GetDescription();

        // Show the panel
        if (cardPanel != null)
            cardPanel.SetActive(true);

        // Game is already paused via SedangGerak flag, no need for Time.timeScale

        Debug.Log($"Showing card: {card.GetTitle()} - {card.GetDescription()}");
    }

    /// <summary>
    /// Called when player clicks Accept button.
    /// </summary>
    public void OnAccept()
    {
        if (AudioManaging.Instance != null) AudioManaging.Instance.PlaySFX("button_push");
        
        if (currentCard == null || currentPawn == null || cardManager == null) return;

        Debug.Log("Player accepted card effect");

        // Hide UI
        HideCard();

        // Execute the card effect (this will handle resuming the game)
        cardManager.ExecuteCardEffect(currentPawn, currentCard);

        // Clear current state
        currentCard = null;
        currentPawn = null;
    }

    /// <summary>
    /// Called when player clicks Cancel button.
    /// </summary>
    public void OnCancel()
    {
        Debug.Log("Player cancelled card effect");
        if (AudioManaging.Instance != null) AudioManaging.Instance.PlaySFX("button_push");
        // Hide UI
        HideCard();

        // Just resume the game without applying effect
        if (cardManager != null)
            cardManager.ResumeGame();

        // Clear current state
        currentCard = null;
        currentPawn = null;
    }

    /// <summary>
    /// Hide the card panel.
    /// </summary>
    private void HideCard()
    {
        if (cardPanel != null)
            cardPanel.SetActive(false);
    }
}
