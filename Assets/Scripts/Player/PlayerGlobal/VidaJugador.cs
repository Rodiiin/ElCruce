using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración Vidas")]
    public int vidasMaximas = 3;
    public int vidasActuales;
    private bool estaMuerto = false;
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
        FindObjectOfType<ControladorVidaUI>().ActualizarCorazones(vidasActuales, true);
        
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
        Debug.Log(gameObject.name + " curado. Vida: " + vidasActuales);
        StartCoroutine(Parpadeo(Color.green));
    }

    private void Morir()
    {
        estaMuerto = true;
        if (animator != null) animator.SetTrigger("Death");

        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");

        // 1. DETENER EL MOVIMIENTO FÍSICO INMEDIATAMENTE
        if (rb != null)
        {
            // 1. CONGELAMOS SOLO EL EJE X (para que no se deslice)
        // Mantenemos la velocidad actual de Y para que si estaba cayendo, siga cayendo
        rb.velocity = new Vector2(0, rb.velocity.y);

        // 2. ASEGURARNOS DE QUE SEA DINÁMICO
        // Si estaba en Static o Kinematic, no caería. Debe ser Dynamic.
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        // 4. OPCIONAL: Aumentar la escala de gravedad para que caiga "pesado"
        rb.gravityScale = 2f;
        }


        // Ajuste de colisionador para la muerte
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(col.size.x, col.size.y * 0.1f);
            col.offset = new Vector2(col.offset.x, col.offset.y - (col.size.y * 0.9f));
        }

        

        // Desactivamos el script de movimiento (sea cual sea)
        MovimientoUnico scriptMov = GetComponent<MovimientoUnico>();
        if (scriptMov != null)
        {
            scriptMov.enabled = false;
        }

        this.enabled = false;
    }

    private IEnumerator Parpadeo(Color colorEfecto)
    {
        spriteRenderer.color = colorEfecto;
        yield return new WaitForSeconds(duracionParpadeo);
        spriteRenderer.color = Color.white;
        recibiendoDaño = false;
    }
}
