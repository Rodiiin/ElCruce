using System.Collections;
using UnityEngine;

public class TriggerEntradaBoss : MonoBehaviour
{
    [Header("Referencias")]
    public IntroduccionBoss introBoss;
    public GameObject bloqueoPuerta;

    private bool activado = false;

    void Start()
    {
        if (bloqueoPuerta != null) bloqueoPuerta.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            activado = true;
            introBoss.IniciarSecuenciaEntrada();
            StartCoroutine(EsperarJugadoresYBloquear());
        }
    }

    private IEnumerator EsperarJugadoresYBloquear()
    {
        yield return new WaitUntil(() => introBoss.IntroTerminada());
        if (bloqueoPuerta != null) bloqueoPuerta.SetActive(true);
    }
}