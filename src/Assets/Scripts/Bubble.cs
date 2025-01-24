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
        
        // Aumentamos el drag base
        rb.drag = 2f; // Ajusta este valor según necesites
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
            rb.velocity = currentVelocity;
        }

        // Aplicamos frenado adicional si es necesario
        if (isBraking)
        {
            Vector2 oppositeForce = -rb.velocity.normalized * brakingForce;
            rb.AddForce(oppositeForce, ForceMode2D.Force);
        }

        // Detectamos si se ha detenido por la fricción
        if (rb.velocity.magnitude < 0.1f)
        {
            isMoving = false;
            isBraking = false;
            movement = Vector2.zero;
            currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero; // Aseguramos que se detenga completamente
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BoxCollider2D>() != null || other.GetComponent<TilemapCollider2D>() != null)
        {
            bubbleSprite.color = Color.red;
            isMoving = false;
            movement = Vector2.zero;
            isBraking = true; // Activamos el frenado
            Debug.Log("Entrando en área y frenando");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BoxCollider2D>() != null || other.GetComponent<TilemapCollider2D>() != null)
        {
            bubbleSprite.color = Color.white;
            isBraking = false; // Desactivamos el frenado
            Debug.Log("Saliendo del área"); // Para depuración
        }
    }
}