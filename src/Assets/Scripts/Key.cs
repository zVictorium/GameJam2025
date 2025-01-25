using UnityEngine;
using UnityEngine.Tilemaps;

public class Key : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    private SpriteRenderer spriteRenderer;
    private Collider2D keyCollider;
    private bool wasCollected = false;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        keyCollider = GetComponent<Collider2D>();
    }

    public void Hide()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (keyCollider != null) keyCollider.enabled = false;
        if (tilemap != null) 
        {
            tilemap.gameObject.SetActive(false);
            tilemap.GetComponent<TilemapCollider2D>().enabled = false;
        }
        wasCollected = true;
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
