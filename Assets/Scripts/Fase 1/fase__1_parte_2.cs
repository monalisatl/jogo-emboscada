using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Fase_1
{
    public class Fase1Parte2 : MonoBehaviour
    {
        public static Fase1Parte2 Instance;
        private bool _confirmationProcessed;
        [SerializeField] private ZonaSoltar[] dropZones;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button rejectButton;
        [SerializeField] private GameObject tableButtons;
        public static int AcertosGarais;
        [SerializeField] private Arrastavel[] allDraggableItems;
        private bool _isBeingDestroyed;
        [FormerlySerializedAs("ContinueButton")] [SerializeField] private Button continueButton;
        [Header("Fedback")]
        [SerializeField] private GameObject panelExplication;
        [SerializeField] private string[] feedbacks;
        [SerializeField] private Button[] feedbackButtons;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private GameObject feedbackAviso;
    
        [Header("Timer")]
        [Tooltip("Tempo máximo para completar a fase em segundos")]
        [SerializeField] private float tempoLimite = 200f;
        [Tooltip("Referência ao objeto de imagem do cronômetro")]
        [SerializeField] private Image cronometro;
        private float _tempoRestante;
        private bool _cronometroAtivo = true;
    
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            if (tableButtons)
                tableButtons.SetActive(false);
            if (confirmButton && confirmButton.gameObject)
                confirmButton.gameObject.SetActive(false);

            foreach (var button in feedbackButtons)
            {   button.interactable = false; }
            if (continueButton)
                continueButton.gameObject.SetActive(false);
            if (panelExplication)
                panelExplication.SetActive(false);
            if(feedbackAviso)
                feedbackAviso.gameObject.SetActive(false);
            
            _confirmationProcessed = false;
            _tempoRestante = tempoLimite; // Ensure timer is reset here too
            _cronometroAtivo = true;      // Ensure timer starts active
        }
    
        private void OnDestroy()
        {
            _isBeingDestroyed = true;
        
            if (Instance == this)
                Instance = null;
        }
    
        private void Start()
        {
            if (allDraggableItems == null || allDraggableItems.Length == 0)
            {
                allDraggableItems = FindObjectsByType<Arrastavel>(FindObjectsSortMode.None);
            }
        
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            }
        
            if (rejectButton != null)
            {
                rejectButton.onClick.RemoveAllListeners();
                rejectButton.onClick.AddListener(OnCancelButtonPress);
            }
        
            StartCoroutine(DelayedInitCheck());
            
            if (!cronometro)
            {
                Debug.LogError("Timer não adicionado!");
            }
        
            UpdateCronometro();
        }
    
        private void Update()
        {
            if (_isBeingDestroyed)
            {
                return;
            }

            if (_confirmationProcessed)
            {
                if (_tempoRestante > 0 && !_cronometroAtivo) { /* Already handled by confirm */ }
                    _cronometroAtivo = false; UpdateCronometro();
                return;
            }
            
            if (_cronometroAtivo)
            {
                _tempoRestante -= Time.deltaTime;
                _tempoRestante = Mathf.Max(_tempoRestante, 0f);
                UpdateCronometro();

                if (_tempoRestante <= 0f)
                {
                    Debug.Log("Tempo esgotado! Confirmação não realizada a tempo.");
                    _cronometroAtivo = false; 
                    TimeOut();
                }
            }
        }
    
        private void UpdateCronometro()
        {
            if (cronometro)
            {
                cronometro.fillAmount = _tempoRestante / tempoLimite;
            }
        }
    
        private void TimeOut()
        {

            foreach (ZonaSoltar zone in dropZones)
            {
                if (!zone) continue;
            
                if (zone.HasItem())
                {
                    int itemId = zone.GetCurrentItemId();
                    int zoneId = zone.GetZoneId();
                    if (itemId == zoneId)
                    {
                        Debug.Log($"Item {itemId} está na zona correta {zoneId}");
                        feedbackButtons[zoneId].gameObject.GetComponent<Image>().color = Color.green;
                    }
                    else
                    {
                        Debug.Log($"Item {itemId} está na zona errada {zoneId}");
                        feedbackButtons[zoneId].gameObject.GetComponent<Image>().color = Color.red;
                    }
                }
                else
                {
                    int zoneId = zone.GetZoneId();
                    Debug.Log($"Zona {zoneId} está vazia - marcada como errada");
                    feedbackButtons[zoneId].gameObject.GetComponent<Image>().color = Color.red;
                }
            }
        
            AcertosGarais = 0;
            Debug.Log($"Total de acertos: {AcertosGarais}");
            EnableFeedback();
        }
    
        private IEnumerator DelayedInitCheck()
        {
            yield return null;
        
            CheckAllItemsPlaced();
        }

        public void CheckAllItemsPlaced()
        {
            if (_isBeingDestroyed) return;
        
            if (dropZones == null || dropZones.Length == 0)
            {
                Debug.LogWarning("Não há zonas de drop definidas!");
                return;
            }
        
            bool allZonesFilled = true;
        
            foreach (ZonaSoltar zone in dropZones)
            {
                if (zone == null || !zone.HasItem())
                {
                    allZonesFilled = false;
                    break;
                }
            }
        
            try {
                if (allZonesFilled)
                {
                    if (tableButtons)
                        tableButtons.SetActive(true);
                    if (confirmButton && confirmButton.gameObject)
                        confirmButton.gameObject.SetActive(true);
                
                    Debug.Log("Todas as zonas preenchidas, botões ativados");
                }
                else
                {
                    if (tableButtons)
                        tableButtons.SetActive(false);
                    if (confirmButton && confirmButton.gameObject)
                        confirmButton.gameObject.SetActive(false);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao atualizar UI: {e.Message}");
            }
        }
    
        public void OnConfirmButtonClicked()
        {
            if (_isBeingDestroyed || _confirmationProcessed) return; // Prevent re-confirmation

            _cronometroAtivo = false;
            _confirmationProcessed = true; // Mark confirmation as processed

            Debug.Log("Confirmação realizada!");
            int acertos = 0;
        
            if (dropZones == null)
            {
                Debug.LogError("dropZones é nulo!");
                return;
            }
        
            foreach (ZonaSoltar zone in dropZones)
            {
                if (zone == null) continue;
            
                int itemId = zone.GetCurrentItemId();
                int zoneId = zone.GetZoneId();
                if (itemId == zoneId)
                {
                    acertos++;
                    Debug.Log($"Item {itemId} está na zona correta {zoneId}");
                    feedbackButtons[zoneId].gameObject.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    Debug.Log($"Item {itemId} está na zona errada {zoneId}");
                    feedbackButtons[zoneId].gameObject.GetComponent<Image>().color = Color.red;
                }
            }
        
            if (acertos == dropZones.Length+1)
                Debug.Log("Todos os itens estão nas colunas corretas!");
            else if (acertos >= (dropZones.Length + 1) / 2)
                Debug.Log("Alguns itens estão nas colunas corretas!");
            else    
                Debug.Log("Há itens em colunas erradas!");
            
            AcertosGarais = acertos;
        
            EnableFeedback();
        }
    
        private void EnableFeedback()
        {
            feedbackAviso.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
            continueButton.interactable = true;
            foreach (var button in feedbackButtons)
            {
                button.interactable = true;
            }
            tableButtons.SetActive(false);
            
            DesabilitarArrastaveis();
        }
        
        private void DesabilitarArrastaveis()
        {
            if (allDraggableItems == null || allDraggableItems.Length == 0)
            {
                allDraggableItems = FindObjectsByType<Arrastavel>(FindObjectsSortMode.None);
            }
            
            foreach (Arrastavel item in allDraggableItems)
            {
                if (item == null) continue;
                
                item.enabled = false;
                
                EventTrigger eventTrigger = item.GetComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    eventTrigger.enabled = false;
                }
                
                GraphicRaycaster raycaster = item.GetComponent<GraphicRaycaster>();
                if (raycaster != null)
                {
                    raycaster.enabled = false;
                }
                
                Collider[] colliders = item.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                }
           
                Collider2D[] colliders2D = item.GetComponents<Collider2D>();
                foreach (Collider2D collider in colliders2D)
                {
                    collider.enabled = false;
                }
            }
       
            Debug.Log("Todos os itens arrastáveis foram desabilitados");
        }
   
        public void OnZoneSelect(int idZone)
        {
            feedbackText.text = feedbacks[idZone];
            panelExplication.SetActive(true);
            continueButton.interactable = false;
        }
    
        public void DesactiveFeedback()
        {
            panelExplication.SetActive(false);
            continueButton.interactable = true;
        }

        public void OnContinueButtonPress()
        {
            StartCoroutine(SafeLoadScene());
        }
    
        private IEnumerator SafeLoadScene()
        {
            yield return new WaitForEndOfFrame();
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            carregarGameData();
            if(fase_1_minigame.acertos_garais + Fase1Parte2.AcertosGarais >= 6){
                MainManager.indiceCanvainicial = 14;
                yield return SaveGameSafely(true);
                LoadMainScene();
            }
            else{
                yield return SaveGameSafely(false);
                MainManager.indiceCanvainicial = 13;
                LoadMainScene();
            }
        }

        private void carregarGameData()
        {
            if (PlayerPrefs.HasKey("playerName"))
            {
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
            }
            else
            {
                Debug.LogError("Nenhum nome de jogador emboscada.");
                EmboscadaController.gameData.playerName = "Jogador";
            }
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
        }

        private void LoadMainScene()
        {
            try
            {
                SceneManager.LoadSceneAsync("main");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao carregar cena: {e.Message}");
            }
        }

        private IEnumerator SaveGameSafely(bool result)
        {
            yield return SaveGame(result);
        }
        private IEnumerator SaveGame(bool result)
        {
            yield return new WaitForEndOfFrame();

            if (EmboscadaController.gameData == null)
            {
                Debug.LogError("gameData é nulo!");
                yield break;
            }

            try
            {
                if (result)
                {
                    EmboscadaController.gameData.niveisganhos[0] = true;
                    EmboscadaController.gameData.classificacao += 1;
                    EmboscadaController.gameData.currentLevel = 15;
                    PlayerPrefs.SetInt("nivel0", 1);
                }
                else
                {
                    EmboscadaController.gameData.niveisganhos[0] = false;
                    EmboscadaController.gameData.currentLevel = 15;
                    PlayerPrefs.SetInt("nivel0", 0);
                }

                PlayerPrefs.SetInt("classificacao", (int)EmboscadaController.gameData.classificacao);
                PlayerPrefs.SetInt("currentLevel", 15);
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao salvar o jogo: {e.Message}");
            }
        }
        public void OnCancelButtonPress()
        {
            if (_isBeingDestroyed) return;
       
            if (tableButtons)
                tableButtons.SetActive(false);
       
            if (dropZones != null)
            {
                foreach (ZonaSoltar zone in dropZones)
                {
                    if (zone == null) continue;
                    zone.RemoveItem();
                }
            }
            if (allDraggableItems != null)
            {
                foreach (Arrastavel item in allDraggableItems)
                {
                    if (item == null) continue;
                    item.ReturnToInitialPosition();
                }
            }
            _confirmationProcessed = false; // Reset flag
            _cronometroAtivo = true;        // Reactivate timer
            _tempoRestante = tempoLimite;   // Reset timer value
            UpdateCronometro();             // Update timer display
            CheckAllItemsPlaced();          // Re-check if buttons should be active (likely not after reset)

            Debug.Log("Tudo resetado para as posições iniciais, pronto para nova tentativa.");
        }
    
        [ContextMenu("Debug Zone Status")]
        public void DebugZoneStatus()
        {
            if (_isBeingDestroyed || dropZones == null) return;
       
            for (int i = 0; i < dropZones.Length; i++)
            {
                if (dropZones[i] != null)
                    Debug.Log($"Zona {i}: HasItem={dropZones[i].HasItem()}, ItemId={dropZones[i].GetCurrentItemId()}, ZoneId={dropZones[i].GetZoneId()}");
                else
                    Debug.Log($"Zona {i}: NULL REFERENCE");
            }
       
            bool allZonesFilled = true;
            foreach (ZonaSoltar zone in dropZones)
            {
                if (zone == null || !zone.HasItem())
                {
                    allZonesFilled = false;
                    break;
                }
            }
       
            Debug.Log($"Todas as zonas preenchidas? {allZonesFilled}");
            Debug.Log($"Botão de confirmação ativo? {(confirmButton && confirmButton.gameObject ? confirmButton.gameObject.activeSelf : false)}");
            Debug.Log($"TableButtons ativo? {(tableButtons ? tableButtons.activeSelf : false)}");
        }
    
        // Método para pausar o timer (pode ser útil para tutoriais ou pausas)
        public void PausarTimer(bool pausar)
        {
            _cronometroAtivo = !pausar;
        }
    
        // Método para ajustar o tempo restante (pode ser útil para power-ups ou penalidades)
        public void AjustarTempoRestante(float segundos)
        {
            _tempoRestante += segundos;
            _tempoRestante = Mathf.Clamp(_tempoRestante, 0f, tempoLimite);
            UpdateCronometro();
        }
    }
}
