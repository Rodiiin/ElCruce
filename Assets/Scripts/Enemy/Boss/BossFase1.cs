using System.Collections;
using UnityEngine;

[RequireComponent(typeof(VidaBoss))]
public class BossFase1 : MonoBehaviour
{
    private Transform jugador1;
    private Transform jugador2;
    private Transform jugadorObjetivo;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private VidaBoss vidaBoss;
    private Collider2D col;
    private float gravedadOriginal;

    [Header("Intro")]
    public IntroduccionBoss introBoss;

    [Header("Movimiento")]
    public float velocidadCaminata = 2.5f;

    [Header("Detección")]
    public float rangoDeteccion = 8f;
    public float rangoAtaque    = 1.5f;

    [Header("Ataque Melee")]
    public int   danioAtaque       = 10;
    public float duracionAtaque    = 1f;
    public float cooldownAtaque    = 0.8f;
    public int   ataquesPorRafaga  = 3;
    public float radioHitboxAtaque = 0.8f;
    public Vector2 offsetHitbox    = new Vector2(1f, 0f);
    [Range(0f, 1f)]
    [Tooltip("En qué punto de la animación (0-1) se aplica el daño. 0.5 = mitad")]
    public float puntoDeImpacto    = 0.5f;

    [Header("Ataque Proyectil - Fase 2")]
    public GameObject proyectilPrefab;
    public int   proyectilesPorRafaga   = 3;
    public float tiempoEntreProyectiles = 0.6f;
    public float offsetYProyectil1      = 0.5f;
    public float offsetYProyectil2      = -0.5f;
    [Tooltip("Cuántos segundos camina hacia el jugador tras disparar antes de volver a decidir")]
    public float duracionCaminataTrasDiparo = 1.5f;

    [Header("Columnas de dagas - Fase 3")]
    public Transform posicionFondo;
    public float velocidadIrAlFondo = 4f;
    public int   columnasPorSerie   = 3;
    public float delayEntreColumnas = 0.8f;
    public float anchoArena         = 12f;
    public float alturaColumna      = 5f;
    public int   seriesPorPausa     = 3;
    public float duracionPausaFase3 = 2.5f;

    [Header("Cansancio")]
    public float duracionCansancio = 2f;

    private enum Estado { Idle, Perseguir, Atacar, Disparar, Columnas, Cansado, Muerto }
    private Estado estadoActual = Estado.Idle;

    private int  ataquesRealizados = 0;
    private bool enCorrutina       = false;
    private bool introYaTermino    = false;
    private bool yaEnFondo         = false;
    private int  seriesLanzadas    = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();
        sr       = GetComponent<SpriteRenderer>();
        vidaBoss = GetComponent<VidaBoss>();
        col      = GetComponent<Collider2D>();
        gravedadOriginal = rb.gravityScale;
    }

    void Start()
    {
        GameObject go1 = GameObject.FindGameObjectWithTag("Player");
        GameObject go2 = GameObject.FindGameObjectWithTag("Player2");
        if (go1 != null) jugador1 = go1.transform;
        if (go2 != null) jugador2 = go2.transform;
        jugadorObjetivo = jugador1;

        if (introBoss != null)
        {
            col.enabled = false;
            rb.bodyType = RigidbodyType2D.Static;
            rb.velocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (estadoActual == Estado.Muerto) return;

        if (introBoss != null && !introBoss.IntroTerminada())
        {
            col.enabled = false;
            rb.bodyType = RigidbodyType2D.Static;
            return;
        }

        if (!introYaTermino)
        {
            introYaTermino  = true;
            col.enabled     = true;
            rb.bodyType     = RigidbodyType2D.Dynamic;
            rb.gravityScale = gravedadOriginal;
            vidaBoss.MostrarBarra();
        }

        if (enCorrutina) return;

        ActualizarObjetivo();
        if (jugadorObjetivo == null) return;

        int fase = vidaBoss.faseActual;

        if (fase == 3 && !yaEnFondo)
        {
            yaEnFondo = true;
            CambiarEstado(Estado.Columnas);
            return;
        }

        float distancia = Vector2.Distance(transform.position, jugadorObjetivo.position);

        switch (estadoActual)
        {
            case Estado.Idle:
                if (distancia <= rangoDeteccion)
                    CambiarEstado(Estado.Perseguir);
                break;

            case Estado.Perseguir:
                if (fase == 3)
                {
                    CambiarEstado(Estado.Columnas);
                }
                else if (distancia <= rangoAtaque)
                {
                    CambiarEstado(Estado.Atacar);
                }
                else if (fase >= 2)
                {
                    // Fase 2: dispara si está lejos, la rutina se encarga de caminar
                    CambiarEstado(Estado.Disparar);
                }
                else
                {
                    // Fase 1: caminar hacia el jugador
                    MoverHaciaObjetivo();
                }
                break;
        }
    }

    // ── HELPERS ───────────────────────────────────────────────────────────────

    private void LimpiarTriggers()
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Cast");
        animator.ResetTrigger("Death");
    }

    private void ActualizarObjetivo()
    {
        bool j1Vivo = jugador1 != null && JugadorEstaVivo(jugador1);
        bool j2Vivo = jugador2 != null && JugadorEstaVivo(jugador2);

        if (j1Vivo && j2Vivo)
        {
            float d1 = Vector2.Distance(transform.position, jugador1.position);
            float d2 = Vector2.Distance(transform.position, jugador2.position);
            jugadorObjetivo = d1 <= d2 ? jugador1 : jugador2;
        }
        else if (j1Vivo)  jugadorObjetivo = jugador1;
        else if (j2Vivo)  jugadorObjetivo = jugador2;
        else              jugadorObjetivo = null;
    }

    private bool JugadorEstaVivo(Transform t)
    {
        VidaJugador  v1 = t.GetComponent<VidaJugador>();
        VidaJugador2 v2 = t.GetComponent<VidaJugador2>();
        if (v1 != null) return v1.vidasActuales > 0;
        if (v2 != null) return !v2.estaMuerto;
        return true;
    }

    // ── ESTADOS ───────────────────────────────────────────────────────────────

    void CambiarEstado(Estado nuevoEstado)
    {
        estadoActual = nuevoEstado;
        switch (nuevoEstado)
        {
            case Estado.Perseguir:
                animator.SetBool("isWalking", true);
                break;
            case Estado.Atacar:
                animator.SetBool("isWalking", false);
                StartCoroutine(RutinaAtaque());
                break;
            case Estado.Disparar:
                animator.SetBool("isWalking", false);
                StartCoroutine(RutinaDisparo());
                break;
            case Estado.Columnas:
                animator.SetBool("isWalking", false);
                StartCoroutine(RutinaColumnas());
                break;
            case Estado.Cansado:
                animator.SetBool("isWalking", false);
                StartCoroutine(RutinaCansancio());
                break;
            case Estado.Idle:
                animator.SetBool("isWalking", false);
                break;
        }
    }

    void MoverHaciaObjetivo()
    {
        animator.SetBool("isWalking", true);
        float dir = jugadorObjetivo.position.x - transform.position.x;
        sr.flipX = dir < 0;
        rb.velocity = new Vector2(Mathf.Sign(dir) * velocidadCaminata, rb.velocity.y);
    }

    // ── CORRUTINAS ────────────────────────────────────────────────────────────

    private IEnumerator RutinaAtaque()
    {
        enCorrutina = true;
        rb.velocity = Vector2.zero;

        while (ataquesRealizados < ataquesPorRafaga)
        {
            ActualizarObjetivo();
            if (jugadorObjetivo == null) break;

            float dist = Vector2.Distance(transform.position, jugadorObjetivo.position);
            if (dist > rangoAtaque * 1.3f)
            {
                ataquesRealizados = 0;
                enCorrutina = false;
                CambiarEstado(Estado.Perseguir);
                yield break;
            }

            sr.flipX = (jugadorObjetivo.position.x - transform.position.x) < 0;

            LimpiarTriggers();
            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(duracionAtaque * puntoDeImpacto);
            AplicarDanioCercano();
            yield return new WaitForSeconds(duracionAtaque * (1f - puntoDeImpacto) + cooldownAtaque);

            ataquesRealizados++;
        }

        ataquesRealizados = 0;
        enCorrutina = false;
        CambiarEstado(Estado.Cansado);
    }

    private IEnumerator RutinaDisparo()
    {
        enCorrutina = true;
        rb.velocity = Vector2.zero;

        // Disparar 3 veces
        for (int i = 0; i < proyectilesPorRafaga; i++)
        {
            ActualizarObjetivo();
            if (jugadorObjetivo == null) break;

            sr.flipX = (jugadorObjetivo.position.x - transform.position.x) < 0;
            LimpiarTriggers();
            animator.SetTrigger("Cast");
            LanzarParProyectiles();

            yield return new WaitForSeconds(tiempoEntreProyectiles);
        }

        // Caminar hacia el jugador por duracionCaminataTrasDiparo segundos
        animator.SetBool("isWalking", true);
        float tiempoCaminando = 0f;

        while (tiempoCaminando < duracionCaminataTrasDiparo)
        {
            ActualizarObjetivo();
            if (jugadorObjetivo == null) break;

            float dist = Vector2.Distance(transform.position, jugadorObjetivo.position);

            // Si llega al rango melee, salir a atacar directamente
            if (dist <= rangoAtaque)
            {
                animator.SetBool("isWalking", false);
                enCorrutina = false;
                CambiarEstado(Estado.Atacar);
                yield break;
            }

            float dir = jugadorObjetivo.position.x - transform.position.x;
            sr.flipX = dir < 0;
            rb.velocity = new Vector2(Mathf.Sign(dir) * velocidadCaminata, rb.velocity.y);

            tiempoCaminando += Time.deltaTime;
            yield return null;
        }

        // Terminó de caminar y sigue lejos → volver a disparar
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        enCorrutina = false;
        CambiarEstado(Estado.Disparar);
    }

    private IEnumerator RutinaColumnas()
    {
        enCorrutina = true;
        seriesLanzadas = 0;

        if (posicionFondo != null)
        {
            animator.SetBool("isWalking", true);
            while (true)
            {
                float distX = Mathf.Abs(transform.position.x - posicionFondo.position.x);
                if (distX <= 0.1f)
                {
                    rb.velocity = Vector2.zero;
                    transform.position = new Vector3(
                        posicionFondo.position.x,
                        transform.position.y,
                        transform.position.z);
                    break;
                }
                float dir = posicionFondo.position.x - transform.position.x;
                sr.flipX = dir < 0;
                rb.velocity = new Vector2(Mathf.Sign(dir) * velocidadIrAlFondo, rb.velocity.y);
                yield return null;
            }
            animator.SetBool("isWalking", false);
        }

        estadoActual = Estado.Columnas;

        ActualizarObjetivo();
        if (jugadorObjetivo != null)
            sr.flipX = (jugadorObjetivo.position.x - transform.position.x) < 0;

        while (estadoActual == Estado.Columnas)
        {
            LimpiarTriggers();
            animator.SetTrigger("Cast");
            yield return new WaitForSeconds(0.3f);

            for (int c = 0; c < columnasPorSerie; c++)
            {
                LanzarColumna(c);
                yield return new WaitForSeconds(delayEntreColumnas);
            }

            seriesLanzadas++;

            if (seriesLanzadas >= seriesPorPausa)
            {
                seriesLanzadas = 0;
                LimpiarTriggers();
                animator.SetTrigger("Hurt");
                yield return new WaitForSeconds(duracionPausaFase3);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        enCorrutina = false;
    }

    private IEnumerator RutinaCansancio()
    {
        enCorrutina = true;
        rb.velocity = Vector2.zero;

        LimpiarTriggers();
        animator.SetTrigger("Hurt");

        yield return new WaitForSeconds(duracionCansancio);

        enCorrutina = false;
        CambiarEstado(Estado.Perseguir);
    }

    // ── DAÑO ─────────────────────────────────────────────────────────────────

    private void AplicarDanioCercano()
    {
        float ladoX    = sr.flipX ? -1f : 1f;
        Vector2 centro = (Vector2)transform.position + new Vector2(offsetHitbox.x * ladoX, offsetHitbox.y);
        Collider2D[] golpeados = Physics2D.OverlapCircleAll(centro, radioHitboxAtaque);

        foreach (Collider2D c in golpeados)
        {
            Vector2 dir = (c.transform.position - transform.position).normalized;
            if (c.CompareTag("Player"))
            {
                VidaJugador v = c.GetComponent<VidaJugador>();
                if (v != null) v.RecibirDaño(dir);
            }
            if (c.CompareTag("Player2"))
            {
                VidaJugador2 v = c.GetComponent<VidaJugador2>();
                if (v != null) v.RecibirDaño(dir);
            }
        }
    }

    private void LanzarParProyectiles()
    {
        if (proyectilPrefab == null || jugadorObjetivo == null) return;

        float dirX = jugadorObjetivo.position.x - transform.position.x;
        Vector2 direccion = new Vector2(Mathf.Sign(dirX), 0f);

        Vector2 origen1 = (Vector2)transform.position + new Vector2(0f, offsetYProyectil1);
        GameObject p1 = Instantiate(proyectilPrefab, origen1, Quaternion.identity);
        NavajaVuelo n1 = p1.GetComponent<NavajaVuelo>();
        if (n1 != null) n1.Configurar(direccion, origen1);

        Vector2 origen2 = (Vector2)transform.position + new Vector2(0f, offsetYProyectil2);
        GameObject p2 = Instantiate(proyectilPrefab, origen2, Quaternion.identity);
        NavajaVuelo n2 = p2.GetComponent<NavajaVuelo>();
        if (n2 != null) n2.Configurar(direccion, origen2);
    }

    private void LanzarColumna(int indiceColumna)
    {
        if (proyectilPrefab == null) return;

        int huecoIndex = Random.Range(0, 4);
        float xColumna = transform.position.x + (sr.flipX ? -1f : 1f)
                         * (indiceColumna + 1) * (anchoArena / (columnasPorSerie + 1));

        for (int i = 0; i < 4; i++)
        {
            if (i == huecoIndex) continue;

            float yDaga = transform.position.y + (i * (alturaColumna / 3f)) - (alturaColumna / 2f);
            Vector3 posicion = new Vector3(xColumna, yDaga, 0f);

            GameObject daga = Instantiate(proyectilPrefab, posicion, Quaternion.identity);
            NavajaVuelo vuelo = daga.GetComponent<NavajaVuelo>();
            if (vuelo != null)
            {
                float dirX = jugadorObjetivo != null ?
                    Mathf.Sign(jugadorObjetivo.position.x - transform.position.x) : 1f;
                vuelo.Configurar(new Vector2(dirX, 0f), posicion);
            }
        }
    }

    // ── LLAMADOS EXTERNOS ─────────────────────────────────────────────────────

    public void OnHit()
    {
        if (estadoActual == Estado.Cansado) return;
        if (estadoActual == Estado.Atacar)
        {
            StopAllCoroutines();
            LimpiarTriggers();
            ataquesRealizados = 0;
            enCorrutina = false;
            CambiarEstado(Estado.Perseguir);
        }
    }

    public void OnMuerte()
    {
        estadoActual = Estado.Muerto;
        StopAllCoroutines();
        LimpiarTriggers();
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }

    // ── GIZMOS ────────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        float lado = (s != null && s.flipX) ? -1f : 1f;
        Gizmos.DrawWireSphere(
            (Vector2)transform.position + new Vector2(offsetHitbox.x * lado, offsetHitbox.y),
            radioHitboxAtaque);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0f, offsetYProyectil1), 0.15f);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0f, offsetYProyectil2), 0.15f);
    }
}