using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float offsetMultiplier = 1f;
    public float smoothTime = .3f;

    private Vector2 startPosition;
    private Vector3 velocity;
    
    void Start()
    {
        startPosition = transform.position;
    }

        void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        
        // Ajustamos para que el centro (0.5) sea el punto 0
        Vector2 offset = new Vector2(mousePos.x - 0.5f, mousePos.y - 0.5f);

        // Calculamos la posicion destino
        Vector3 targetPosition = new Vector3(
            startPosition.x + (offset.x * offsetMultiplier),
            startPosition.y + (offset.y * offsetMultiplier),
            transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    
    }
}
