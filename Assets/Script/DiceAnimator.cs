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
    
    [Tooltip("How long to display final result (seconds)")]
    public float resultDisplayDuration = 0.5f;
    
    [Tooltip("Play sound effect during animation")]
    public bool playSoundEffect = true;

    [Header("Visual Effects")]
    [Tooltip("Enable dice to grow/shrink during animation")]
    public bool useScaleAnimation = true;
    
    [Tooltip("Scale multiplier during animation")]
    public float scaleMultiplier = 1.2f;
    
    [Tooltip("Enable rotation during roll")]
    public bool useRotation = true;
    
    [Tooltip("Rotation speed (degrees per second)")]
    public float rotationSpeed = 720f;
    
    [Tooltip("Enable glow/pulse effect on final result")]
    public bool useGlowEffect = true;
    
    [Tooltip("Shake intensity during roll")]
    public float shakeIntensity = 0.1f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
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
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

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

        // Phase 1: Rapid random flashing with effects
        while (elapsed < animationDuration)
        {
            // Show random dice face
            int randomFace = Random.Range(0, 6);
            SetDiceFace(randomFace);

            float t = elapsed / animationDuration;

            // Scale animation (pulse effect)
            if (useScaleAnimation)
            {
                float pulseT = Mathf.PingPong(elapsed / animationDuration * 2f, 1f);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, pulseT);
            }

            // Rotation effect (spinning dice)
            if (useRotation)
            {
                float angle = rotationSpeed * Time.deltaTime;
                transform.Rotate(0, 0, angle);
            }

            // Shake effect (screen shake feel)
            if (shakeIntensity > 0)
            {
                Vector3 shake = new Vector3(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity, shakeIntensity),
                    0
                );
                transform.localPosition = originalPosition + shake;
            }

            // Wait before next flash
            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed;
        }

        // Reset position and rotation
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;

        // Phase 2: Show final result
        SetDiceFace(finalValue - 1); // Convert 1-6 to 0-5 index

        // Dramatic scale-in animation for result
        if (useScaleAnimation)
        {
            // Shrink then grow (bounce effect)
            float t = 0f;
            float bounceTime = 0.3f;
            
            while (t < bounceTime)
            {
                t += Time.deltaTime;
                float progress = t / bounceTime;
                
                // Bounce curve: shrink -> overshoot -> settle
                float bounceScale = 1f;
                if (progress < 0.5f)
                {
                    // Shrink phase
                    bounceScale = Mathf.Lerp(scaleMultiplier, 0.8f, progress * 2f);
                }
                else
                {
                    // Grow and overshoot phase
                    float bounceProgress = (progress - 0.5f) * 2f;
                    bounceScale = Mathf.Lerp(0.8f, 1.2f, bounceProgress);
                }
                
                transform.localScale = originalScale * bounceScale;
                yield return null;
            }
            
            // Settle to normal size
            t = 0f;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t / 0.2f);
                yield return null;
            }
            transform.localScale = originalScale;
        }

        // Play result sound
        if (playSoundEffect && AudioManaging.Instance != null)
        {
            AudioManaging.Instance.PlaySFX("dice_result");
        }

        // Phase 3: Glow/pulse effect on final result
        if (useGlowEffect)
        {
            float glowTime = 0f;
            float glowDuration = resultDisplayDuration * 0.7f; // Glow during most of display time
            
            while (glowTime < glowDuration)
            {
                glowTime += Time.deltaTime;
                
                // Pulse scale slightly (breathing effect)
                float pulseT = Mathf.Sin(glowTime * 5f) * 0.05f + 1f;
                transform.localScale = originalScale * pulseT;
                
                // Optional: Color pulse (if you want color change)
                // Note: This won't work without additional setup, but adding for future
                
                yield return null;
            }
            
            // Reset scale
            transform.localScale = originalScale;
        }

        // Display final result for readable duration
        yield return new WaitForSeconds(resultDisplayDuration - (useGlowEffect ? resultDisplayDuration * 0.7f : 0f));

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
