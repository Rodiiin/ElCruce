using UnityEngine;
using UnityEngine.EventSystems;

public class BotonSonido : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource source;
    public AudioClip sonidoHover; 
    public AudioClip sonidoClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        source.PlayOneShot(sonidoHover);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        source.PlayOneShot(sonidoClick);
    }
}