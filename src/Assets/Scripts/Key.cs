using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class Key : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    
    private Collider2D keyCollider;
    private bool wasCollected = false;

    private void Start()
    {
        keyCollider = GetComponent<Collider2D>();
        originalColor = tilemap.color;
        spriteRenderer = transform.Find("Visuals").GetComponent<SpriteRenderer>();
    }

    public void Hide()
    {
        wasCollected = true;
        if (keyCollider != null) keyCollider.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float duration = 1f; // duración del fade en segundos
        float elapsed = 0f;
        Color originalColor = tilemap.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            Color newColor = originalColor;
            newColor.a = 1f - normalizedTime;
            tilemap.color = newColor;
            yield return null;
        }

        if (tilemap != null) 
        {
            tilemap.enabled = false;
            tilemap.gameObject.SetActive(false);
            tilemap.GetComponent<TilemapCollider2D>().enabled = false;
        }
    }

    public void Show()
    {
        if (tilemap != null) 
        {
            tilemap.enabled = true;
            tilemap.color = originalColor;
            tilemap.gameObject.SetActive(true);
            tilemap.GetComponent<TilemapCollider2D>().enabled = true;
        }
        if (keyCollider != null) keyCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        wasCollected = false;
    }

    public bool IsCollected()
    {
        return wasCollected;
    }
}
