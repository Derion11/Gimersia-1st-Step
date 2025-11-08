using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] SpriteRenderer spRenderer;
    private void Start()
    {
        if (spRenderer != null)
        {
            Debug.Log($"SpriteRenderer{spRenderer.sprite.name}");
        }
        else
        {
            Debug.Log("SpriteRenderer tidak ada");
        }
    }
    private void Update()
    {
        Debug.Log("Calling Update...");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key was pressed.");
            if (spRenderer != null)
            {
                spRenderer.color = Color.red;
            }
            else
            {
                Debug.Log("SpriteRenderer tidak ada");
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Space key was pressed.");
            if (spRenderer != null)
            {
                spRenderer.color = Color.yellow;
            }
            else
            {
                Debug.Log("SpriteRenderer tidak ada");
            }
        }
        // Cek perubahan disini guys lesgoo
    }
}
