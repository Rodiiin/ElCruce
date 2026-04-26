using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perro : MonoBehaviour
{
    [Header("Detección Rectangular")]
    public Vector2 tamanoDeteccion = new Vector2(8f, 4f); // Ancho y Alto
    public Vector2 offsetDeteccion = new Vector2(0f, 2f); // Desplazamiento del área
    public LayerMask capaJugadores;

    [Header("Configuración de Movimiento")]
    public float velocidadCorrer = 6f;

    [Header("Tiempos")]
    public float tiempoEsperaAtaque = 0.74f;
   

    [Header("Referencias Hurtbox 1")]
    public Transform attackPoint;
    public float radioGolpe = 0.5f; // Tamaño del mordisco
    
    [Header("Referencias Hurtbox 2 ")]
    public Transform attackPoint2;
    public Vector2 tamanoGolpe2 = new Vector2(1f, 1f);

    private Transform jugadorObjetivo;
    private Rigidbody2D rb;
    private Animator animator;
    private bool estaAtacando = false;
    private Vector2 posicionInicial; // Para guardar el punto de guardia

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        posicionInicial = transform.position;
    }

    void Update()
    {
        if (estaAtacando) return;

        EncontrarJugador();

        if (jugadorObjetivo != null)
        {
            
            Collider2D golpe1 = Physics2D.OverlapCircle(attackPoint.position, radioGolpe, capaJugadores);
            
            Collider2D golpe2 = Physics2D.OverlapBox(attackPoint2.position, tamanoGolpe2, 0f, capaJugadores);

                if (golpe1 != null || golpe2 != null)
            {
                // Si el círculo rojo toca al jugador, ¡Ataca!
                PrepararAtaque();
            }
            else
            {
                // Si lo ve en el rectángulo azul pero no llega el círculo rojo, corre
                Perseguir();
            }
        }
        else
        {
            Detenerse();
        }
    }

    void EncontrarJugador()
    {
        // Calculamos el centro real sumando la posición del perro + el offset
        Vector2 centroReal = posicionInicial + offsetDeteccion;

        // Buscamos jugadores en el área
        Collider2D jugador = Physics2D.OverlapBox(centroReal, tamanoDeteccion, 0f, capaJugadores);

        if (jugador != null)
        {
            // Verificamos Tags como medida extra
            if (jugador.CompareTag("Player") || jugador.CompareTag("Player2"))
            {
                jugadorObjetivo = jugador.transform;
            }
        }
        else
        {
            jugadorObjetivo = null;
        }
    }

    void Perseguir()
    {
        animator.SetBool("isRunning", true);

        // Calculamos dirección
        float direccionX = jugadorObjetivo.position.x - transform.position.x;
        
        // Aplicamos velocidad
        rb.velocity = new Vector2(Mathf.Sign(direccionX) * velocidadCorrer, rb.velocity.y);

        // OPCIÓN B: Girar el transform (escala)
        Girar(direccionX);
    }

    void Girar(float direccionX)
    {
        if (direccionX > 0)
            transform.localScale = new Vector3(1, 1, 1); // Mirar derecha
        else if (direccionX < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Mirar izquierda
    }

    void Detenerse()
    {
        animator.SetBool("isRunning", false);
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void PrepararAtaque()
    {
        estaAtacando = true;
        rb.velocity = Vector2.zero; // Se detiene para morder
        animator.SetBool("isRunning", false);
        animator.SetTrigger("attack");

        // Espera los 0.74 segundos antes de volver a rastrear
        StartCoroutine(EsperarParaSiguienteAtaque());
    }

    // Esta es la corrutina que maneja el tiempo de espera
    IEnumerator EsperarParaSiguienteAtaque()
    {
        // Esperamos el tiempo que definiste en el inspector (0.74s)
        yield return new WaitForSeconds(tiempoEsperaAtaque);
        
        // Al terminar el tiempo, liberamos al perro para que vuelva a perseguir
        estaAtacando = false;
    }
    public void RealizarDaño()
    {
        // Daño Círculo
        if (attackPoint != null)
        {
            Collider2D[] toques1 = Physics2D.OverlapCircleAll(attackPoint.position, radioGolpe, capaJugadores);
            AplicarDañoALista(toques1);
        }

        // Daño Rectángulo - CAMBIADO A OverlapBoxAll
        if (attackPoint2 != null)
        {
            Collider2D[] toques2 = Physics2D.OverlapBoxAll(attackPoint2.position, tamanoGolpe2, 0f, capaJugadores);
            AplicarDañoALista(toques2);
        }
    }

    // Método auxiliar para no repetir código de daño
    void AplicarDañoALista(Collider2D[] toques)
    {
        foreach (Collider2D col in toques)
        {
            // 1. Intentamos obtener ambos componentes
            VidaJugador vidaJ1 = col.GetComponent<VidaJugador>();
            VidaJugador2 vidaJ2 = col.GetComponent<VidaJugador2>();

            // 2. Calculamos la dirección del empuje
            Vector2 direccionGolpe = col.transform.position - transform.position;

            // 3. Aplicamos el daño a quien corresponda
            if (vidaJ1 != null)
            {
                vidaJ1.RecibirDaño(direccionGolpe);
            }
            else if (vidaJ2 != null)
            {
                vidaJ2.RecibirDaño(direccionGolpe);
            }
        }
    }

    // Para ver el rango en el editor
    private void OnDrawGizmosSelected()
    {
        // Dibujar el área de detección rectangular (Amarillo/Azul)
        Gizmos.color = Color.blue;

        Vector2 centroParaGizmo = Application.isPlaying ? posicionInicial : (Vector2)transform.position;

        Vector2 centroReal = centroParaGizmo + offsetDeteccion;
        Gizmos.DrawWireCube(centroReal, tamanoDeteccion);
        
        // Hurtbox 1 (Rojo)
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, radioGolpe);
        }

        // Hurtbox 2 (Verde)
        if (attackPoint2 != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(attackPoint2.position, tamanoGolpe2);
        }
    }
}
