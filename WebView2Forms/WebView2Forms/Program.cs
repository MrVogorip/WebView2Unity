using System;
using System.Windows.Forms;

namespace WebView2Forms
{
    public static class WebView2FormLoader
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WebView2Forms());
        }
    }
}
