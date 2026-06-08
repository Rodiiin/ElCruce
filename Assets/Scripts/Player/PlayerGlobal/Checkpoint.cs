using UnityEngine;

// Pon este script en un GameObject vacío con Box Collider 2D marcado como Is Trigger
// Colócalo donde quieres que sea el checkpoint
public class Checkpoint : MonoBehaviour
{
    [Header("Posiciones de spawn")]
    public Transform spawnJugador1;  // Punto donde aparece el jugador 1
    public Transform spawnJugador2;  // Punto donde aparece el jugador 2

    // Guardamos las posiciones en estáticos para que sobrevivan al reload
    public static Vector3? posicionGuardadaJ1 = null;
    public static Vector3? posicionGuardadaJ2 = null;
    public static bool checkpointActivado = false;

    private bool yaActivado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (yaActivado) return;
        if (!other.CompareTag("Player") && !other.CompareTag("Player2")) return;

        yaActivado = true;
        checkpointActivado = true;

        posicionGuardadaJ1 = spawnJugador1 != null ? spawnJugador1.position : (Vector3?)null;
        posicionGuardadaJ2 = spawnJugador2 != null ? spawnJugador2.position : (Vector3?)null;

        Debug.Log("Checkpoint activado");
    }

    // Gizmo para verlo en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = yaActivado ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>() != null
            ? (Vector3)GetComponent<Collider2D>().bounds.size
            : Vector3.one);

        if (spawnJugador1 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(spawnJugador1.position, 0.3f);
        }
        if (spawnJugador2 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnJugador2.position, 0.3f);
        }
    }
}