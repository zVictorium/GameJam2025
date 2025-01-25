using UnityEngine;

public class Medusa : MonoBehaviour
{
    [SerializeField] private Vector3 positionA;
    [SerializeField] private Vector3 positionB;
    [SerializeField] private float moveSpeed = 0.75f;
    
    private float lerpTime = 0f;
    private bool movingToB = true;

    void Update()
    {
        lerpTime += Time.deltaTime * moveSpeed;
        
        if (movingToB)
        {
            float smoothValue = Mathf.SmoothStep(0f, 1f, Mathf.Pow(lerpTime, 3));
            transform.position = Vector3.Lerp(positionA, positionB, smoothValue);
            if (lerpTime >= 1f)
            {
                lerpTime = 0f;
                movingToB = false;
            }
        }
        else
        {
            float smoothValue = Mathf.SmoothStep(0f, 1f, Mathf.Pow(lerpTime, 3));
            transform.position = Vector3.Lerp(positionB, positionA, smoothValue);
            if (lerpTime >= 1f)
            {
                lerpTime = 0f;
                movingToB = true;
            }
        }
    }
}
