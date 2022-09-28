#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class WebView2PostBuildCopyFiles
{
    private const string WebView2Exe = "/WebView2Forms.exe";
    private const string WebView2Folder = "/WebView2Forms";

    private static readonly string[] WebView2Dlls =
    {
        "/Microsoft.Web.WebView2.Core.dll",
        "/Microsoft.Web.WebView2.WinForms.dll",
        "/WebView2Loader.dll",
    };

    [PostProcessBuild]
    public static void Start(BuildTarget target, string buildDirectory)
    {
        var path = new StackTrace(true).GetFrame(0).GetFileName();
        var pathWebView2 = Directory.GetParent(path).Parent.Parent.FullName;
        var pathBuild = Directory.GetParent(buildDirectory).FullName + "/" + Application.productName + "_Data";

        Copy(pathWebView2 + WebView2Folder + WebView2Exe, pathBuild + WebView2Exe);

        foreach (var dll in WebView2Dlls)
            Copy(pathWebView2 + WebView2Folder + dll, pathBuild + dll);
    }

    private static void Copy(string from, string dest) => File.Copy(from, dest, true);
}
#endif