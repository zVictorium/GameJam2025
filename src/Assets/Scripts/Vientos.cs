using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vientos : MonoBehaviour
{
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
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

    // Método para cambiar la visibilidad usando opacidad
    public void SetVisibility(bool isVisible)
    {
        float maxAlpha = 20f/255f; // Aproximadamente 0.078
        float targetAlpha = isVisible ? maxAlpha : 0f;
        StopAllCoroutines(); // Detener cualquier fade en progreso
        StartCoroutine(FadeSprites(targetAlpha, 0.2f)); // Aumentado a 2 segundos
    }

    private IEnumerator FadeSprites(float targetAlpha, float duration)
    {
        float elapsedTime = 0;
        Dictionary<SpriteRenderer, float> startAlphas = new Dictionary<SpriteRenderer, float>();

        // Guardar alphas iniciales
        foreach (var renderer in childRenderers)
        {
            startAlphas[renderer] = renderer.color.a;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float evaluatedT = fadeCurve.Evaluate(t);

            foreach (var renderer in childRenderers)
            {
                float startAlpha = startAlphas[renderer];
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, evaluatedT);
                Color color = renderer.color;
                color.a = newAlpha;
                renderer.color = color;
            }
            
            yield return null;
        }

        // Asegurar valor final
        foreach (var renderer in childRenderers)
        {
            Color color = renderer.color;
            color.a = targetAlpha;
            renderer.color = color;
        }
    }

    // Método para rotar el objeto
    public void RotateObject(float degrees)
    {
        transform.Rotate(Vector3.forward * degrees);
    }
}
