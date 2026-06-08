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
    public float fuerzaSegundoSalto = 5f;
    public Transform detectorSuelo; // Un objeto vacío en los pies del personaje
    public float radioDeteccion = 0.1f;
    public LayerMask capaSuelo; // Selecciona la capa del suelo en el inspector
    //Para el doble salto
    public int saltosMaximosEnElAire = 2;
    private int saltosRealizados;


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
    private VidaJugador vida; // NUEVA REFERENCIA
    private float mover;
    private bool estaEnSuelo;
    private HitboxAtaque hitbox; // Para detectar enemigos al atacar

    void Start()
    {
        // Buscamos los componentes en ESTE mismo objeto
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vida = GetComponent<VidaJugador>();
        hitbox = GetComponentInChildren<HitboxAtaque>();


        gravedadOriginal = rb.gravityScale; 


    }

    // Update is called once per frame
    void Update()
    {

        // --- No permitir movimiento si está muerto
        if (vida != null && vida.vidasActuales <= 0) 
        {
            // Solo disparamos el Trigger si NO estamos ya en la animación de muerte
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) 
            {
                animator.SetTrigger("Death");
                rb.velocity = Vector2.zero; // Frenazo total
                rb.simulated = false;       // Opcional: Para que los enemigos ya no lo detecten
            }
            return; // Bloquea el resto del script
        }

        if (estaHaciendoDash || atacando) return;

        //Presionar leftShift para el dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashDisponible)
            StartCoroutine(RealizarDash());
        

        if (Input.GetKeyDown(KeyCode.Space) && ataqueDisponible)
            StartCoroutine(RealizarAtaque());
        

        // Detectar entrada
        mover = 0f;
        if (Input.GetKey(KeyCode.D)) mover = 1f;
        if (Input.GetKey(KeyCode.A)) mover = -1f;

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
            animator.ResetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.W) )
        {
            if (estaEnSuelo || saltosRealizados < saltosMaximosEnElAire)
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
                if (animator != null) 
                {
                    // FORZAMOS que deje de creer que está en el suelo por este frame
                    animator.SetBool("isGrounded", false); 
                    animator.SetTrigger("Jump");
                }
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
        if (vida != null && vida.recibiendoDaño) return;

        // Verificamos si tocamos el suelo
        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);

        if (rb == null || estaHaciendoDash) return;

        // Aplicamos el movimiento físico
        float velocidadX = mover * velocidad;
        
        // Detener el movimiento inmediatamente al soltar
        rb.velocity = new Vector2(velocidadX, rb.velocity.y);        
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
 
        // Activar animación
        if (animator != null) animator.SetTrigger("Attack");
 
        // Pequeño delay antes de activar el hitbox (sincroniza con la animación)
        yield return new WaitForSeconds(0.05f);

        if (hitbox != null)
        {
        Vector3 pos = hitbox.transform.localPosition;
        pos.x = spriteRenderer.flipX ? 0.5f : -0.5f;
        hitbox.transform.localPosition = pos;
        hitbox.ActivarHitbox();
        }
 
        // Hitbox activa durante el ataque
        yield return new WaitForSeconds(tiempoAtaque);
 
        // Desactivar hitbox
        if (hitbox != null) hitbox.DesactivarHitbox();
 
        atacando = false;
 
        // Cooldown
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
