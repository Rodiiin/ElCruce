using System.Collections.Generic;
using UnityEngine;
using System.Collections; // Necesario para las Coroutines

[RequireComponent(typeof(CanvasGroup))] // Obliga a que el objeto tenga un CanvasGroup
public class DesvanecerInstrucciones : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Header("Configuración del Tiempo")]
    public float tiempoEspera = 15f; // Tiempo antes de empezar a desaparecer
    public float duracionDesvanecimiento = 2f; // Cuánto tarda en desaparecer (2 segs)

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Empezamos la cuenta regresiva con una Coroutine
        StartCoroutine(CuentaRegresivaYDesvanecer());
    }

    private IEnumerator CuentaRegresivaYDesvanecer()
    {
        // 1. Esperamos los 15 segundos
        yield return new WaitForSeconds(tiempoEspera);

        // 2. Empezamos el desvanecimiento poco a poco
        float tiempoPasado = 0;
        
        while (tiempoPasado < duracionDesvanecimiento)
        {
            tiempoPasado += Time.deltaTime;
            
            // Calculamos el alpha (va de 1 a 0)
            canvasGroup.alpha = Mathf.Lerp(1, 0, tiempoPasado / duracionDesvanecimiento);
            
            // Esperamos al siguiente frame
            yield return null;
        }

        // 3. Cuando termina, nos aseguramos de que el alpha sea 0
        canvasGroup.alpha = 0;

        // 4. (Opcional) Desactivamos el objeto por completo para que no use recursos
        gameObject.SetActive(false);
    }
}