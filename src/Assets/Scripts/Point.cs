using UnityEngine;

public class Point : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D pointCollider;

    private void Start()
    {
        // Buscar el SpriteRenderer en el hijo "Visuals" si existe, si no en el objeto actual
        spriteRenderer = transform.Find("Visuals")?.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        pointCollider = GetComponent<Collider2D>();

        // Verificar que tenemos los componentes necesarios
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en Point ni en sus hijos");
        }
        if (pointCollider == null)
        {
            Debug.LogError("No se encontró Collider2D en Point");
        }
    }

    public void Hide()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (pointCollider != null) pointCollider.enabled = false;
    }

    public void Show()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (pointCollider != null) pointCollider.enabled = true;
    }

    public void Remove()
    {
        Hide();
    }

    public bool IsVisible()
    {
        return spriteRenderer != null && spriteRenderer.enabled;
    }
}
