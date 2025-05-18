using UnityEngine;
using UnityEngine.EventSystems;

namespace Fase_5.Respescagem_Scritps.Fase_1
{
    public class ZonaSoltaRepescagem : MonoBehaviour, IDropHandler
    {
        [SerializeField] private int zoneId;
        [SerializeField] private RectTransform snapPosition; // Posição onde o item deve se encaixar
    
        private ArrastavelRepescagem currentItem = null;
    
        // Verificação de segurança
        private bool isDestroying = false;

        public void OnDrop(PointerEventData eventData)
        {
            if (isDestroying) return;
        
            // Proteção contra nulos
            if (eventData == null || eventData.pointerDrag == null)
                return;
            
            // Obter o componente do item arrastável
            GameObject droppedObject = eventData.pointerDrag;
            if (droppedObject == null)
                return;
            
            ArrastavelRepescagem newItem = droppedObject.GetComponent<ArrastavelRepescagem>();
            if (newItem == null)
                return;
            
            // Verificar se já existe um item na coluna
            if (currentItem != null && currentItem != newItem)
            {
                // Prevenir referências circulares
                ArrastavelRepescagem itemToReturn = currentItem;
            
                // Limpe a referência antes de retornar
                currentItem = null;
            
                // Agora retorne o item anterior para sua posição
                itemToReturn.ReturnToInitialPosition();
            }
        
            // Define o novo item (seja ele novo ou o mesmo de antes)
            currentItem = newItem;
        
            // Usa o RectTransform do snapPosition ou o próprio transform
            Transform targetTransform = snapPosition ? snapPosition : transform;
            newItem.OnDroppedInZone(targetTransform, Vector2.zero);
        
            // Verificar estado de todos os itens
            SafeCheckAllItemsPlaced();
        }

        // Método seguro para verificar se todos os itens foram colocados
        private void SafeCheckAllItemsPlaced()
        {
            if (Fase1Parte2.Instance != null)
            {
                try
                {
                    Fase1Parte2.Instance.CheckAllItemsPlaced();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Erro ao verificar itens: {e.Message}");
                }
            }
        }

        public void RemoveItem()
        {
            currentItem = null;
            SafeCheckAllItemsPlaced();
        }
    
        public bool HasItem()
        {
            return currentItem != null;
        }
    
        public int GetZoneId()
        {
            return zoneId;
        }
    
        public int GetCurrentItemId()
        {
            return currentItem ? currentItem.GetItemId() : -1;
        }
    
        public ArrastavelRepescagem GetCurrentItem()
        {
            return currentItem;
        }
    
        // Gerenciamento seguro de destruição
        private void OnDestroy()
        {
            isDestroying = true;
            currentItem = null;
        }
    }
}
