using UnityEngine;
using UnityEngine.EventSystems;

namespace WebView2
{
    public class WebView2OpenCloseClick : MonoBehaviour, IPointerClickHandler
    {
        public WebView2Controller webView2;

        public void OnPointerClick(PointerEventData eventData) =>
            webView2.gameObject.SetActive(!webView2.gameObject.activeSelf);
    }
}