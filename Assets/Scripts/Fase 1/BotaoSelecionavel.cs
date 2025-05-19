using UnityEngine;
using UnityEngine.UI;

namespace Fase_1
{
    [RequireComponent(typeof(Button))]
    public class BotaoSelecionavel : MonoBehaviour
    {
        private fase_1_minigame gameManager;
        private Button button;
        private Image imagem;
    
        private void Awake()
        {
            // Mover inicialização para Awake para garantir que aconteça antes
            button = GetComponent<Button>();
            imagem = GetComponent<Image>();
        }
    
        private void Start()
        {
            // Verificar se já encontrou o gameManager
            if (gameManager == null)
            {
                gameManager = FindAnyObjectByType<fase_1_minigame>();
            
                if (gameManager == null)
                {
                    Debug.LogError($"BotaoSelecionavel em {gameObject.name}: Não conseguiu encontrar o fase_1_minigame!");
                    return;
                }
            }
        
            // Remover listeners existentes para evitar duplicação
            button.onClick.RemoveAllListeners();
        
            // Adicionar o listener
            button.onClick.AddListener(() => {
                if (gameManager != null)
                    gameManager.OnButtonClick(gameObject);
                else
                    Debug.LogError($"BotaoSelecionavel em {gameObject.name}: gameManager é null!");
            });
        
            // Certificar que este botão esteja nas listas corretas
            Debug.Log($"Botão {gameObject.name} inicializado com sucesso.");
        }
    }
}
