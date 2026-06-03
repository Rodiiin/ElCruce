using System.Collections;
using UnityEngine;

public class TriggerEntradaBoss : MonoBehaviour
{
    public IntroduccionBoss introBoss;
    public GameObject bloqueoPuerta;
    public float delayBloqueo = 1.5f; // segundos antes de bloquear

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
            StartCoroutine(ActivarBloqueo());
        }
    }

    private IEnumerator ActivarBloqueo()
    {
        yield return new WaitForSeconds(delayBloqueo);
        if (bloqueoPuerta != null) bloqueoPuerta.SetActive(true);
    }
}