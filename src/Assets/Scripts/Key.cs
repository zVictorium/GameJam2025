using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class Key : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private AnimationCurve fadeCurve;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D keyCollider;
    private bool wasCollected = false;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        keyCollider = GetComponent<Collider2D>();
        if (fadeCurve == null)
        {
            fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }
    }

    public void Hide()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (keyCollider != null) keyCollider.enabled = false;
        if (tilemap != null) 
        {
            StartCoroutine(FadeOutTilemap());
        }
        wasCollected = true;
    }

    private IEnumerator FadeOutTilemap()
    {
        TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
        Color originalColor = tilemapRenderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeOutDuration;
            float alpha = fadeCurve.Evaluate(1 - normalizedTime);
            
            tilemapRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        tilemap.gameObject.SetActive(false);
        tilemap.GetComponent<TilemapCollider2D>().enabled = false;
    }

    public void Show()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (keyCollider != null) keyCollider.enabled = true;
        if (tilemap != null) 
        {
            tilemap.gameObject.SetActive(true);
            tilemap.GetComponent<TilemapCollider2D>().enabled = true;
        }
        wasCollected = false;
    }

    public bool IsCollected()
    {
        return wasCollected;
    }
}
