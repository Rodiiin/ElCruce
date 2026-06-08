using UnityEngine;

// Pon este script en un GameObject hijo del jugador
// que tenga un Collider2D marcado como Is Trigger
// Ese objeto es el hitbox del ataque
public class HitboxAtaque : MonoBehaviour
{
    [Header("Daño")]
    public int danio = 10;

    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        // El hitbox empieza desactivado
        DesactivarHitbox();
    }

    public void ActivarHitbox()
    {
        if (col != null) col.enabled = true;
    }

    public void DesactivarHitbox()
    {
        if (col != null) col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            VidaBoss vida = other.GetComponent<VidaBoss>();
            if (vida != null)
                vida.RecibirDanio(danio);
        }
    }
}