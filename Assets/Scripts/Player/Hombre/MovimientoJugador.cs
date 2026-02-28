using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
   // Variables para ajustar desde Unity
    public float velocidad = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private float mover;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Depuración: Si esto sale en la consola, alguno de los componentes no se encontró
        if (rb == null) Debug.LogError("No se encontró Rigidbody2D en los hijos");
        if (animator == null) Debug.LogError("No se encontró Animator en los hijos");
        if (spriteRenderer == null) Debug.LogError("No se encontró SpriteRenderer en los hijos");
    }

    void Update()
    {

        // Detecta -1, 0 o 1
        mover = Input.GetAxisRaw("Horizontal");

        // Usamos Mathf.Abs para que si vas a la izquierda (-1), el valor sea 1 positivo
        if (animator != null) {
            animator.SetFloat("Speed", Mathf.Abs(mover));
        }

       // --- LÓGICA DE GIRO SEGURA ---
        if (mover > 0) {
            // Mirar a la derecha
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
        else if (mover < 0) {
            // Mirar a la izquierda (voltear imagen)
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        //float mover = Input.GetAxis("Horizontal");

        // Aplicamos el movimiento físico
        if (mover != 0) {
            rb.velocity = new Vector2(mover * velocidad, rb.velocity.y);
        } else {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
}
