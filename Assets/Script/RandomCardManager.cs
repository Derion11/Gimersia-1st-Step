using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages random card spawning, tracking, and activation.
/// </summary>
public class RandomCardManager : MonoBehaviour
{
    [Header("Card Settings")]
    [Tooltip("How many cards to spawn on the board")]
    public int numberOfCards = 15;

    [Tooltip("Minimum movement steps")]
    public int minSteps = 1;

    [Tooltip("Maximum movement steps")]
    public int maxSteps = 10;

    [Header("Visuals")]
    [Tooltip("Prefab to show where cards are located on the board")]
    public GameObject cardVisualPrefab;

    [Header("References")]
    public gridBoard board;
    public RandomCardUI cardUI;

    [Header("Card Appearance (Unified)")]
    [Tooltip("Color for all mystery cards (since effect is hidden)")]
    public Color cardColor = Color.yellow;

    // Internal tracking
    private List<RandomCard> activeCards = new List<RandomCard>();
    private Dictionary<int, GameObject> cardVisuals = new Dictionary<int, GameObject>();

    void Start()
    {
        if (board == null)
        {
            Debug.LogError("RandomCardManager: 'board' reference missing!");
            return;
        }

        if (cardUI == null)
        {
            Debug.LogError("RandomCardManager: 'cardUI' reference missing!");
            return;
        }

        // Cards will be spawned by gridBoard.Start() after board is ready
        Debug.Log("RandomCardManager: Initialized and waiting for board to call SpawnRandomCards()");
    }

    /// <summary>
    /// Spawns random cards at random positions on the board.
    /// </summary>
    public void SpawnRandomCards()
    {
        if (board == null)
        {
            Debug.LogError("RandomCardManager: Board reference is null!");
            return;
        }

        if (board.posisiKotak == null || board.posisiKotak.Count == 0)
        {
            Debug.LogWarning($"RandomCardManager: Board not ready! posisiKotak count = {board.posisiKotak?.Count ?? 0}");
            return;
        }

        Debug.Log($"RandomCardManager: Starting to spawn {numberOfCards} cards on board with {board.posisiKotak.Count} squares");

        // Clear any existing cards
        ClearAllCards();

        // Generate available positions (avoid first and last squares)
        List<int> availablePositions = new List<int>();
        for (int i = 1; i < board.LastIndex; i++)
        {
            availablePositions.Add(i);
        }

        // Spawn cards
        int cardsToSpawn = Mathf.Min(numberOfCards, availablePositions.Count);
        for (int i = 0; i < cardsToSpawn; i++)
        {
            // Pick random position
            int randomIndex = Random.Range(0, availablePositions.Count);
            int gridPos = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex); // Prevent duplicates

            // Random card type (50/50 chance)
            RandomCard.CardType type = Random.value >= 0.5f
                ? RandomCard.CardType.MoveForward
                : RandomCard.CardType.MoveBackward;

            // Random steps
            int steps = Random.Range(minSteps, maxSteps + 1);

            // Create card (effect is hidden until activated!)
            RandomCard card = new RandomCard(type, steps, gridPos);
            activeCards.Add(card);

            // Spawn visual indicator (all cards look the same - mystery!)
            if (cardVisualPrefab != null)
            {
                Vector3 position = board.GetPosisiKotak(gridPos);
                GameObject visual = Instantiate(cardVisualPrefab, position, Quaternion.identity, board.transform);
                
                // Scale down to fit the grid square
                visual.transform.localScale = Vector3.one * (board.ukuranKotak * 0.6f); // 60% of grid size
                
                // All cards use the same color (mystery effect)
                SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = cardColor; // Same color for all cards
                    sr.sortingOrder = 50; // Above tiles, below pawns
                }

                // Add floating animation (like Mario Bros coins!)
                FloatingAnimation floater = visual.AddComponent<FloatingAnimation>();
                floater.floatAmplitude = board.ukuranKotak * 0.15f; // Float 15% of tile size
                floater.floatSpeed = 2f; // 2 cycles per second
                floater.enableRotation = false; // Set to true if you want spinning cards

                cardVisuals[gridPos] = visual;
            }

            Debug.Log($"Spawned mystery card at position {gridPos}: {type} {steps} steps (hidden)");
        }
    }

    /// <summary>
    /// Get the card at a specific grid position (if any).
    /// </summary>
    public RandomCard GetCardAtPosition(int gridIndex)
    {
        return activeCards.Find(card => card.gridPosition == gridIndex);
    }

    /// <summary>
    /// Called when a pawn lands on a card.
    /// </summary>
    public void OnCardActivated(Pawn pawn, RandomCard card)
    {
        if (cardUI == null) return;

        // Show the UI and let the player decide
        cardUI.ShowCard(pawn, card, this);
    }

    /// <summary>
    /// Execute the card effect (move the pawn).
    /// </summary>
    public void ExecuteCardEffect(Pawn pawn, RandomCard card)
    {
        if (pawn == null || card == null) return;

        int movement = card.type == RandomCard.CardType.MoveForward ? card.steps : -card.steps;
        
        Debug.Log($"Executing card: {card.type} {card.steps} steps (current pos: {pawn.nomorSaatIni})");

        // Remove the card first
        RemoveCard(card.gridPosition);

        // Release the game lock temporarily so movement can happen
        if (board != null)
        {
            board.SedangGerak = false;
        }

        // Move the pawn (fromCard=true prevents checking for another card)
        pawn.JalanBeberapaLangkah(movement, fromCard: true);
    }

    /// <summary>
    /// Remove a card from the board.
    /// </summary>
    public void RemoveCard(int gridIndex)
    {
        // Remove from active list
        activeCards.RemoveAll(card => card.gridPosition == gridIndex);

        // Destroy visual
        if (cardVisuals.ContainsKey(gridIndex))
        {
            Destroy(cardVisuals[gridIndex]);
            cardVisuals.Remove(gridIndex);
        }

        Debug.Log($"Removed card at position {gridIndex}");
    }

    /// <summary>
    /// Clear all cards from the board.
    /// </summary>
    public void ClearAllCards()
    {
        activeCards.Clear();

        foreach (var visual in cardVisuals.Values)
        {
            if (visual != null) Destroy(visual);
        }
        cardVisuals.Clear();
    }

    /// <summary>
    /// Resume game after card decision (accept or cancel).
    /// </summary>
    public void ResumeGame()
    {
        if (board != null)
        {
            board.SedangGerak = false;

            // Switch turns
            if (board.giliranPlayer == 0)
                board.giliranPlayer = 1;
            else
                board.giliranPlayer = 0;
        }
    }
}
