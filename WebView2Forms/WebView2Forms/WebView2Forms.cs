using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebView2Forms
{
    public partial class WebView2Forms : Form
    {
        private const int MillisecondsDelay = 600;
        private const string UrlGoogle = "https://www.google.com/";
        
        private readonly WebView2 _webView;
        private readonly TcpServer _server;
        private readonly string _url;
        
        private byte[] _imageBytes;

        public WebView2Forms()
        {
            InitializeComponent();

            _url = UrlGoogle;
            _server = new TcpServer();
            _server.Click += OnClick;
            _webView = new WebView2 { Size = new Size(1024, 768) };
            _webView.CoreWebView2InitializationCompleted += CoreWebView2InitializationCompleted;
            _webView.NavigationCompleted += NavigationCompleted;

            Controls.Add(_webView);

            InitializeAsync();
        }

        private async void InitializeAsync() => 
            await _webView.EnsureCoreWebView2Async();

        private void CoreWebView2InitializationCompleted(object sender, EventArgs e) => 
            _webView?.CoreWebView2?.Navigate(_url);
        
        private async void ExecuteScript(string js) => 
            await _webView.CoreWebView2.ExecuteScriptAsync(js);

        private async void NavigationCompleted(object sender, EventArgs e)
        {
            if (_webView?.CoreWebView2 == null)
                return;
            
            _webView.NavigationCompleted -= NavigationCompleted;
            
            while (true)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await _webView.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, memoryStream);
                    _imageBytes = memoryStream.ToArray();
                    memoryStream.Close(); 
                    await Task.Delay(MillisecondsDelay);
           
                }
                _server.SendMessage(_imageBytes);
            }
        }

        private void OnClick(ClickData data)
        {
            var js = data.Scroll != 0 ? SimulateScrollScript(data.Scroll) : SimulateClickScript(data.X, data.Y);
            
            BeginInvoke(new MethodInvoker(() =>
            {
                if (_webView?.CoreWebView2 == null)
                    return;
                
                ExecuteScript(js);
            }));
        }

        private static string SimulateScrollScript(int scroll) => 
            "window.scrollTo({top: this.scrollY + "+ scroll + ", behavior: 'smooth'});";

        private static string SimulateClickScript(string x, string y) =>
            "let ClickEvent = (x, y) => { let ev = new MouseEvent('click', { 'view': window, 'bubbles': true, 'cancelable': true, 'screenX': x, 'screenY': y });" +
            "let el = document.elementFromPoint(x, y); el.dispatchEvent(ev); };" +
            "let x = " + x + ";" +
            "let y = " + y + ";" +
            "ClickEvent(x, y);";

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            Opacity = 0;
            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _server.Click -= OnClick;
            _server?.Close();
            _webView?.Stop();
            base.OnFormClosing(e);
        }
    }
}
