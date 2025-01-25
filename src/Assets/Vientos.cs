using UnityEngine;

public class Vientos : MonoBehaviour
{
    private SpriteRenderer[] childRenderers;

    void Start()
    {
        // Obtener los SpriteRenderer de los hijos
        childRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        // Mover y escalar cada hijo aleatoriamente
        foreach (var renderer in childRenderers)
        {
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = Random.Range(-0.5f, 0.5f);
            renderer.transform.localPosition += new Vector3(randomX, randomY, 0);
            
            float randomScale = Random.Range(0.8f, 1.2f);
            renderer.transform.localScale = new Vector3(randomScale, randomScale, 1f);
            
            // Añadir offset aleatorio a la animación
            Animator animator = renderer.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(0, -1, Random.Range(0f, 1f));
            }
        }
    }

    // Método para cambiar la visibilidad
    public void SetVisibility(bool isVisible)
    {
        foreach (var renderer in childRenderers)
        {
            renderer.enabled = isVisible;
        }
    }

    // Método para rotar el objeto
    public void RotateObject(float degrees)
    {
        transform.Rotate(Vector3.forward * degrees);
    }
}
