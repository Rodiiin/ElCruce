using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaJugador2 : MonoBehaviour
{
    [Header("Configuración Vidas")]
    public int vidasMaximas = 3;
    public int vidasActuales;
    public bool estaMuerto = false;
    public bool recibiendoDaño = false;

    [Header("Efectos Visuales")]
    public float duracionParpadeo = 0.5f;
    public float fuerzaImpulso = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vidasActuales = vidasMaximas;
    }

    public void RecibirDaño(Vector2 direccionGolpe)
    {
        if (recibiendoDaño || estaMuerto) return;

        vidasActuales--;
        FindObjectOfType<ControladorVidaUI>().ActualizarCorazones(vidasActuales, false);
        
        if (vidasActuales <= 0)
        {
            Morir();
            return;
        }

        recibiendoDaño = true;
        if (animator != null) animator.SetTrigger("Hit");

        rb.velocity = Vector2.zero;
        rb.AddForce(direccionGolpe.normalized * fuerzaImpulso, ForceMode2D.Impulse);
        StartCoroutine(Parpadeo(Color.red));
    }

    public void Curar(int cantidad)
    {
        if (estaMuerto) return;

        vidasActuales = Mathf.Min(vidasActuales + cantidad, vidasMaximas);
        
        // ESTA ES LA LÍNEA QUE FALTA:
        FindObjectOfType<ControladorVidaUI>().ActualizarCorazones(vidasActuales, false);
        
        StartCoroutine(Parpadeo(Color.green));
    }

    private void Morir()
    {
        estaMuerto = true;
        if (animator != null) animator.SetTrigger("Death");

        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");

        if (rb != null)
        {
            // Frenamos movimiento horizontal pero dejamos que caiga si está en el aire
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.gravityScale = 2f; 
        }

        // Achicamos el collider
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(col.size.x, col.size.y * 0.1f);
            col.offset = new Vector2(col.offset.x, col.offset.y - (col.size.y * 0.9f));
        }

        // APAGAR EL MOVIMIENTO:
        // Buscamos el script de movimiento y lo desactivamos
        MovimientoJugador2 mov = GetComponent<MovimientoJugador2>();
        if (mov != null) mov.enabled = false;

        this.enabled = false; // Apaga este script de vida también
    }

    private IEnumerator Parpadeo(Color colorEfecto)
    {
        spriteRenderer.color = colorEfecto;
        yield return new WaitForSeconds(duracionParpadeo);
        spriteRenderer.color = Color.white;
        recibiendoDaño = false;
    }
}
