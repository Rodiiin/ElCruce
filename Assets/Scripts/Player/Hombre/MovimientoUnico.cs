using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoUnico : MonoBehaviour
{
    [Header("Configuración movimiento")]
    public float velocidad = 5f;
    public float suavizado = 0.05f;

    [Header("Configuración Salto")]
    public float fuerzaSalto = 5f;
    public Transform detectorSuelo; // Un objeto vacío en los pies del personaje
    public float radioDeteccion = 0.2f;
    public LayerMask capaSuelo; // Selecciona la capa del suelo en el inspector

    [Header("Configuración Daño")]
    public float fuerzaImpulso = 5f;
    public float duracionParpadeo = 0.5f;

    [Header("Configuración Vidas")]
    public int vidasMaximas = 3;
    private int vidasActuales;
    private bool estaMuerto = false;


    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float mover;
    private Vector2 velocidadSuavizada = Vector2.zero;
    private bool estaEnSuelo;
    private bool recibiendoDaño = false;

    void Start()
    {
        // Buscamos los componentes en ESTE mismo objeto
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vidasActuales = vidasMaximas;

    }

    // Update is called once per frame
    void Update()
    {

        // --- No permitir movimiento si está recibiendo daño
        if (recibiendoDaño || estaMuerto) return;

        // Detectar entrada
        mover = Input.GetAxis("Horizontal");

        // --- Lógica de giro
        if (mover > 0) {
            // Mirar a la izquierda
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (mover < 0) {
            // Mirar a la derecha
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }

        // - Lógica de salto (Detección) ---
        // Verificamos si tocamos el suelo
        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);

        if (Input.GetKeyDown(KeyCode.W) && estaEnSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            if (animator != null) animator.SetTrigger("Jump"); 
        }

        // --- ACTUALIZAR PARÁMETROS DE ANIMACIÓN ---
        if (animator != null) {
            // Speed para caminar
            animator.SetFloat("Speed", Mathf.Abs(mover));
            // VelY para saber si sube o baja
            animator.SetFloat("VelY", rb.velocity.y);
            // Bool para saber si está en el suelo
            animator.SetBool("isGrounded", estaEnSuelo);
        }
    }

    void FixedUpdate()
    {
        if (rb == null || recibiendoDaño || estaMuerto) return;

        // Aplicamos el movimiento físico
        Vector2 velocidadObjetivo = new Vector2(mover * velocidad, rb.velocity.y);
        
        // Detener el movimiento inmediatamente al soltar
        rb.velocity = Vector2.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidadSuavizada, suavizado);
        
    }

    // --- Método para recibir daño 
    public void RecibirDaño(Vector2 direccionGolpe)
    {
        if (recibiendoDaño || estaMuerto) return; // Evitar múltiples golpes seguidos

        vidasActuales --;

        if (vidasActuales <= 0)
        {
            Morir();
            return; 
        }

        recibiendoDaño = true;
        
        // 1. Activar animación
        if (animator != null) animator.SetTrigger("Hit");

        // 2. Aplicar impulso físico
        rb.velocity = Vector2.zero; // Detener movimiento actual
        rb.AddForce(direccionGolpe.normalized * fuerzaImpulso, ForceMode2D.Impulse);

        // 3. Iniciar parpadeo rojo
        StartCoroutine(ParpadeoRojo());
    }

    private void Morir()
    {
        estaMuerto = true;
        recibiendoDaño = false; // Ya no recibe daño, está muerto
        
        
        // Activar animación de muerte
        if (animator != null) animator.SetTrigger("Death");
        
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
        // Opcional: Desactivar colisiones o física para que no interfiera
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; // El cuerpo se queda quieto en el suelo
        
        Debug.Log("El jugador ha muerto");
        // Aquí podrías llamar a una pantalla de Game Over
    }

    private IEnumerator ParpadeoRojo()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(duracionParpadeo);
        spriteRenderer.color = Color.white;
        recibiendoDaño = false; // Permitir movimiento de nuevo
    }







    // --- VISUALIZACIÓN DE GIZMOS ---
    private void OnDrawGizmos()
    {
        if (detectorSuelo != null)
        {
            // Dibujamos una esfera roja en la posición del detector para ver el rango
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        }
    }


}
