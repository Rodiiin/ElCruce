using System.Collections;
using UnityEngine;
using TMPro;

public class IntroduccionBoss : MonoBehaviour
{
    [Header("Jugadores")]
    public GameObject nino;
    public GameObject nina;

    [Header("Punto de parada de los jugadores")]
    public Transform puntoParadaNino;
    public Transform puntoParadaNina;

    [Header("Cámara")]
    public CamaraCompartida2D scriptCamara;
    public float zoomIntro = 4.5f;
    public float zoomNormal = 5f;

    [Header("Diálogo")]
    public DialogoBoss manejadorDialogo;

    [Header("UI - Nombre del Boss")]
    public CanvasGroup panelNombreBoss;
    public TextMeshProUGUI textoNombreBoss;
    public float duracionNombre = 2.5f;

    [Header("Velocidad de caminar en la cinemática")]
    public float velocidadCaminata = 3f;

    private Camera camPrincipal;
    private bool introTerminada = false;

    void Awake()
    {
        camPrincipal = Camera.main;
        if (panelNombreBoss != null)
        {
            panelNombreBoss.alpha = 0f;
            panelNombreBoss.gameObject.SetActive(false);
        }
    }

    public void IniciarSecuenciaEntrada()
    {
        StartCoroutine(SecuenciaCompleta());
    }

    private IEnumerator SecuenciaCompleta()
    {
        // 1. Bloquear jugadores y desactivar cámara
        BloquearJugadores(true);
        if (scriptCamara != null) scriptCamara.enabled = false;

        // 2. Jugadores caminan a sus puntos
        yield return StartCoroutine(CaminarJugadores());

        // 3. Pausa dramática
        yield return new WaitForSeconds(0.5f);

        // 4. Cámara se mueve al boss con zoom
        yield return StartCoroutine(MoverCamaraAlBoss(zoomIntro));

        // 5. Diálogo
        if (manejadorDialogo != null)
            manejadorDialogo.IniciarDialogoAuto();

        yield return StartCoroutine(EsperarDialogo());

        // 6. Restaurar zoom ANTES del título
        yield return StartCoroutine(MoverCamaraAlBoss(zoomNormal));
        if (scriptCamara != null) scriptCamara.enabled = true;

        // 7. Nombre del boss
        yield return StartCoroutine(AnimarNombreBoss());

        // 8. Devolver control
        BloquearJugadores(false);
        introTerminada = true;
        Debug.Log("INTRO TERMINADA");
    }

    private IEnumerator CaminarJugadores()
    {
        bool ninoListo = (puntoParadaNino == null);
        bool ninaLista = (puntoParadaNina == null);

        while (!ninoListo || !ninaLista)
        {
            if (!ninoListo && nino != null)
            {
                Vector2 dir = puntoParadaNino.position - nino.transform.position;
                nino.transform.position = Vector2.MoveTowards(
                    nino.transform.position, puntoParadaNino.position,
                    velocidadCaminata * Time.deltaTime);
                SpriteRenderer sr = nino.GetComponent<SpriteRenderer>();
                if (sr != null) sr.flipX = dir.x > 0;
                Animator anim = nino.GetComponent<Animator>();
                if (anim != null) anim.SetFloat("Speed", 1f);
                if (Vector2.Distance(nino.transform.position, puntoParadaNino.position) < 0.05f)
                {
                    ninoListo = true;
                    if (anim != null) anim.SetFloat("Speed", 0f);
                }
            }

            if (!ninaLista && nina != null)
            {
                Vector2 dir = puntoParadaNina.position - nina.transform.position;
                nina.transform.position = Vector2.MoveTowards(
                    nina.transform.position, puntoParadaNina.position,
                    velocidadCaminata * Time.deltaTime);
                SpriteRenderer sr = nina.GetComponent<SpriteRenderer>();
                if (sr != null) sr.flipX = dir.x > 0;
                Animator anim = nina.GetComponent<Animator>();
                if (anim != null) anim.SetFloat("Speed", 1f);
                if (Vector2.Distance(nina.transform.position, puntoParadaNina.position) < 0.05f)
                {
                    ninaLista = true;
                    if (anim != null) anim.SetFloat("Speed", 0f);
                }
            }

            if (camPrincipal != null && nino != null && nina != null)
            {
                Vector3 medio = (nino.transform.position + nina.transform.position) / 2f;
                medio.z = camPrincipal.transform.position.z;
                camPrincipal.transform.position = Vector3.Lerp(
                    camPrincipal.transform.position, medio, 5f * Time.deltaTime);
            }

            yield return null;
        }
    }

    // Mueve la cámara al boss Y hace zoom al targetSize en 1.5 segundos
    private IEnumerator MoverCamaraAlBoss(float targetSize)
    {
        if (camPrincipal == null) yield break;

        Vector3 destino = new Vector3(
            transform.position.x,
            transform.position.y,
            camPrincipal.transform.position.z);

        float tiempoTotal = 1.5f;
        float tiempoActual = 0f;
        Vector3 posInicial = camPrincipal.transform.position;
        float zoomInicial = camPrincipal.orthographicSize;

        while (tiempoActual < tiempoTotal)
        {
            tiempoActual += Time.deltaTime;
            float t = tiempoActual / tiempoTotal;
            t = t * t * (3f - 2f * t); // curva suave
            camPrincipal.transform.position = Vector3.Lerp(posInicial, destino, t);
            camPrincipal.orthographicSize = Mathf.Lerp(zoomInicial, targetSize, t);
            yield return null;
        }

        camPrincipal.transform.position = destino;
        camPrincipal.orthographicSize = targetSize;
    }

    private IEnumerator EsperarDialogo()
    {
        if (manejadorDialogo == null) yield break;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => manejadorDialogo.DialogoTerminado());
    }

    private IEnumerator AnimarNombreBoss()
    {
        if (panelNombreBoss == null) yield break;
        panelNombreBoss.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            panelNombreBoss.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        yield return new WaitForSeconds(duracionNombre);

        while (t > 0f)
        {
            t -= Time.deltaTime * 1.5f;
            panelNombreBoss.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        panelNombreBoss.gameObject.SetActive(false);
    }

    private void BloquearJugadores(bool bloquear)
    {
        if (nino != null)
        {
            var mov = nino.GetComponent<MovimientoUnico>();
            if (mov != null) mov.enabled = !bloquear;
        }
        if (nina != null)
        {
            var mov2 = nina.GetComponent<MovimientoJugador2>();
            if (mov2 != null) mov2.enabled = !bloquear;
        }
    }

    public bool IntroTerminada() => introTerminada;
}