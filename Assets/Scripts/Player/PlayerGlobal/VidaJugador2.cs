using System.Collections;
using UnityEngine;

public class VidaJugador2 : MonoBehaviour
{
    [Header("Configuración Vidas")]
    public int vidasMaximas = 3;
    public int vidasActuales;
    public bool estaMuerto = false;
    public bool recibiendoDaño = false;

    [Header("Reanimación")]
    public int reanimacionesMaximas = 2;
    [HideInInspector] public int reanimacionesUsadas = 0;

    [Header("Efectos Visuales")]
    public float duracionParpadeo = 0.5f;
    public float fuerzaImpulso = 5f;
    public float duracionInmunidad = 2f;

    [HideInInspector] public Vector2 colSizeOriginal;
    [HideInInspector] public Vector2 colOffsetOriginal;
    [HideInInspector] public float gravedadOriginal;
    [HideInInspector] public bool esInmune = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        vidasActuales = vidasMaximas;

        if (col != null)
        {
            colSizeOriginal = col.size;
            colOffsetOriginal = col.offset;
        }
        if (rb != null)
            gravedadOriginal = rb.gravityScale;
    }

    public void RecibirDaño(Vector2 direccionGolpe)
    {
        if (recibiendoDaño || estaMuerto|| esInmune) return;

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
        FindObjectOfType<ControladorVidaUI>().ActualizarCorazones(vidasActuales, false);
        StartCoroutine(Parpadeo(Color.green));
    }

    public bool PuedeSerReanimado()
    {
        return reanimacionesUsadas < reanimacionesMaximas;
    }

    public void Reanimar()
    {
        reanimacionesUsadas++;

        estaMuerto = false;
        recibiendoDaño = false;
        vidasActuales = 1;

        if (col != null)
        {
            col.enabled = true;
            col.size = colSizeOriginal;
            col.offset = colOffsetOriginal;
        }

        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = gravedadOriginal;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        gameObject.layer = LayerMask.NameToLayer("Player");

        MovimientoJugador2 mov = GetComponent<MovimientoJugador2>();
        if (mov != null) mov.enabled = true;


        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        ControladorVidaUI ui = FindObjectOfType<ControladorVidaUI>();
        if (ui != null)
        {
            ui.ActualizarCorazones(1, false);
            ui.RestaurarIcono(false);
        }

        StartCoroutine(InmunidadVisual());
    }

    private void Morir()
    {
        estaMuerto = true;
        if (animator != null) animator.SetTrigger("Death");

        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");

        if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.gravityScale = 2f;
        }

        if (col != null)
        {
            col.size = new Vector2(col.size.x, colSizeOriginal.y * 0.3f);
            col.offset = new Vector2(col.offset.x, colOffsetOriginal.y - (colSizeOriginal.y * 0.1f));
        }

        MovimientoJugador2 mov = GetComponent<MovimientoJugador2>();
        if (mov != null) mov.enabled = false;

    }

    private IEnumerator Parpadeo(Color colorEfecto)
    {
        spriteRenderer.color = colorEfecto;
        yield return new WaitForSeconds(duracionParpadeo);
        spriteRenderer.color = Color.white;
        recibiendoDaño = false;
    }
        private IEnumerator InmunidadVisual()
    {
        esInmune = true; // inmune al daño
        // recibiendoDaño NO se toca, así el movimiento funciona normal
    
        float tiempoPasado = 0f;
        while (tiempoPasado < duracionInmunidad)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            tiempoPasado += 0.2f;
        }

        spriteRenderer.color = Color.white;
        esInmune = false;
    }
}