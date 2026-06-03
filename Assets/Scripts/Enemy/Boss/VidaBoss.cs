using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VidaBoss : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    public int vidaActual;

    [Header("UI - Barra de vida")]
    public Slider barraVida;         // Arrastra aquí tu Slider
    public GameObject panelBarraBoss; // El panel que contiene la barra (para ocultarla antes del combate)

    [Header("Fases")]
    [HideInInspector] public int faseActual = 1;

    [Header("Efectos")]
    public float duracionParpadeo = 0.15f;
    private SpriteRenderer sr;
    private Animator animator;
    private bool recibiendoDanio = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        vidaActual = vidaMaxima;
    }

    void Start()
    {
        // La barra empieza oculta hasta que empiece la pelea
        if (panelBarraBoss != null) panelBarraBoss.SetActive(false);

        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaMaxima;
        }
    }

    public void MostrarBarra()
    {
        if (panelBarraBoss != null) panelBarraBoss.SetActive(true);
    }

    public void RecibirDanio(int cantidad)
    {
        if (recibiendoDanio) return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Max(vidaActual, 0);

        // Actualizar barra
        if (barraVida != null)
            barraVida.value = vidaActual;

        // Parpadeo
        StartCoroutine(Parpadeo());

        // Revisar fases
        ActualizarFase();

        if (vidaActual <= 0)
            Morir();
    }

    private void ActualizarFase()
    {
        float porcentaje = (float)vidaActual / vidaMaxima;

        if (porcentaje <= 0.33f && faseActual < 3)
        {
            faseActual = 3;
            Debug.Log("FASE 3");
        }
        else if (porcentaje <= 0.66f && faseActual < 2)
        {
            faseActual = 2;
            Debug.Log("FASE 2");
        }
    }

    private void Morir()
    {
        Debug.Log("BOSS MUERTO");
        if (animator != null) animator.SetTrigger("Death");
        // Aquí después conectamos el fin de la pelea
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