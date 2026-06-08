using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Jugadores")]
    public GameObject jugador1;
    public GameObject jugador2;

    [Header("Spawns iniciales (si no hay checkpoint)")]
    public Transform spawnInicialJ1;
    public Transform spawnInicialJ2;

    void Awake()
    {
        if (Checkpoint.checkpointActivado)
        {
            if (jugador1 != null && Checkpoint.posicionGuardadaJ1.HasValue)
                MoverJugador(jugador1, Checkpoint.posicionGuardadaJ1.Value);

            if (jugador2 != null && Checkpoint.posicionGuardadaJ2.HasValue)
                MoverJugador(jugador2, Checkpoint.posicionGuardadaJ2.Value);
        }
        else
        {
            if (jugador1 != null && spawnInicialJ1 != null)
                MoverJugador(jugador1, spawnInicialJ1.position);

            if (jugador2 != null && spawnInicialJ2 != null)
                MoverJugador(jugador2, spawnInicialJ2.position);
        }
    }

    private void MoverJugador(GameObject jugador, Vector3 destino)
    {
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity        = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        jugador.transform.position = destino;

        if (rb != null)
        {
            rb.velocity        = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}