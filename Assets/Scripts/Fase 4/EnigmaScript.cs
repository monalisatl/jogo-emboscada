using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_4
{
    public class EnigmaScript : MonoBehaviour
    {
        [Header("Timer")]
        [Tooltip("Tempo total para resolver os enigmas (em segundos)")]
        [SerializeField] private float tempoTotal = 300f; // 5 minutos para resolver os enigmas
        [SerializeField] private Image timerImage; // Referência para a imagem do timer na UI
        private float _tempoRestante;
        private bool _timerAtivo = false;
        private bool _tempoEsgotado = false;
    
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


        void Start()
        {
            // Inicializa o tempo
            _tempoRestante = tempoTotal;
            _timerAtivo = true;
        
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
    
        void Update()
        {
            if (_timerAtivo && !_tempoEsgotado)
            {
                _tempoRestante -= Time.deltaTime;
                if (timerImage != null)
                {
                    timerImage.fillAmount = _tempoRestante / tempoTotal;
                }
                if (_tempoRestante <= 0)
                {
                    _tempoRestante = 0;
                    _timerAtivo = false;
                    _tempoEsgotado = true;
                    Debug.Log("Tempo esgotado! O jogador perdeu.");
                    DesabilitarBotoesEnigma();
                    MostrarMensagemTempoEsgotado();
                    StartCoroutine(FinalizarComDerrota(1.5f));
                }
            }
        }

        public void SelecionarEnigma(int index, Button origemBtn)
        {
            // Se o tempo acabou, não permite mais selecionar enigmas
            if (_tempoEsgotado)
                return;
            
            var go = Instantiate(puzzleUIPrefab, null, false);
            Canvas cv = go.GetComponent<Canvas>();
            cv.renderMode      = RenderMode.ScreenSpaceCamera;
            cv.worldCamera     = Camera.main;
            cv.sortingOrder    = 3;
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
                {
                    SaveGame(true);
                    MainManager.indiceCanvainicial = cenaVitoria;
                    SceneManager.LoadSceneAsync("main");
                }
                else
                {
                    SaveGame(false);
                    MainManager.indiceCanvainicial = cenaDerrota;
                    SceneManager.LoadSceneAsync("main");
                }
            }
        }
    
        private void DesabilitarBotoesEnigma()
        {
            // Desabilita todos os botões de enigmas
            foreach (var botao in botoesEnigmas)
            {
                botao.interactable = false;
            }
        }
    
        private void MostrarMensagemTempoEsgotado()
        {
            GameObject mensagem = new GameObject("MensagemTempoEsgotado");
            mensagem.transform.SetParent(parentCanvas, false);
        
            RectTransform rt = mensagem.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(800, 200);
        
            Text texto = mensagem.AddComponent<Text>();
            texto.text = "TEMPO ESGOTADO!";
            texto.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            texto.fontSize = 48;
            texto.alignment = TextAnchor.MiddleCenter;
            texto.color = Color.red;
        }
    
        private IEnumerator FinalizarComDerrota(float delay)
        {
            // Aguarda um pequeno delay antes de finalizar o jogo
            yield return new WaitForSeconds(delay);
        
            SaveGame(false);
            MainManager.indiceCanvainicial = cenaDerrota;
            SceneManager.LoadSceneAsync("main");
        }

        private void SaveGame(bool acertou)
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisganhos[3] = acertou;
            int cls = PlayerPrefs.GetInt("classificacao", 0);
            if (acertou) cls++;
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)cls;
            
            EmboscadaController.gameData.currentLevel = 80;

            // grava no PlayerPrefs
            PlayerPrefs.SetInt("nivel3", acertou ? 1 : 0);
            Debug.Log($"salvo nivel3 com valor:{acertou}\nbuscando o valor{PlayerPrefs.GetInt("nivel3")}");
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

        private IEnumerator PrepareAudio(AudioClip prologoClip)
        {
            if (prologoClip == null)
            {
                Debug.LogWarning("LoadAudio: prologoClip não atribuído.");
                yield break;
            }

            prologoClip.LoadAudioData();
            while (prologoClip.loadState != AudioDataLoadState.Loaded)
            {
                if (prologoClip.loadState == AudioDataLoadState.Failed)
                {
                    Debug.LogError("LoadAudio: Falha ao carregar os dados de áudio.");
                    yield break;
                }
                yield return null;
            }
        }
        public void PausarTimer()
        {
            _timerAtivo = false;
        }
        
        public void RetornarTimer()
        {
            if (!_tempoEsgotado)
                _timerAtivo = true;
        }
    }
}
