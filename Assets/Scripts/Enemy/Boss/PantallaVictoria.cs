using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PantallaVictoria : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup fondoNegro;
    public TextMeshProUGUI textoVictoria;
    public TextMeshProUGUI textoContinuar;

    [Header("Configuración")]
    public float velocidadFade = 1.5f;
    public float delayTexto    = 0.5f;
    public string escenaSiguiente = "";

    void Start()
    {
        if (fondoNegro != null)
            fondoNegro.alpha = 0f;
        if (textoVictoria  != null) textoVictoria.alpha  = 0f;
        if (textoContinuar != null) textoContinuar.alpha = 0f;
    }

    public void MostrarVictoria()
    {
        StartCoroutine(SecuenciaVictoria());
    }

    private IEnumerator SecuenciaVictoria()
    {
        BloquearJugadores(true);

        // 1. Fade a negro
        if (fondoNegro != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * velocidadFade;
                fondoNegro.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(delayTexto);

        // 2. Fade in texto principal
        if (textoVictoria != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * velocidadFade;
                textoVictoria.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.8f);

        // 3. Fade in "presiona E"
        if (textoContinuar != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * velocidadFade;
                textoContinuar.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }

        // 4. Esperar input
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

        // 5. Siguiente escena o quedarse
        if (!string.IsNullOrEmpty(escenaSiguiente))
            SceneManager.LoadScene(escenaSiguiente);
    }

    private void BloquearJugadores(bool bloquear)
    {
        GameObject j1 = GameObject.FindGameObjectWithTag("Player");
        GameObject j2 = GameObject.FindGameObjectWithTag("Player2");

        if (j1 != null)
        {
            var mov = j1.GetComponent<MovimientoUnico>();
            if (mov != null) mov.enabled = !bloquear;
        }
        if (j2 != null)
        {
            var mov2 = j2.GetComponent<MovimientoJugador2>();
            if (mov2 != null) mov2.enabled = !bloquear;
        }
    }
}