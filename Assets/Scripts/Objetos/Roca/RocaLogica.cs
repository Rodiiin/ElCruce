using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocaLogica : MonoBehaviour
{
    private Vector3 posicionOriginal;
    private Rigidbody2D rb;

    void Start()
    {
        posicionOriginal = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    // Esta función la llamaremos desde el jugador después de lanzar
    public void IniciarRespawn(float tiempo)
    {
        StartCoroutine(RespawnCorrutina(tiempo));
    }

    IEnumerator RespawnCorrutina(float tiempo)
    {
        // Esperamos el tiempo (3 segundos por ejemplo)
        yield return new WaitForSeconds(tiempo);

        // Devolvemos la roca a su sitio
        transform.SetParent(null); // Por si acaso
        transform.position = posicionOriginal;
        
        // Resetear físicas
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true; // Para que el jugador pueda volver a tocarla
    }
}