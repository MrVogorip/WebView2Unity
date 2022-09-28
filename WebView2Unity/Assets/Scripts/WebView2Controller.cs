using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WebView2
{
    public class WebView2Controller : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        private TcpServer _server;
        private IntPtr _hProcess;

        private RectTransform _transform;
        private Image _image;
        private bool _canUpdate;
        private byte[] _imageBytes;
        private int _scrollDelta;
        private Vector2 _scroll;

        private static string PathWebView2
        {
            get
            {
#if UNITY_EDITOR
                return (Application.dataPath + @"\WebView2Forms\WebView2Forms.exe").Replace("Assets", "");
#else
                return Application.dataPath + @"\WebView2Forms.exe";
#endif
            }
        }

        private void OnEnable()
        {
            _transform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _server = new TcpServer();
            _server.UpdateData += UpdateData;
            LoadWebView2();
            StartCoroutine(ConnectToServer());
        }

        private void FixedUpdate()
        {
            if (!_canUpdate || _imageBytes == null)
                return;

            _canUpdate = false;
            var texture = new Texture2D(1920, 1080);
            texture.LoadImage(_imageBytes);
            _image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }

        private void OnDisable()
        {
            _server.UpdateData -= UpdateData;
            _server.Close();
            _imageBytes = null;
            _image.sprite = null;

            WinApi.Close(_hProcess);
        }

        private void LoadWebView2() => 
            _hProcess = WinApi.Open(PathWebView2);

        private IEnumerator ConnectToServer()
        {
            yield return new WaitForSeconds(1.3f);
            _server.Start();
        }

        private void UpdateData(byte[] data)
        {
            _imageBytes = data;
            _canUpdate = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_server == null)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _transform, 
                eventData.position, 
                null,
                out var localPoint);

            var data = new ClickData(localPoint, _scrollDelta);

            _server.SendMessage(data.ToString());
        }

        public void OnPointerDown(PointerEventData pointerEventData) => 
            _scroll = pointerEventData.position;

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            _scroll -= pointerEventData.position;
            _scrollDelta = (int)_scroll.y;
        }
    }
}