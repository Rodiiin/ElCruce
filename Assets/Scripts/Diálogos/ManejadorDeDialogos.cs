using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ManejadorDeDialogos : MonoBehaviour
{
    public TextMeshProUGUI textoDialogo; // Arrastra aquí el objeto "Dialogo"
    public GameObject panelDialogo;      // Arrastra aquí "Dialogo_Panel"
    
    [TextArea(3, 5)]
    public string[] frases;              // Aquí escribirás la historia
    
    private int index;
    public float velocidadLetra = 0.05f;
    public GameObject objetoSalida;
    
    void Start()
    {
        // 1. Al empezar, nos aseguramos de que el panel esté apagado por si acaso
        panelDialogo.SetActive(false);

        // 2. Programamos la aparición a los 15 segundos
        Invoke("IniciarDialogo", 13f);
    }

    public void IniciarDialogo()
    {
        index = 0;
        // 3. ¡Aquí es donde el panel aparece mágicamente!
        panelDialogo.SetActive(true); 
        StartCoroutine(EscribirFrase());
    }

    IEnumerator EscribirFrase()
    {
        textoDialogo.text = "";
        foreach (char letra in frases[index].ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadLetra);
        }
    }

    void Update()
{
    // Cambiamos el Click/Espacio por la tecla E
    if (Input.GetKeyDown(KeyCode.E))
    {
        // Si el texto ya terminó de escribirse letra por letra
        if (textoDialogo.text == frases[index])
        {
            SiguienteFrase();
        }
        else
        {
            // Si el jugador presiona E mientras se escribe, 
            // mostramos la frase completa de golpe para no hacerlo esperar
            StopAllCoroutines();
            textoDialogo.text = frases[index];
        }
    }
}

    void SiguienteFrase()
    {
        if (index < frases.Length - 1)
        {
            index++;
            StartCoroutine(EscribirFrase());
        }
        else
        {
            TerminarConversacion();
        }
    }

    void TerminarConversacion()
    {
        panelDialogo.SetActive(false);
        // ¡Mágia! Activamos la opción de salir
        if (objetoSalida != null)
        {
            objetoSalida.SetActive(true);
            Debug.Log("Ya pueden salir de la casa");
        }
    }
}
