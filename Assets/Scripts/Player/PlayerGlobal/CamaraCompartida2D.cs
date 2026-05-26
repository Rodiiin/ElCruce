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
    public float distanciaMaxima = 12f; 

    [Header("Modo Interior (Fijo)")]
    public bool esFija = false;
    public Vector3 posicionFijaInterior;

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

        // Revisamos si los niños están vivos
        bool ninoVivo = (scriptVidaNino != null && !scriptVidaNino.estaMuerto);
        bool ninaViva = (scriptVidaNina != null && !scriptVidaNina.estaMuerto);

        Vector3 targetPosition = transform.position;

        // ==========================================
        // CASO INTERIOR: La pantalla se queda congelada
        // ==========================================
        if (esFija)
        {
            // La cámara ignora a los niños y apunta directo al centro de la casa
            targetPosition = posicionFijaInterior + offset;
        }
        // ==========================================
        // CASOS EXTERIORES: Seguimiento normal
        // ==========================================
        else
        {
            // Ambos niños vivos
            if (ninoVivo && ninaViva)
            {
                float distanciaActual = Mathf.Abs(nino.transform.position.x - nina.transform.position.x);

                // Si se alejan demasiado, la cámara se planta y no avanza en X
                if (distanciaActual > distanciaMaxima)
                {
                    targetPosition = new Vector3(transform.position.x, ((nino.transform.position.y + nina.transform.position.y) / 2f) + offset.y, offset.z);
                }
                else
                {
                    Vector3 puntoMedio = (nino.transform.position + nina.transform.position) / 2f;
                    targetPosition = puntoMedio + offset;
                }
            }
            // Solo la niña está viva
            else if (!ninoVivo && ninaViva)
            {
                targetPosition = nina.transform.position + offset;
            }
            // Solo el niño está vivo
            else if (ninoVivo && !ninaViva)
            {
                targetPosition = nino.transform.position + offset;
            }
        }

        // Movimiento fluido de la cámara hacia su destino
        transform.position = Vector3.Lerp(transform.position, targetPosition, suavizado);
    }
}