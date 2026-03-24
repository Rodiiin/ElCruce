using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vida : MonoBehaviour
{
    [Header("Configuración Vidas")]
    public int vidasMaximas = 3;
    public int vidasActuales;
    private bool estaMuerto = false;
    private bool recibiendoDaño = false;

    [Header("Efectos Visuales")]
    public float duracionParpadeo = 0.5f;
    public float fuerzaImpulso = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
}
