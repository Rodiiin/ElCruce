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

    [Header("Victoria")]
    public PantallaVictoria pantallaVictoria;

    [Header("Fases")]
    [HideInInspector] public int faseActual = 1;

    [Header("Efectos")]
    public float duracionParpadeo = 0.15f;

    private SpriteRenderer sr;
    private Animator animator;
    private BossFase1 bossIA;
    private bool recibiendoDanio = false;
    private bool muerto = false;

    void Awake()
    {
        sr       = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        // 1. Reproducir animación de muerte UNA sola vez
        if (animator != null)
        {
            animator.SetTrigger("Death");
            // Esperar a que la animación termine
            yield return null; // un frame para que el trigger se aplique
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            // Esperar duración de la animación de muerte
            yield return new WaitForSeconds(info.length);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        // 2. Mostrar pantalla de victoria
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