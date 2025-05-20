using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_5
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;
    
        public static CoroutineRunner Instance 
        {
            get 
            {
                if (instance) return instance;
                var go = new GameObject("CoroutineRunner");
                instance = go.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(go);
                return instance;
            }
        }
    }
    
    public static class RepescagemManager
    {  
        public static GameObject LoadingPagePrefab;
        public static int CurrentRepescagemLevel { get; private set; } = 0;
        public static void CheckAllRepescagensComplete()
        {
            bool allComplete = true;
        
            for (int i = 1; i < 4; i++)
            {
                if (PlayerPrefs.GetInt($"repescagem{i}", 0) != 1)
                {
                    allComplete = false;
                    break;
                }
            }
            CoroutineRunner.Instance.StartCoroutine(LoadingScene(allComplete));
        }
        public static void StartRepescagem(int nivel)
        {
            CurrentRepescagemLevel = nivel;
        }

        public static void Clear()
        {
            CurrentRepescagemLevel = 0;
        }

        public static bool IsRepescagemMode(int nivel)
        {
            return CurrentRepescagemLevel == nivel;
        }
        public static IEnumerator LoadingScene(bool allComplete)
        {
            GameObject loadingPage = null;
            if (LoadingPagePrefab)
            {
                loadingPage = Object.Instantiate(LoadingPagePrefab);
            }
            
            string sceneToLoad = allComplete ? "fase5" : "faserepescagem";
            
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneToLoad);

            Slider progressBar = loadingPage?.GetComponentInChildren<Slider>();
            
            while (!asyncOp.isDone)
            {
                if (progressBar != null)
                {
                    progressBar.value = asyncOp.progress;
                }
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
            
            if (loadingPage is not null)
            {
                Object.Destroy(loadingPage);
            }
        }
        public static void SetLoadingPagePrefab(GameObject prefab)
        {
            LoadingPagePrefab = prefab;
        }
    }
}
