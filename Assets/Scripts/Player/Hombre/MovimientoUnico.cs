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
    public float fuerzaSegundoSalto = 0.001f;
    public Transform detectorSuelo; // Un objeto vacío en los pies del personaje
    public float radioDeteccion = 0.9f;
    public LayerMask capaSuelo; // Selecciona la capa del suelo en el inspector
    //Para el doble salto
    public int saltosMaximos = 2;
    private int saltosRealizados;

    [Header("Configuración Daño")]
    public float fuerzaImpulso = 5f;
    public float duracionParpadeo = 0.5f;

    [Header("Configuración Vidas")]
    public int vidasMaximas = 3;
    private int vidasActuales;
    private bool estaMuerto = false;

    [Header("Configuración Dash")]
    public float velocidadDash = 20f;
    public float duracionDash = 0.2f;
    public float cooldownDash = 1f;
    private bool estaHaciendoDash = false;
    private bool dashDisponible = true;
    private float gravedadOriginal;

    [Header("Configuración Ataque")]
    public float tiempoAtaque = 0.3f; // Cuánto dura la animación/hitbox
    public float cooldownAtaque = 0.2f; // Tiempo entre ataques
    private bool atacando = false;
    private bool ataqueDisponible = true;



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

        gravedadOriginal = rb.gravityScale; 


    }

    // Update is called once per frame
    void Update()
    {

        // --- No permitir movimiento si está recibiendo daño
        if (recibiendoDaño || estaMuerto || estaHaciendoDash || atacando) return;

        //Presionar leftShift para el dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashDisponible)
        {
            StartCoroutine(RealizarDash());
        }

        if (Input.GetKeyDown(KeyCode.Space) && ataqueDisponible)
        {
            StartCoroutine(RealizarAtaque());
        }

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
        

        if (estaEnSuelo)
        {
            saltosRealizados = 0;
        }

        if (Input.GetKeyDown(KeyCode.W) )
        {
            if (estaEnSuelo || saltosRealizados < saltosMaximos)
        {
            // Aplicar fuerza (reseteamos velocidad vertical antes para salto uniforme)
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            
            if (saltosRealizados == 0)
                {
                    // Primer salto
                    rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
                }
                else
                {
                    // Segundo salto 
                    rb.AddForce(Vector2.up * fuerzaSegundoSalto, ForceMode2D.Impulse);
                }
            
            saltosRealizados++;
            
            // Trigger de animación (puedes usar el mismo trigger de salto)
            if (animator != null) animator.SetTrigger("Jump");
        }
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
        // Verificamos si tocamos el suelo
        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);

        if (rb == null || recibiendoDaño || estaMuerto|| estaHaciendoDash) return;

        // Aplicamos el movimiento físico
        float velocidadX = mover * velocidad;
        
        // Detener el movimiento inmediatamente al soltar
        rb.velocity = new Vector2(velocidadX, rb.velocity.y);        
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

    // Función para recuperar vida
    public void Curar(int cantidad)
    {
        if (estaMuerto) return;

        vidasActuales += cantidad;

        // Asegurarnos de no sobrepasar las vidas máximas
        if (vidasActuales > vidasMaximas)
        {
            vidasActuales = vidasMaximas;
        }

        Debug.Log("Vida recuperada. Vida actual: " + vidasActuales);

        StartCoroutine(FeedbackCuracion());
    }

    private IEnumerator FeedbackCuracion()
    {
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }



    private void Morir()
    {
        estaMuerto = true;
        recibiendoDaño = false; // Ya no recibe daño, está muerto
        
        // Ajustar el collider dspues de la muerte
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            // Reducimos la altura a la mitad, por ejemplo
            collider.size = new Vector2(collider.size.x, collider.size.y * 0.09f);
            // Ajustamos el centro para que la parte inferior siga en el suelo
            collider.offset = new Vector2(collider.offset.x, collider.offset.y - (collider.size.y * .9f));
        }

        

        // Cambiar capa para colisionar solo con el suelo ---
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");

        // Activar animación de muerte
        if (animator != null) animator.SetTrigger("Death");

        this.enabled = false;
        
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

    private IEnumerator RealizarDash()
    {
        dashDisponible = false;
        estaHaciendoDash = true;

        // 1. Activar animación
        if (animator != null) animator.SetTrigger("Dash");

        // 2. Aplicar velocidad de dash (ignorar gravedad)
        rb.gravityScale = 0f;

        // Dash hacia donde mira el personaje
        float direccionDash = spriteRenderer.flipX ? 1f : -1f; 
        rb.velocity = new Vector2(direccionDash * velocidadDash, 0f);

        // 3. Esperar duración del dash
        yield return new WaitForSeconds(duracionDash);

        // 4. Finalizar dash
        rb.gravityScale = gravedadOriginal;

        estaHaciendoDash = false;

        // 5. Cooldown
        yield return new WaitForSeconds(cooldownDash);
        dashDisponible = true;
    }


    private IEnumerator RealizarAtaque()
    {
        ataqueDisponible = false;
        atacando = true;

        // 1. Activar animación
        if (animator != null) animator.SetTrigger("Attack");

        // 3. Esperar duración del ataque (hitbox activa)
        yield return new WaitForSeconds(tiempoAtaque);

        // 4. Finalizar ataque
        atacando = false;

        // 5. Cooldown antes de poder atacar de nuevo
        yield return new WaitForSeconds(cooldownAtaque);
        ataqueDisponible = true;
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
