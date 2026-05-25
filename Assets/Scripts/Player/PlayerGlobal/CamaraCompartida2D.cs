using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraCompartida2D : MonoBehaviour
{
    [Header("Personajes")]
    public GameObject nino;
    public GameObject nina;

    [Header("Configuración de Cámara")]
    public float suavizado = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("Distancia Cooperativa")]
    public float distanciaMaxima = 12f; // Ajusta este número según el tamaño (Size) de tu cámara

    private VidaJugador scriptVidaNino;
    private VidaJugador2 scriptVidaNina;

    void Start()
    {
        if (nino != null) scriptVidaNino = nino.GetComponent<VidaJugador>();
        if (nina != null) scriptVidaNina = nina.GetComponent<VidaJugador2>();
    }

    void LateUpdate()
    {
        if (nino == null || nina == null) return;

        bool ninoVivo = (scriptVidaNino != null && !scriptVidaNino.estaMuerto);
        bool ninaViva = (scriptVidaNina != null && !scriptVidaNina.estaMuerto);

        Vector3 targetPosition = transform.position;

        // CASO 1: Ambos vivos (Aquí viene la magia del freno)
        if (ninoVivo && ninaViva)
        {
            // Calculamos la distancia real entre los dos niños en el eje X
            float distanciaActual = Mathf.Abs(nino.transform.position.x - nina.transform.position.x);

            // Si se están alejando más de la cuenta, la cámara NO avanza más, obligándolos a juntarse
            if (distanciaActual > distanciaMaxima)
            {
                // Mantenemos la posición actual de la cámara en X, solo suavizamos el eje Y por si saltan
                targetPosition = new Vector3(transform.position.x, ((nino.transform.position.y + nina.transform.position.y) / 2f) + offset.y, offset.z);
            }
            else
            {
                // Si están a buena distancia, los seguimos en el punto medio normal
                Vector3 puntoMedio = (nino.transform.position + nina.transform.position) / 2f;
                targetPosition = puntoMedio + offset;
            }
        }
        // CASO 2: El niño murió -> Sigue a la niña libremente
        else if (!ninoVivo && ninaViva)
        {
            targetPosition = nina.transform.position + offset;
        }
        // CASO 3: La niña murió -> Sigue al niño libremente
        else if (ninoVivo && !ninaViva)
        {
            targetPosition = nino.transform.position + offset;
        }

        // Movimiento fluido
        transform.position = Vector3.Lerp(transform.position, targetPosition, suavizado);
    }
}