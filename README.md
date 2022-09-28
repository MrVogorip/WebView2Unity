# WebView2Unity
Simple WebView2 example with Unity

Due to the impossibility of using WebView2 directly in Unity, this example shows how WebView2 can be used in Unity through a WinForms middleware application.
Communication between Unity and WinForms application is carried out via TCP, sending click coordinates and receiving a browser image.
To run, you need to build WebView2Forms project, and copy 4 files
(Microsoft.Web.WebView2.Core.dll, Microsoft.Web.WebView2.WinForms.dll, WebView2Loader.dll, WebView2Forms.exe) 
to WebView2Forms folder in Unity project.
Starting and closing WebView2Forms.exe is done through kernel32.dll functions, due to il2cpp unity limitations.
Communication with browser is possible only through javascript.
p.s. this is an unproductive workaround.
