using System.Collections;
using UnityEngine;
using TMPro;

// Versión del diálogo para el boss — arranca automáticamente desde IntroduccionBoss
// No necesita trigger ni signo de "!"
public class DialogoBoss : MonoBehaviour
{
    [SerializeField] private GameObject PanelDialogo;
    [SerializeField] private TMP_Text TextoDialogo;
    [SerializeField, TextArea(2, 10)] private string[] dialogos;

    private bool isDialogoActive;
    private int indexDialogo;
    private float dialogoTimer = 0.05f;

    void Start()
    {
        if (PanelDialogo != null) PanelDialogo.SetActive(false);
    }
    // IntroduccionBoss llama este método cuando llega el momento
    public void IniciarDialogoAuto()
    {
        Debug.Log("IniciarDialogoAuto llamado");
        if(isDialogoActive) return;
        isDialogoActive = true;
        PanelDialogo.SetActive(true);
        indexDialogo = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowDialogo());
    }

    void Update()
    {
        if (!isDialogoActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (TextoDialogo.text == dialogos[indexDialogo])
                NextDialogo();
            else
            {
                StopAllCoroutines();
                TextoDialogo.text = dialogos[indexDialogo];
            }
        }
    }

    private void NextDialogo()
    {
        indexDialogo++;
        if (indexDialogo < dialogos.Length)
        {
            StartCoroutine(ShowDialogo());
        }
        else
        {
            isDialogoActive = false;
            PanelDialogo.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private IEnumerator ShowDialogo()
    {
        TextoDialogo.text = string.Empty;
        foreach (char letter in dialogos[indexDialogo])
        {
            TextoDialogo.text += letter;
            yield return new WaitForSecondsRealtime(dialogoTimer);
        }
    }

    // Para que IntroduccionBoss sepa cuándo terminó
    public bool DialogoTerminado() => !isDialogoActive && PanelDialogo != null && !PanelDialogo.activeSelf;
}