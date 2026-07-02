using System;
using UnityEditor;
using UnityEngine;

// Build script para gerar o Android App Bundle (.aab) assinado para a Play Store.
// Executado via linha de comando em batch mode:
//   Unity.exe -quit -batchmode -projectPath <proj> -executeMethod BuildAndroid.BuildAAB -logFile <log>
//
// As senhas do keystore sao lidas de variaveis de ambiente para nao ficarem no codigo:
//   ANDROID_KEYSTORE_PASS  -> senha do keystore
//   ANDROID_KEYALIAS_PASS  -> senha do alias
public static class BuildAndroid
{
    public static void BuildAAB()
    {
        var scenes = GetEnabledScenes();
        if (scenes.Length == 0)
            Fail("Nenhuma cena habilitada em Build Settings.");

        string keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASS");
        string aliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASS");
        if (string.IsNullOrEmpty(keystorePass) || string.IsNullOrEmpty(aliasPass))
            Fail("Defina as variaveis de ambiente ANDROID_KEYSTORE_PASS e ANDROID_KEYALIAS_PASS.");

        // Assinatura release
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = "user.keystore"; // relativo a raiz do projeto
        PlayerSettings.Android.keyaliasName = "emboscada";
        PlayerSettings.Android.keystorePass = keystorePass;
        PlayerSettings.Android.keyaliasPass = aliasPass;

        // App Bundle (.aab) exigido pela Play Store
        EditorUserBuildSettings.buildAppBundle = true;

        string outDir = "Builds/Android";
        System.IO.Directory.CreateDirectory(outDir);
        string outPath = System.IO.Path.Combine(
            outDir,
            $"jogo-emboscada-{PlayerSettings.bundleVersion}-vc{PlayerSettings.Android.bundleVersionCode}.aab");

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None,
        };

        Debug.Log($"[BuildAndroid] Iniciando build AAB -> {outPath}");
        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"[BuildAndroid] SUCESSO: {outPath} ({summary.totalSize} bytes)");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"[BuildAndroid] FALHOU: {summary.result} ({summary.totalErrors} erros)");
            EditorApplication.Exit(1);
        }
    }

    static string[] GetEnabledScenes()
    {
        var list = new System.Collections.Generic.List<string>();
        foreach (var s in EditorBuildSettings.scenes)
            if (s.enabled) list.Add(s.path);
        return list.ToArray();
    }

    static void Fail(string msg)
    {
        Debug.LogError("[BuildAndroid] " + msg);
        EditorApplication.Exit(1);
    }
}
