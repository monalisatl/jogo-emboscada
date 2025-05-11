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
    
    // Flag para controle de destruição
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
        // Evitar problemas ao destruir a instância
        isBeingDestroyed = true;
        
        // Se este objeto for a instância atual, limpe a referência estática
        if (Instance == this)
            Instance = null;
    }
    
    private void Start()
    {
        // Se não tiver a lista de items arrastáveis, tente encontrá-los automaticamente
        if (allDraggableItems == null || allDraggableItems.Length == 0)
        {
            allDraggableItems = FindObjectsByType<Arrastavel>(FindObjectsSortMode.None);
        }
        
        // Inicializar os botões se necessário
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
        
        // Verificar estado inicial com pequeno delay para garantir inicialização
        StartCoroutine(DelayedInitCheck());
    }
    
    // Coroutine para verificação com delay
    private IEnumerator DelayedInitCheck()
    {
        // Espera um frame para garantir que tudo está inicializado
        yield return null;
        
        // Verifica os estados dos objetos
        CheckAllItemsPlaced();
    }

    public void CheckAllItemsPlaced()
    {
        if (isBeingDestroyed) return;
        
        // Proteção contra nulos
        if (dropZones == null || dropZones.Length == 0)
        {
            Debug.LogWarning("Não há zonas de drop definidas!");
            return;
        }
        
        bool allZonesFilled = true;
        
        // Verifica se todas as zonas têm itens
        foreach (ZonaSoltar zone in dropZones)
        {
            if (zone == null || !zone.HasItem())
            {
                allZonesFilled = false;
                break;
            }
        }
        
        // Gerenciar visibilidade dos botões com base no estado
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
        
        // Proteção contra referências nulas
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
        
        // Execute ações baseadas no resultado
        if (acertos == dropZones.Length+1)
            Debug.Log("Todos os itens estão nas colunas corretas!");
        else if (acertos >= (dropZones.Length + 1) / 2)
            Debug.Log("Alguns itens estão nas colunas corretas!");
        else    
            Debug.Log("Há itens em colunas erradas!");
            
        acertos_garais = acertos;

        // Usar um método seguro para carregar a cena
        StartCoroutine(SafeLoadScene(acertos));
    }
    
    // Método seguro para carregamento de cena
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

    public void OnCancelButtonPress()
    {
        if (isBeingDestroyed) return;
        
        // Fecha o painel de botões
        if (tableButtons)
            tableButtons.SetActive(false);
        
        // Limpar todas as zonas com proteção contra nulos
        if (dropZones != null)
        {
            foreach (ZonaSoltar zone in dropZones)
            {
                if (zone == null) continue;
                zone.RemoveItem();
            }
        }
        
        // Retornar todos os itens para suas posições iniciais
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
        
        // Verificar estado atual e mostrar no console
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
