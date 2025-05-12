using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public static LoadingScreenController Instance { get; private set; }

    [Header("Referências UI")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressBar;
    [SerializeField][Tooltip("Tempo de fade-in/out em segundos")] private float fadeTime = 0.5f;


    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        loadingScreen.SetActive(false);
    }


    public IEnumerator ShowLoading(IEnumerable<Func<IEnumerator>> loadRoutines)
    {
        loadingScreen.SetActive(true);
        canvasGroup.alpha = 1;
        progressBar.fillAmount = 0;

        var routines = new List<Func<IEnumerator>>(loadRoutines);
        int total = routines.Count;
        int done = 0;

        foreach (var getRoutine in routines)
        {

            yield return StartCoroutine(RunWithProgress(getRoutine(), p =>
            {
                float baseOffset = (float)done / total;
                float slice = 1f / total;
                progressBar.fillAmount = Mathf.Clamp01(baseOffset + p * slice);
            }));
            done++;
        }

        yield return StartCoroutine(Fade(1, 0));
        loadingScreen.SetActive(false);
    }

    private IEnumerator RunWithProgress(IEnumerator routine, Action<float> onProgress)
    {
        yield return StartCoroutine(routine);
        onProgress(1f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t0 = Time.time;
        while (Time.time < t0 + fadeTime)
        {
            float t = (Time.time - t0) / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    private void UpdateProgressBar(float progress)
    {
        progressBar.fillAmount = Mathf.Clamp01(progress);
    }

    public IEnumerator ShowLoading(AsyncOperation op)
    {
        loadingScreen.SetActive(true);
        canvasGroup.alpha = 1f;
        progressBar.fillAmount = 0f;
        
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            float p = Mathf.Clamp01(op.progress / 0.9f);
            UpdateProgressBar(p);
            Debug.Log($"Loading progress: {p*100f:0.0}%");
            yield return null;
        }
        UpdateProgressBar(1f);
        float t0 = Time.time;
        while (Time.time < t0 + fadeTime)
        {
            float t = (Time.time - t0) / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        op.allowSceneActivation = true;
        yield return new WaitUntil(() => op.isDone);
        loadingScreen.SetActive(false);
    }

}
