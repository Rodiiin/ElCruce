using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SistemaRevivir : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoParaRevivir = 3f;
    public float radioRevivir = 2f;

    [Header("UI Barra (World Space)")]
    public GameObject panelRevivir;
    public Image barraProgreso;

    [Header("UI Texto")]
    public TMPro.TextMeshProUGUI textoReviviendo;

    private float tiempoPunto = 0f;
    private int numPuntos = 0;
    private float progresoActual = 0f;

    private VidaJugador vidaJ1;
    private VidaJugador2 vidaJ2;

    void Awake()
    {
        vidaJ1 = GetComponent<VidaJugador>();
        vidaJ2 = GetComponent<VidaJugador2>();
    }

    void Start()
    {
        if (panelRevivir != null) panelRevivir.SetActive(false);
    }

    void Update()
    {

        bool estaMuerto = (vidaJ1 != null && vidaJ1.estaMuerto) ||
                          (vidaJ2 != null && vidaJ2.estaMuerto);

        if (!estaMuerto) { ResetBarra(); return; }

        bool puedeReanimar = (vidaJ1 != null && vidaJ1.PuedeSerReanimado()) ||
                             (vidaJ2 != null && vidaJ2.PuedeSerReanimado());

        if (!puedeReanimar) { ResetBarra(); return; }

        Collider2D jugadorCerca = BuscarJugadorVivoCerca();

        if (jugadorCerca == null)
        {
            progresoActual = 0f;
            MostrarBarra(false);
            return;
        }

        bool presionando = (jugadorCerca.CompareTag("Player") && Input.GetKey(KeyCode.E)) ||
                           (jugadorCerca.CompareTag("Player2") && Input.GetKey(KeyCode.RightControl));

        if (presionando)
        {
            MostrarBarra(true);
            progresoActual += Time.deltaTime;

            // Animación puntos suspensivos
            tiempoPunto += Time.deltaTime;
            if (tiempoPunto >= 0.4f)
            {
                tiempoPunto = 0f;
                numPuntos = (numPuntos + 1) % 4;
            }
            if (textoReviviendo != null)
                textoReviviendo.text = "Reanimando" + new string('.', numPuntos);

            if (barraProgreso != null)
                barraProgreso.fillAmount = progresoActual / tiempoParaRevivir;

            if (progresoActual >= tiempoParaRevivir)
                EjecutarReanimacion();
        }
        else
        {
            progresoActual = 0f;
            tiempoPunto = 0f;
            numPuntos = 0;
            MostrarBarra(false);
        }
    }

    void ActualizarPosicionUI()
    {
        if (panelRevivir == null) return;
        Camera cam = Camera.main;
        if (cam == null) return;
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        panelRevivir.transform.position = screenPos;
    }

    Collider2D BuscarJugadorVivoCerca()
    {
        Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, radioRevivir, ~0);

        foreach (Collider2D col in cercanos)
        {
            if (col.gameObject == this.gameObject) continue;

            if (col.CompareTag("Player") || col.CompareTag("Player2"))
            {
                VidaJugador v1 = col.GetComponent<VidaJugador>();
                VidaJugador2 v2 = col.GetComponent<VidaJugador2>();

                bool vivo = (v1 != null && !v1.estaMuerto) ||
                            (v2 != null && !v2.estaMuerto);

                if (vivo) return col;
            }
        }
        return null;
    }

    void EjecutarReanimacion()
    {
        progresoActual = 0f;
        MostrarBarra(false);

        if (vidaJ1 != null && vidaJ1.estaMuerto && vidaJ1.PuedeSerReanimado())
            vidaJ1.Reanimar();
        else if (vidaJ2 != null && vidaJ2.estaMuerto && vidaJ2.PuedeSerReanimado())
            vidaJ2.Reanimar();
    }

    void MostrarBarra(bool mostrar)
    {
        if (panelRevivir != null) panelRevivir.SetActive(mostrar);
        if (!mostrar && barraProgreso != null) barraProgreso.fillAmount = 0f;
        if (!mostrar && textoReviviendo != null) textoReviviendo.text = "";
    }

    void ResetBarra()
    {
        progresoActual = 0f;
        tiempoPunto = 0f;
        numPuntos = 0;
        MostrarBarra(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioRevivir);
    }
}