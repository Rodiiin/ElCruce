using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaAditiva : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string nombreSubescena; 

    [Header("¿Qué hace esta puerta?")]
    public bool esParaEntrar = true;

    [Header("Configuración de Zoom (Cámara)")]
    [Tooltip("Qué tan cerca se ve la cámara dentro de la casa")]
    public float zoomInterior = 6f;
    [Tooltip("El zoom normal que usas en la calle/exterior")]
    public float zoomExterior = 10f;

    [Header("Referencias de Personajes (Opcional en Inspector)")]
    public GameObject nino;
    public GameObject nina;

    // Candado global para evitar el bucle infinito de entrar y salir al mismo tiempo
    private static bool transitando = false;

    void Awake()
    {
        transitando = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si ya estamos viajando, ignoramos cualquier otro choque inmediato
        if (transitando) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player2"))
        {
            AsignarNiñosSiFaltan();
            // Iniciamos la nueva corrutina segura
            StartCoroutine(ProcesoTeletransporteConSeguridad());
        }
    }

    private void AsignarNiñosSiFaltan()
    {
        if (GestorJugadores.Instancia != null)
        {
            if (nino == null) nino = GestorJugadores.Instancia.nino;
            if (nina == null) nina = GestorJugadores.Instancia.nina;
        }
    }

    private IEnumerator ProcesoTeletransporteConSeguridad()
    {
        transitando = true; // Cerramos el candado al iniciar el viaje

        // Ejecutamos todo el proceso de carga/descarga y teletransporte
        yield return StartCoroutine(ProcesoTeletransporte());

        // Esperamos medio segundo en la nueva posición antes de volver a activar las puertas
        yield return new WaitForSeconds(0.5f); 

        transitando = false; // Abrimos el candado, listos para otro viaje
    }

    private IEnumerator ProcesoTeletransporte()
    {
        // Buscamos el script de la cámara en la Main Camera
        CamaraCompartida2D scriptCamara = Camera.main.GetComponent<CamaraCompartida2D>();

        if (esParaEntrar)
        {
            // 1. Cargar la casa de forma aditiva
            if (!SceneManager.GetSceneByName(nombreSubescena).isLoaded)
            {
                AsyncOperation cargaAsincrona = SceneManager.LoadSceneAsync(nombreSubescena, LoadSceneMode.Additive);
                while (!cargaAsincrona.isDone)
                {
                    yield return null;
                }
            }
            
            // 2. Buscar el punto vacío que pusiste en el centro de tu subescena
            GameObject puntoCasa = GameObject.Find("Camaracasa1");
            if (puntoCasa != null)
            {
                if (scriptCamara != null)
                {
                    // Clavamos la cámara en la posición de ese punto central
                    scriptCamara.posicionFijaInterior = puntoCasa.transform.position;
                    scriptCamara.esFija = true;
                }

                // Ajustamos el zoom de la Main Camera al tamaño de interiores
                Camera.main.orthographicSize = zoomInterior;

                // Movemos a los niños a la casa
                MoverPersonajes(puntoCasa.transform.position);
            }
            else
            {
                Debug.LogError("No se encontró el objeto 'Camaracasa1' en la subescena.");
            }
        }
        else
        {
            // 1. Buscar el punto de salida en el patio
            GameObject puntoPatio = GameObject.Find("PuntoPatioTrasero");
            
            if (puntoPatio != null)
            {
                if (scriptCamara != null)
                {
                    // Soltamos la cámara para que vuelva a seguir a los niños de forma normal
                    scriptCamara.esFija = false;
                }

                // Regresamos el zoom de la Main Camera al tamaño original de la calle
                Camera.main.orthographicSize = zoomExterior;

                // Movemos a los niños de regreso al patio exterior
                MoverPersonajes(puntoPatio.transform.position);
            }
            else
            {
                Debug.LogError("No se encontró el objeto 'PuntoPatioTrasero' en el mapa principal.");
            }

            // 2. Descargamos la casa de la memoria
            if (SceneManager.GetSceneByName(nombreSubescena).isLoaded)
            {
                SceneManager.UnloadSceneAsync(nombreSubescena);
            }
        }
    }

    private void MoverPersonajes(Vector3 posicionDestino)
    {
        if (nino != null) nino.transform.position = posicionDestino;
        if (nina != null) nina.transform.position = posicionDestino + new Vector3(1f, 0f, 0f); 
    }
}