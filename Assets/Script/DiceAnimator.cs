using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles dice roll animation with rapid image changes before showing final result.
/// Can be used on Canvas (UI) or World Space (SpriteRenderer).
/// </summary>
public class DiceAnimator : MonoBehaviour
{
    [Header("Dice Face Images (1-6)")]
    [Tooltip("Drag your 6 dice sprites here, in order from 1 to 6")]
    public Sprite[] diceFaces = new Sprite[6];

    [Header("Display Component (Choose ONE)")]
    [Tooltip("For UI Canvas - drag the Image component")]
    public Image uiImage;
    
    [Tooltip("For World Space - drag the SpriteRenderer component")]
    public SpriteRenderer spriteRenderer;

    [Header("Animation Settings")]
    [Tooltip("How long the dice animation plays (seconds)")]
    public float animationDuration = 1.0f;
    
    [Tooltip("How fast dice faces flash (seconds between changes)")]
    public float flashSpeed = 0.05f;
    
    [Tooltip("Play sound effect during animation")]
    public bool playSoundEffect = true;

    [Header("Optional: Scale Animation")]
    [Tooltip("Enable dice to grow/shrink during animation")]
    public bool useScaleAnimation = true;
    
    [Tooltip("Scale multiplier during animation")]
    public float scaleMultiplier = 1.2f;

    private Vector3 originalScale;
    private bool isAnimating = false;

    void Start()
    {
        // Validate setup
        if (diceFaces.Length != 6)
        {
            Debug.LogError("DiceAnimator: You need exactly 6 dice face sprites!");
        }

        if (uiImage == null && spriteRenderer == null)
        {
            Debug.LogError("DiceAnimator: You must assign either uiImage OR spriteRenderer!");
        }

        // Store original scale
        originalScale = transform.localScale;

        // Don't hide - keep GameObject active but make it invisible
        SetVisibility(false);
    }

    /// <summary>
    /// Control visibility without deactivating GameObject.
    /// </summary>
    private void SetVisibility(bool visible)
    {
        if (uiImage != null)
        {
            uiImage.enabled = visible;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }
    }

    /// <summary>
    /// Play dice animation and show the final result.
    /// </summary>
    /// <param name="finalValue">The dice value to show at the end (1-6)</param>
    /// <param name="onComplete">Callback when animation finishes</param>
    public void RollDice(int finalValue, System.Action onComplete = null)
    {
        Debug.Log($"DiceAnimator: RollDice called with value {finalValue}");

        if (isAnimating)
        {
            Debug.LogWarning("DiceAnimator: Animation already in progress!");
            return;
        }

        if (finalValue < 1 || finalValue > 6)
        {
            Debug.LogError($"DiceAnimator: Invalid dice value {finalValue}! Must be 1-6.");
            return;
        }

        Debug.Log("DiceAnimator: Starting animation coroutine...");
        StartCoroutine(PlayRollAnimation(finalValue, onComplete));
    }

    /// <summary>
    /// Coroutine that handles the dice roll animation.
    /// </summary>
    private IEnumerator PlayRollAnimation(int finalValue, System.Action onComplete)
    {
        Debug.Log("DiceAnimator: PlayRollAnimation started!");
        isAnimating = true;
        ShowDice();

        // Play sound effect
        if (playSoundEffect && AudioManaging.Instance != null)
        {
            AudioManaging.Instance.PlaySFX("dice_roll"); // You'll need to add this sound
        }

        float elapsed = 0f;
        Vector3 targetScale = originalScale * scaleMultiplier;

        Debug.Log($"DiceAnimator: Starting flash phase for {animationDuration} seconds");

        // Phase 1: Rapid random flashing
        while (elapsed < animationDuration)
        {
            // Show random dice face
            int randomFace = Random.Range(0, 6);
            SetDiceFace(randomFace);

            // Optional: Scale animation (pulse effect)
            if (useScaleAnimation)
            {
                float t = Mathf.PingPong(elapsed / animationDuration * 2f, 1f);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            }

            // Wait before next flash
            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed;
        }

        // Phase 2: Show final result
        SetDiceFace(finalValue - 1); // Convert 1-6 to 0-5 index

        // Reset scale
        if (useScaleAnimation)
        {
            float t = 0f;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, t / 0.2f);
                yield return null;
            }
            transform.localScale = originalScale;
        }

        // Play result sound
        if (playSoundEffect && AudioManaging.Instance != null)
        {
            AudioManaging.Instance.PlaySFX("dice_result");
        }

        // Brief pause to show result
        yield return new WaitForSeconds(0.5f);

        Debug.Log("DiceAnimator: Animation complete! Calling callback...");
        isAnimating = false;

        // Hide dice (optional - remove if you want it to stay visible)
        HideDice();

        // Trigger callback
        onComplete?.Invoke();
    }

    /// <summary>
    /// Set which dice face to display (0-5 for dice 1-6).
    /// </summary>
    private void SetDiceFace(int index)
    {
        if (index < 0 || index >= diceFaces.Length) return;

        if (uiImage != null)
        {
            uiImage.sprite = diceFaces[index];
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = diceFaces[index];
        }
    }

    /// <summary>
    /// Show the dice display.
    /// </summary>
    public void ShowDice()
    {
        SetVisibility(true);
    }

    /// <summary>
    /// Hide the dice display.
    /// </summary>
    public void HideDice()
    {
        SetVisibility(false);
    }

    /// <summary>
    /// Check if animation is currently playing.
    /// </summary>
    public bool IsAnimating()
    {
        return isAnimating;
    }
}
