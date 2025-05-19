using UnityEngine;
using UnityEngine.EventSystems;

namespace Fase_1
{
    public class Arrastavel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private int itemId;
    
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Vector2 initialPosition;
        private Transform initialParent;
        private Vector2 originalPosition;
        private Transform originalParent;
        private bool isDropped = false;
    
        // Propriedade para verificação de segurança
        private bool isBeingDestroyed = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        
            // Garantir que CanvasGroup seja adicionado corretamente
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        
            // Salvar a posição inicial e parent quando o jogo começa
            if (rectTransform != null)
                initialPosition = rectTransform.anchoredPosition;
        
            initialParent = transform.parent;
        }

        // Garantir referências mais seguras ao Canvas
        private Canvas GetValidCanvas()
        {
            if (canvas == null)
            {
                // Tente encontrar o canvas pai
                canvas = GetComponentInParent<Canvas>();
            
                // Se ainda for nulo, tente encontrar qualquer canvas na cena
                if (canvas == null)
                {
                    canvas = FindAnyObjectByType<Canvas>();
                
                    if (canvas == null)
                    {
                        Debug.LogError("Nenhum Canvas encontrado na cena para o objeto arrastável: " + gameObject.name);
                    }
                }
            }
        
            return canvas;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isBeingDestroyed) return;
        
            // Se já estiver em uma coluna, remova-se de lá
            if (isDropped && transform.parent != null)
            {
                var currentDropZone = transform.parent.GetComponent<ZonaSoltar>();
                if (currentDropZone != null)
                {
                    currentDropZone.RemoveItem();
                }
                isDropped = false;
            }
        
            // Salve posição original para caso o drop não seja válido
            originalPosition = rectTransform.anchoredPosition;
            originalParent = transform.parent;
        
            Canvas validCanvas = GetValidCanvas();
            if (validCanvas != null)
            {
                // Use o método seguro para mudar o parent
                SafeSetParent(validCanvas.transform);
            }
        
            // Reduza a opacidade e desabilite bloqueio de raycasts durante arrasto
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0.6f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isBeingDestroyed || rectTransform == null) return;
        
            Canvas validCanvas = GetValidCanvas();
            if (validCanvas != null)
            {
                // Atualize a posição com base no movimento do mouse
                rectTransform.anchoredPosition += eventData.delta / validCanvas.scaleFactor;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isBeingDestroyed) return;
        
            // Restaure valores padrão
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
        
            // Se não foi dropado em uma zona válida, retorne à posição original
            if (!isDropped)
            {
                SafeSetParent(originalParent);
                if (rectTransform != null)
                    rectTransform.anchoredPosition = originalPosition;
            }
        
            // Verificar se todos os itens foram colocados
            if (Fase1Parte2.Instance != null)
                Fase1Parte2.Instance.CheckAllItemsPlaced();
        }

        // Método seguro para definir o parent
        private void SafeSetParent(Transform newParent)
        {
            if (isBeingDestroyed || transform == null || newParent == null) return;
        
            try
            {
                transform.SetParent(newParent);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao definir parent para {gameObject.name}: {e.Message}");
            }
        }

        // Método chamado pela DropZone quando o item for aceito
        public void OnDroppedInZone(Transform parentTransform, Vector2 newPosition)
        {
            if (isBeingDestroyed) return;
        
            isDropped = true;
            SafeSetParent(parentTransform);
        
            if (rectTransform != null)
                rectTransform.anchoredPosition = newPosition;
        
            // Notificar o gerenciador que um item foi posicionado
            if (Fase1Parte2.Instance != null)
                Fase1Parte2.Instance.CheckAllItemsPlaced();
        }

        // Método para forçar retorno à posição INICIAL do jogo
        public void ReturnToInitialPosition()
        {
            if (isBeingDestroyed) return;
        
            isDropped = false;
            SafeSetParent(initialParent);
        
            if (rectTransform != null)
                rectTransform.anchoredPosition = initialPosition;
        
            // Garantir que valores do CanvasGroup sejam restaurados
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
        }

        public int GetItemId()
        {
            return itemId;
        }
    
        // Tratamento para destruição correta
        private void OnDestroy()
        {
            isBeingDestroyed = true;
        }
    }
}
