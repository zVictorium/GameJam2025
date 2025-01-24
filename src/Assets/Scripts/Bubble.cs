using UnityEngine;
using UnityEngine.Tilemaps;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float brakingForce = 5f; // Nueva variable para la fuerza de frenado
    private bool isBraking = false; // Nueva variable para controlar el frenado
    private Vector2 movement;
    private Vector2 currentVelocity;
    private bool isMoving = false;
    private Rigidbody2D rb;
    private SpriteRenderer bubbleSprite;

    void Start()
    {
        bubbleSprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        // Aumentamos el damping base
        rb.linearDamping = 2f; // Ajusta este valor según necesites
    }

    void FixedUpdate()
    {
        if (!isMoving)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            
            if (movement.magnitude > 0)
            {
                movement = movement.normalized;
                isMoving = true;
            }
        }

        // Solo aplicamos fuerza si estamos en movimiento
        if (isMoving)
        {
            currentVelocity = Vector2.Lerp(currentVelocity, movement * moveSpeed, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = currentVelocity;
        }

        // Aplicamos frenado adicional si es necesario
        if (isBraking)
        {
            Vector2 oppositeForce = -rb.linearVelocity.normalized * brakingForce;
            rb.AddForce(oppositeForce, ForceMode2D.Force);
        }

        // Detectamos si se ha detenido por la fricción
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            isMoving = false;
            isBraking = false;
            movement = Vector2.zero;
            currentVelocity = Vector2.zero;
            rb.linearVelocity = Vector2.zero; // Aseguramos que se detenga completamente
            
            // Centramos la burbuja en el tile más cercano
            Vector2 tileCenter = GetNearestTileCenter();
            transform.position = tileCenter;
        }
    }

    private Vector2 GetNearestTileCenter()
    {
        Vector2 currentPos = transform.position;
        float x = Mathf.Floor(currentPos.x) + 0.5f;
        float y = Mathf.Floor(currentPos.y) + 0.5f;
        return new Vector2(x, y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Map map = other.GetComponent<Map>();
        if (map != null && map.IsWall())
        {
            bubbleSprite.color = Color.red;
            isMoving = false;
            movement = Vector2.zero;
            isBraking = true;
            Debug.Log("Entrando en pared y frenando");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Map map = other.GetComponent<Map>();
        if (map != null && map.IsWall())
        {
            bubbleSprite.color = Color.white;
            isBraking = false;
            Debug.Log("Saliendo de la pared");
        }
    }
}