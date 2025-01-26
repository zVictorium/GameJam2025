using UnityEngine;

public class Torbellino : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col2D;

    void Start()
    {
        spriteRenderer = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void DisableVisuals()
    {
        spriteRenderer.enabled = false;
        col2D.enabled = false;
    }

    public void EnableVisuals()
    {
        spriteRenderer.enabled = true;
        col2D.enabled = true;
    }
}
