using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deckard : MonoBehaviour
{
    [Header("Detección")]
    public float radioDeteccion = 5f;
    public LayerMask capaJugadores; // Selecciona "Player" en el Inspector
    
    private Transform jugadorObjetivo;
    private Animator animator;
    private bool jugadorEnRango = false;

    [Header("Ataque")]
    public GameObject navajaPrefab;
    public Transform puntoDisparo;
    public float tiempoEntreNavajas = 2f;
    private float cronometroAtaque;

    public GameObject efectoHumo; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        EncontrarJugadorMasCercano();

        if (jugadorObjetivo != null)
        {
            float distancia = Vector2.Distance(transform.position, jugadorObjetivo.position);

            if (distancia <= radioDeteccion)
            {
                jugadorEnRango = true;
                ActualizarMirada();
                if (jugadorEnRango)
                {
                    cronometroAtaque += Time.deltaTime;
                    if (cronometroAtaque >= tiempoEntreNavajas)
                    {
                        LanzarNavaja();
                        cronometroAtaque = 0;
                    }
                }
            }
            else
            {
                RegresarAFrente();
            }
        }
        else
        {
            RegresarAFrente();
        }

    }

    void EncontrarJugadorMasCercano()
    {
        // Buscamos a todos los posibles jugadores en el radio
        Collider2D[] jugadoresEncontrados = Physics2D.OverlapCircleAll(transform.position, radioDeteccion, capaJugadores);
        
        float distanciaCercana = Mathf.Infinity;
        Transform objetivoTemporal = null;

        foreach (Collider2D col in jugadoresEncontrados)
        {
            // Verificamos que sea P1 o P2 por su Tag
            if (col.CompareTag("Player") || col.CompareTag("Player2"))
            {
                float distancia = Vector2.Distance(transform.position, col.transform.position);
                if (distancia < distanciaCercana)
                {
                    distanciaCercana = distancia;
                    objetivoTemporal = col.transform;
                }
            }
        }
        jugadorObjetivo = objetivoTemporal;
    }
    void ActualizarMirada()
    {
        animator.SetBool("PlayerEnRango", true);

        // Calculamos la dirección X (Posición Jugador - Posición Deckard)
        float direccionX = jugadorObjetivo.position.x - transform.position.x;

        // Normalizamos el valor para que sea 1 (derecha) o -1 (izquierda)
        float valorAnim = direccionX > 0 ? 1f : -1f;

        animator.SetFloat("DirectionX", valorAnim);
    }

    void RegresarAFrente()
    {
        if (jugadorEnRango)
        {
            jugadorEnRango = false;
            cronometroAtaque = 0;
            animator.SetBool("PlayerEnRango", false);
            animator.SetFloat("DirectionX", 0f);
        }
    }

    void LanzarNavaja()
    {
        if (jugadorObjetivo == null) return;

        // Instanciar la navaja
        GameObject nuevaNavaja = Instantiate(navajaPrefab, puntoDisparo.position, Quaternion.identity);
        
        // Calcular dirección hacia la ubicación actual del player
        Vector2 direccionAtaque = jugadorObjetivo.position - puntoDisparo.position;
        
        // Configurar el vuelo
        nuevaNavaja.GetComponent<NavajaVuelo>().Configurar(direccionAtaque, puntoDisparo.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si lo que toca a Deckard es un Jugador...
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            VidaJugador vida = collision.GetComponent<VidaJugador>();
            if (vida != null)
            {
                // Calculamos dirección del golpe (Desde Deckard hacia el Player)
                Vector2 direccionGolpe = collision.transform.position - transform.position;
                
                // Aplicamos daño y empujón
                vida.RecibirDaño(direccionGolpe);

                // ACTIVAR EL HUMO (Si lo tienes configurado)
                ActivarEfectoHumo();
            }
        }
    }

    void ActivarEfectoHumo()
    {
        // Si reciclamos el objeto efectoHumo que usamos con Roy:
        if (efectoHumo != null)
        {
            CancelInvoke("DesactivarHumo");

            efectoHumo.SetActive(true);

            Animator humoAnim = efectoHumo.GetComponent<Animator>();

            if (humoAnim != null)
            {
                humoAnim.SetTrigger("Humo_Ataque");
            }
            // Lo apagamos después de un momento
            Invoke("DesactivarHumo", 0.5f); 
        }
    }

    void DesactivarHumo()
    {
        if(efectoHumo != null) efectoHumo.SetActive(false);
    }


    // Dibujamos el círculo de rango en la pestaña Scene para poder ajustarlo fácil
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}
