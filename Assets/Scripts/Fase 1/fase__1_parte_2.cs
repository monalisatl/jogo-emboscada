using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class fase__1_parte_2 : MonoBehaviour
{
    public static fase__1_parte_2 Instance;
    
    [SerializeField] private ZonaSoltar[] dropZones;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private GameObject tableButtons;
    public static int acertos_garais = 0;
    
    [SerializeField] private Arrastavel[] allDraggableItems;
    
    private bool isBeingDestroyed = false;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        // Garantir que botões comecem desativados
        if (tableButtons)
            tableButtons.SetActive(false);
        if (confirmButton && confirmButton.gameObject)
            confirmButton.gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        isBeingDestroyed = true;
        
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
    }
    
    private IEnumerator DelayedInitCheck()
    {
        yield return null;
        
        CheckAllItemsPlaced();
    }

    public void CheckAllItemsPlaced()
    {
        if (isBeingDestroyed) return;
        
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
        if (isBeingDestroyed) return;
        
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
            }
            else
            {
                Debug.Log($"Item {itemId} está na zona errada {zoneId}");
            }
        }
        
        if (acertos == dropZones.Length+1)
            Debug.Log("Todos os itens estão nas colunas corretas!");
        else if (acertos >= (dropZones.Length + 1) / 2)
            Debug.Log("Alguns itens estão nas colunas corretas!");
        else    
            Debug.Log("Há itens em colunas erradas!");
            
        acertos_garais = acertos;
        
        StartCoroutine(SafeLoadScene(acertos));
    }
    
    private IEnumerator SafeLoadScene(int acertos)
    {
        yield return new WaitForEndOfFrame();
        
        try {
            if(fase_1_minigame.acertos_garais + fase__1_parte_2.acertos_garais >= 6){
                MainManager.indiceCanvainicial = 14;
                
                SceneManager.LoadSceneAsync("main");
            }
            else{
                MainManager.indiceCanvainicial = 13;
                SceneManager.LoadSceneAsync("main");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao carregar cena: {e.Message}");
        }
    }
    private IEnumerator SaveGame(bool result)
    {
        yield return new WaitForEndOfFrame();
        
        if (EmboscadaController.gameData == null)
        {
            Debug.LogError("gameData é nulo!");
            yield break;
        }
        if (result)
        {
            EmboscadaController.gameData.niveisganhos[0] = true;
            EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Estagiário;
            EmboscadaController.gameData.currentLevel = 15;
            PlayerPrefs.SetInt("nivel"+0, 1);
            PlayerPrefs.SetInt("classificacao", 1);
            PlayerPrefs.SetInt("currentLevel", 15);
            PlayerPrefs.Save();
        }
        else
        {
            EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Amador;
            
        }
    }
    public void OnCancelButtonPress()
    {
        if (isBeingDestroyed) return;
        
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
        
        Debug.Log("Tudo resetado para as posições iniciais");
    }
    
    [ContextMenu("Debug Zone Status")]
    public void DebugZoneStatus()
    {
        if (isBeingDestroyed || dropZones == null) return;
        
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
}
