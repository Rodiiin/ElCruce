using System.Collections;
using UnityEngine;

// Pon este script en un GameObject vacío con Box Collider 2D marcado como "Is Trigger"
// Colócalo en el umbral de la puerta del boss
public class TriggerEntradaBoss : MonoBehaviour
{
    [Header("Referencia al Boss")]
    public IntroduccionBoss introBoss;

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger tocado por: " + other.gameObject.name + " tag: " + other.tag);
    
    if (activado) return;
    if (other.CompareTag("Player") || other.CompareTag("Player2"))
    {
        activado = true;
        introBoss.IniciarSecuenciaEntrada();
    }
    }
}