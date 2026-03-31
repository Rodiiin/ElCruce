using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampaElectrica : MonoBehaviour
{
    [Header("Configuración de Tiempos")]
    public float tiempoSeguro = 2f;    // Tiempo apagado
    public float tiempoPeligroso = 1.5f; // Tiempo con electricidad
    public int daño = 1;

    [Header("Referencias")]
    // Arrastra aquí el objeto HIJO 'Animacion_Electricidad'
    public GameObject objetoAnimacion; 


    private CircleCollider2D[] hitboxes;
    private Animator animatorElectricidad;
    private SpriteRenderer spriteRendererElectricidad;
    private bool estaActiva = false;


    void Awake()
    {
        hitboxes = GetComponents<CircleCollider2D>();
        
        if (objetoAnimacion != null)
        {
            animatorElectricidad = objetoAnimacion.GetComponent<Animator>();
            spriteRendererElectricidad = objetoAnimacion.GetComponent<SpriteRenderer>();
            if (spriteRendererElectricidad != null) spriteRendererElectricidad.enabled = false;
        }

        // Empezamos apagados
        CambiarEstado(false);
        StartCoroutine(CicloElectrico());
    }

    IEnumerator CicloElectrico()
    {
        while (true)
        {
            // --- ESTADO SEGURO ---
            CambiarEstado(false);
            yield return new WaitForSeconds(tiempoSeguro);

            // --- ESTADO PELIGROSO ---
            CambiarEstado(true);
            yield return new WaitForSeconds(tiempoPeligroso);
        }
    }

    void CambiarEstado(bool activar)
    {
        estaActiva = activar;

        // 1. Activar/Desactivar Colliders (Daño)
        foreach (var col in hitboxes)
        {
            col.enabled = activar;
        }

        // 2. Controlar la animación visual
        if (animatorElectricidad != null && spriteRendererElectricidad != null)
        {
            if (activar)
            {
                spriteRendererElectricidad.enabled = true;
                // Usamos Play(0) para reiniciar la animación desde el principio
                animatorElectricidad.Play("Electricidad_Activa", 0, 0f);
            }
            else
            {
                spriteRendererElectricidad.enabled = false;
                animatorElectricidad.Play("Apagado");
            }
        }
    }

    

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Si la trampa está activa y toca al jugador
        if (estaActiva && (collision.CompareTag("Player") || collision.CompareTag("Player2")))
        {
            VidaJugador vida = collision.GetComponent<VidaJugador>();
            if (vida != null)
            {
                // Dirección del empuje (desde el centro de la trampa hacia el jugador)
                Vector2 direccion = collision.transform.position - transform.position;
                vida.RecibirDaño(direccion);
            }
        }
    }

}
