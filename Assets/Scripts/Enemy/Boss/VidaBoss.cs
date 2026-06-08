using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VidaBoss : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    public int vidaActual;

    [Header("UI - Barra de vida")]
    public Slider barraVida;
    public GameObject panelBarraBoss;

    [Header("Muerte y Salida")]
    public DialogoBoss dialogoFinal;
    public Transform puntoSalida;
    public float velocidadSalida = 4f;

    [Header("Victoria")]
    public PantallaVictoria pantallaVictoria;

    [Header("Fases")]
    [HideInInspector] public int faseActual = 1;

    [Header("Efectos")]
    public float duracionParpadeo = 0.15f;

    private SpriteRenderer sr;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private BossFase1 bossIA;
    private bool recibiendoDanio = false;
    private bool muerto = false;

    void Awake()
    {
        sr       = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();
        col      = GetComponent<Collider2D>();
        bossIA   = GetComponent<BossFase1>();
        vidaActual = vidaMaxima;
    }

    void Start()
    {
        if (panelBarraBoss != null) panelBarraBoss.SetActive(false);
        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value    = vidaMaxima;
        }
    }

    public void MostrarBarra()
    {
        if (panelBarraBoss != null) panelBarraBoss.SetActive(true);
    }

    public void RecibirDanio(int cantidad)
    {
        if (recibiendoDanio || muerto) return;

        vidaActual -= cantidad;
        vidaActual  = Mathf.Max(vidaActual, 0);

        if (barraVida != null) barraVida.value = vidaActual;

        StartCoroutine(Parpadeo());
        ActualizarFase();

        if (vidaActual <= 0) Morir();
    }

    private void ActualizarFase()
    {
        float pct = (float)vidaActual / vidaMaxima;
        if      (pct <= 0.33f && faseActual < 3) faseActual = 3;
        else if (pct <= 0.66f && faseActual < 2) faseActual = 2;
    }

    private void Morir()
    {
        muerto = true;
        if (bossIA != null) bossIA.OnMuerte();
        StartCoroutine(SecuenciaMuerte());
    }

    private IEnumerator SecuenciaMuerte()
    {
        // 1. Detener al boss
        if (rb != null) rb.velocity = Vector2.zero;
        if (animator != null) animator.SetBool("isWalking", false);

        // 2. Pausa dramática
        yield return new WaitForSeconds(0.5f);

        // 3. Ocultar barra antes del diálogo
        if (panelBarraBoss != null) panelBarraBoss.SetActive(false);

        // 4. Diálogo final
        if (dialogoFinal != null)
        {
            dialogoFinal.IniciarDialogoAuto();
            yield return new WaitUntil(() => dialogoFinal.DialogoTerminado());
        }

        // 5. Quitar collider para atravesar jugadores
        if (col != null) col.enabled = false;

        // 6. Boss camina hacia el punto de salida
        if (animator != null) animator.SetBool("isWalking", true);
        sr.flipX = true;

        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        float destinoX = puntoSalida != null ?
            puntoSalida.position.x :
            transform.position.x - 20f; // fallback si no hay punto

        while (transform.position.x > destinoX)
        {
            rb.velocity = new Vector2(-velocidadSalida, 0f);
            yield return null;
        }

        // 7. Desaparecer
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);

        // 8. Pantalla de victoria
        if (pantallaVictoria != null)
            pantallaVictoria.MostrarVictoria();
    }

    private IEnumerator Parpadeo()
    {
        recibiendoDanio = true;
        if (sr != null) sr.color = Color.red;
        yield return new WaitForSeconds(duracionParpadeo);
        if (sr != null) sr.color = Color.white;
        recibiendoDanio = false;
    }
}