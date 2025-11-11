using UnityEngine;

/// <summary>
/// Data structure for a random card effect.
/// </summary>
[System.Serializable]
public class RandomCard
{
    public enum CardType { MoveForward, MoveBackward }

    public CardType type;
    public int steps;           // How many steps to move
    public int gridPosition;    // Which square this card is on

    public RandomCard(CardType type, int steps, int gridPosition)
    {
        this.type = type;
        this.steps = steps;
        this.gridPosition = gridPosition;
    }

    /// <summary>
    /// Get a user-friendly title for this card.
    /// </summary>
    public string GetTitle()
    {
        return type == CardType.MoveForward ? "Move Forward!" : "Move Backward!";
    }

    /// <summary>
    /// Get a description of what this card does.
    /// </summary>
    public string GetDescription()
    {
        string direction = type == CardType.MoveForward ? "forward" : "backward";
        return $"Move {steps} step{(steps > 1 ? "s" : "")} {direction}";
    }
}
