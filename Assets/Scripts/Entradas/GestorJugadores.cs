using UnityEngine;

public class GestorJugadores : MonoBehaviour
{
    // Instancia pública para que cualquier script del juego pueda consultarla
    public static GestorJugadores Instancia;

    [Header("Referencias Globales")]
    public GameObject nino;
    public GameObject nina;

    void Awake()
    {
        // Configuramos el Singleton
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}