using System;
using System.Collections;
using System.Collections.Generic;
using Fase_5;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// Se o RepescagemManager estiver num namespace, adicione o using correspondente.
// using MeuNamespace.Repescagem;

namespace Fase_4
{
    public class EnigmaScript : MonoBehaviour
    {
        [Header("Timer")]
        [Tooltip("Tempo total para resolver os enigmas (em segundos)")]
        [SerializeField] private float tempoTotal = 30f;
        [SerializeField] private Image timerImage;
        [SerializeField] private GameObject timeOut;

        private float _tempoRestante;
        private bool _timerAtivo = false;
        private bool _tempoEsgotado = false;

        public static EnigmaScript instance;
        public List<Enigma> enigmas;
        public GameObject puzzleUIPrefab;
        public Transform parentCanvas;
        public int cenaVitoria;
        public int cenaDerrota;
        [SerializeField] private GameObject instruct;

        private int respostasCorretas = 0;
        private int totalRespondidos = 0;

        public AudioClip prologoClip;
        public AudioSource audioSource;
        public Button[] botoesEnigmas;
        private bool isRepescagemMode;
        private const int thisLevel = 4;

        private void Awake()
        {
            // Singleton
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Detecta se estamos em repescagem deste nível
            isRepescagemMode = RepescagemManager.IsRepescagemMode(thisLevel);
        }

        private void Start()
        {
            _tempoRestante = tempoTotal;
            PausarTimer();
            timeOut.SetActive(false);
            StartCoroutine(RunAudio());

            // liga cada botão ao índice
            for (int i = 0; i < botoesEnigmas.Length; i++)
            {
                int idx = i;
                botoesEnigmas[i].onClick.RemoveAllListeners();
                botoesEnigmas[i].onClick.AddListener(() =>
                    SelecionarEnigma(idx, botoesEnigmas[idx])
                );
            }
        }

        private void Update()
        {
            if (_timerAtivo && !_tempoEsgotado)
            {
                _tempoRestante -= Time.deltaTime;
                float fill = _tempoRestante / tempoTotal;
                timerImage.fillAmount = fill;

                if (_tempoRestante <= 0)
                {
                    _tempoRestante = 0;
                    _timerAtivo = false;
                    _tempoEsgotado = true;
                    DesabilitarBotoesEnigma();
                    MostrarMensagemTempoEsgotado();
                    StartCoroutine(FinalizarComDerrota(1.5f));
                }
            }
        }

        public void SelecionarEnigma(int index, Button origemBtn)
        {
            if (_tempoEsgotado) return;

            var go = Instantiate(puzzleUIPrefab, parentCanvas, false);
            var cv = go.GetComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceCamera;
            cv.worldCamera = Camera.main;
            cv.sortingOrder = 3;
            go.transform.SetAsLastSibling();

            var ui = go.GetComponentInChildren<EnigmaUI>();
            ui.Inicializar(enigmas[index], OnEnigmaRespondido, origemBtn);
        }

        private void OnEnigmaRespondido(bool acertou)
        {
            totalRespondidos++;
            if (acertou) respostasCorretas++;

            if (totalRespondidos >= enigmas.Count)
            {
                _timerAtivo = false;
                if (respostasCorretas >= 2)
                    HandleWin();
                else
                    HandleLose();
            }
        }

        private void HandleWin()
        {
            
            if (isRepescagemMode)
            {
                PlayerPrefs.SetInt($"repescagem{thisLevel}", 0);
                PlayerPrefs.SetInt("nivel3", 1);
                PlayerPrefs.Save();
                RepescagemManager.Clear();
                SceneManager.LoadScene("faserepescagem");
            }
            else
            { SaveGame(true);
                MainManager.indiceCanvainicial = cenaVitoria;
                SceneManager.LoadSceneAsync("main");
            }
        }

        private void HandleLose()
        {

            if (isRepescagemMode)
            {
                // Reinicia a própria cena até passar
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            { SaveGame(false);
                MainManager.indiceCanvainicial = cenaDerrota;
                SceneManager.LoadSceneAsync("main");
            }
        }

        private IEnumerator FinalizarComDerrota(float delay)
        {
            yield return new WaitForSeconds(delay);
            HandleLose();
        }

        private void DesabilitarBotoesEnigma()
        {
            foreach (var botao in botoesEnigmas)
                botao.interactable = false;
        }

        private void MostrarMensagemTempoEsgotado()
        {
            timeOut.SetActive(true);
        }

        private void SaveGame(bool acertou)
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisganhos[3] = acertou;
            int cls = PlayerPrefs.GetInt("classificacao", 0);
            if (acertou) cls++;
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)cls;
            EmboscadaController.gameData.currentLevel = 80;

            PlayerPrefs.SetInt("nivel3", acertou ? 1 : 0);
            PlayerPrefs.SetInt("classificacao", cls);
            PlayerPrefs.SetInt("currentLevel", EmboscadaController.gameData.currentLevel);
            PlayerPrefs.Save();
        }

        public void CloseInstruct()
        {
            if (instruct != null)
            {
                instruct.SetActive(false);
                Destroy(instruct);
                LigarTimer();
            }
        }

        private IEnumerator RunAudio()
        {
            var steps = new List<Func<IEnumerator>>()
            {
                () => PrepareAudio(prologoClip),
            };

            yield return LoadingScreenController.Instance.ShowLoading(steps);
            audioSource.Play();
        }

        private IEnumerator PrepareAudio(AudioClip clip)
        {
            if (clip == null) yield break;
            clip.LoadAudioData();
            while (clip.loadState != AudioDataLoadState.Loaded)
                yield return null;
        }

        public void PausarTimer() => _timerAtivo = false;
        public void RetornarTimer() { if (!_tempoEsgotado) _timerAtivo = true; }
        public void LigarTimer()    => _timerAtivo = true;
    }
}
