using UnityEngine;

public class Medusa : MonoBehaviour
{
    [SerializeField] private Vector3 positionA;
    [SerializeField] private Vector3 positionB;
    [SerializeField] private float moveSpeed = 1f;
    
    private float lerpTime = 0f;
    private bool movingToB = true;

    void Update()
    {
        lerpTime += Time.deltaTime * moveSpeed;
        
        if (movingToB)
        {
            transform.position = Vector3.Lerp(positionA, positionB, lerpTime);
            if (lerpTime >= 1f)
            {
                lerpTime = 0f;
                movingToB = false;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(positionB, positionA, lerpTime);
            if (lerpTime >= 1f)
            {
                lerpTime = 0f;
                movingToB = true;
            }
        }
    }
}
