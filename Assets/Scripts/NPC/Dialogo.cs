using System.Collections;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class Dialogo : MonoBehaviour
{
    [SerializeField] private GameObject SignoDialogo;
    [SerializeField] private GameObject PanelDialogo;
    [SerializeField] private TMP_Text    TextoDialogo;
    [SerializeField,TextArea(2,10)] private string[] dialogos;
    private bool isPlayerInRange;
    private bool isDialogoActive;
    private int indexDialogo;
    private float dialogoTimer=0.05f;
    // Start is called before the first frame updateate is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogoActive)
            {
                strartDialogo();
            }
            else if (TextoDialogo.text == dialogos[indexDialogo])
            {
                NextDialogo();
            }
            else
            {
                StopAllCoroutines();
                TextoDialogo.text = dialogos[indexDialogo];
            }
        }
    }
    private void  strartDialogo()
    {
        isDialogoActive = true;
        PanelDialogo.SetActive(true);
        SignoDialogo.SetActive(false);
        indexDialogo = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowDialogo());
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
            SignoDialogo.SetActive(true);
            Time.timeScale = 1f;
        }
    }
    private IEnumerator ShowDialogo()
    {
        TextoDialogo.text=string.Empty;
        foreach (char letter in dialogos[indexDialogo])
        {
            TextoDialogo.text += letter;
            yield return new WaitForSecondsRealtime(dialogoTimer);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            isPlayerInRange = true;
            SignoDialogo.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            isPlayerInRange = false;
            SignoDialogo.SetActive(false);
        }
    }
}
