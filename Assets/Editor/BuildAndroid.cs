using System;
using UnityEditor;
using UnityEngine;

// Build script para gerar o APK e/ou o Android App Bundle (.aab) assinados.
// Executado via linha de comando em batch mode:
//   Unity.exe -quit -batchmode -projectPath <proj> -executeMethod BuildAndroid.BuildAll -logFile <log>
//
// Metodos disponiveis:
//   BuildAndroid.BuildAAB  -> apenas o .aab (Play Store)
//   BuildAndroid.BuildAPK  -> apenas o .apk (instalacao direta / testes)
//   BuildAndroid.BuildAll  -> gera os dois
//
// As senhas do keystore sao lidas de variaveis de ambiente para nao ficarem no codigo:
//   ANDROID_KEYSTORE_PASS  -> senha do keystore
//   ANDROID_KEYALIAS_PASS  -> senha do alias
public static class BuildAndroid
{
    const string OutDir = "Builds/Android";

    public static void BuildAAB()
    {
        if (Build(appBundle: true))
            EditorApplication.Exit(0);
        else
            EditorApplication.Exit(1);
    }

    public static void BuildAPK()
    {
        if (Build(appBundle: false))
            EditorApplication.Exit(0);
        else
            EditorApplication.Exit(1);
    }

    public static void BuildAll()
    {
        bool okApk = Build(appBundle: false);
        bool okAab = Build(appBundle: true);
        EditorApplication.Exit(okApk && okAab ? 0 : 1);
    }

    // Retorna true em caso de sucesso. Nao chama Exit para permitir buildar os dois em sequencia.
    static bool Build(bool appBundle)
    {
        var scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("[BuildAndroid] Nenhuma cena habilitada em Build Settings.");
            return false;
        }

        string keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASS");
        string aliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASS");
        if (string.IsNullOrEmpty(keystorePass) || string.IsNullOrEmpty(aliasPass))
        {
            Debug.LogError("[BuildAndroid] Defina ANDROID_KEYSTORE_PASS e ANDROID_KEYALIAS_PASS.");
            return false;
        }

        // Caminho absoluto do keystore (Application.dataPath == <projeto>/Assets)
        string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
        string keystorePath = System.IO.Path.Combine(projectRoot, "user.keystore");
        if (!System.IO.File.Exists(keystorePath))
        {
            Debug.LogError("[BuildAndroid] Keystore nao encontrado em: " + keystorePath);
            return false;
        }

        // Assinatura release
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keyaliasName = "emboscada";
        PlayerSettings.Android.keystorePass = keystorePass;
        PlayerSettings.Android.keyaliasPass = aliasPass;

        EditorUserBuildSettings.buildAppBundle = appBundle;

        System.IO.Directory.CreateDirectory(OutDir);
        string ext = appBundle ? "aab" : "apk";
        string outPath = System.IO.Path.Combine(
            OutDir,
            $"jogo-emboscada-{PlayerSettings.bundleVersion}-vc{PlayerSettings.Android.bundleVersionCode}.{ext}");

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None,
        };

        Debug.Log($"[BuildAndroid] Keystore: {keystorePath}");
        Debug.Log($"[BuildAndroid] Iniciando build {ext.ToUpper()} -> {outPath}");
        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"[BuildAndroid] SUCESSO {ext.ToUpper()}: {outPath} ({summary.totalSize} bytes)");
            return true;
        }

        Debug.LogError($"[BuildAndroid] FALHOU {ext.ToUpper()}: {summary.result} ({summary.totalErrors} erros)");
        return false;
    }

    static string[] GetEnabledScenes()
    {
        var list = new System.Collections.Generic.List<string>();
        foreach (var s in EditorBuildSettings.scenes)
            if (s.enabled) list.Add(s.path);
        return list.ToArray();
    }
}
