using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class fase_1_minigame : MonoBehaviour
{
    // Botões corretos e errados definidos no inspector
    public List<GameObject> Buttons_corretos;
    public List<GameObject> Buttons_errados;

    // Lista para acompanhar quais botões foram selecionados pelo jogador
    public List<GameObject> Buttons_selecionados = new List<GameObject>();

    // Total de botões que devem ser selecionados
    [SerializeField] private int requiredSelections = 6;

    // Botão de confirmação (next)
    [SerializeField] private Button nextButton;

    // Cores para feedback visual
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    // Estado do jogo
    public static bool vitoria_fase_1;
    public static int acertos_garais = 0;

    // Flags para controle do estado do jogo
    private bool verificacaoRealizada = false;

    // Flag para controlar se já adicionamos os listeners
    private bool listenersConfigurados = false;

    void Awake()
    {
        // Inicialização básica
        Buttons_selecionados.Clear();
        vitoria_fase_1 = false;
        verificacaoRealizada = false;

        Debug.Log($"fase_1_minigame Awake: Encontrou {Buttons_corretos.Count} botões corretos e {Buttons_errados.Count} botões errados");
    }

    void Start()
    {
        // Configurar cores iniciais dos botões
        ResetarCoresBotoes();

        // Verificar se o botão Next está atribuído
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnPressSeguir);
            nextButton.interactable = false;
            Debug.Log("Botão Next configurado com sucesso");
        }
        else
        {
            Debug.LogError("Botão Next não está atribuído!");
        }

        // Verificar se todos os botões têm o componente BotaoSelecionavel
        VerificarComponentesBotoes();
    }

    // Novo método para verificar componentes
    private void VerificarComponentesBotoes()
    {
        List<GameObject> todosBotoes = new List<GameObject>();
        todosBotoes.AddRange(Buttons_corretos);
        todosBotoes.AddRange(Buttons_errados);
        todosBotoes = todosBotoes.Distinct().ToList();

        foreach (GameObject botao in todosBotoes)
        {
            if (botao == null)
            {
                Debug.LogError("Um dos botões na lista é null!");
                continue;
            }

            // Verificar se o botão tem o componente BotaoSelecionavel
            BotaoSelecionavel comp = botao.GetComponent<BotaoSelecionavel>();
            if (comp == null)
            {
                Debug.LogWarning($"O botão {botao.name} não tem o componente BotaoSelecionavel! Adicionando...");
                botao.AddComponent<BotaoSelecionavel>();
            }

            // Verificar se tem Button e Image
            Button btn = botao.GetComponent<Button>();
            Image img = botao.GetComponent<Image>();

            if (btn == null)
                Debug.LogError($"O botão {botao.name} não tem o componente Button!");

            if (img == null)
                Debug.LogError($"O botão {botao.name} não tem o componente Image!");
        }
    }

    // Configuração de botões - agora só usada se precisar adicionar listeners manualmente
    // Esta função não é mais chamada automaticamente
    public void ConfigurarBotoesManualmente()
    {
        if (listenersConfigurados) return;

        List<GameObject> todosBotoes = new List<GameObject>();
        todosBotoes.AddRange(Buttons_corretos);
        todosBotoes.AddRange(Buttons_errados);
        todosBotoes = todosBotoes.Distinct().ToList();

        foreach (GameObject botao in todosBotoes)
        {
            if (botao == null) continue;

            Button btnComponent = botao.GetComponent<Button>();
            if (btnComponent != null)
            {
                btnComponent.onClick.RemoveAllListeners(); // Remove listeners existentes
                btnComponent.onClick.AddListener(() => OnButtonClick(botao));
                Debug.Log($"Configurado listener para o botão {botao.name}");
            }
            else
            {
                Debug.LogError($"Botão {botao.name} não tem componente Button!");
            }
        }

        listenersConfigurados = true;
    }

    // Resetar cores dos botões para o estado inicial
    private void ResetarCoresBotoes()
    {
        List<GameObject> todosBotoes = new List<GameObject>();
        todosBotoes.AddRange(Buttons_corretos);
        todosBotoes.AddRange(Buttons_errados);
        todosBotoes = todosBotoes.Distinct().ToList();

        int contagem = 0;
        foreach (GameObject botao in todosBotoes)
        {
            if (botao == null) continue;

            Image imagem = botao.GetComponent<Image>();
            if (imagem != null)
            {
                imagem.color = normalColor;
                contagem++;
            }
        }

        Debug.Log($"Cores resetadas para {contagem} botões");
    }

    // O resto do código permanece igual...

    // Chamado quando um botão é clicado
    public void OnButtonClick(GameObject botao)
    {
        Debug.Log($"Clicou no botão: {botao.name}");

        // Se a verificação já foi realizada, ignoramos novos cliques
        if (verificacaoRealizada)
            return;

        Image imagemBotao = botao.GetComponent<Image>();

        // Verifica se o botão já está selecionado
        if (Buttons_selecionados.Contains(botao))
        {
            // Desseleciona o botão
            Buttons_selecionados.Remove(botao);
            if (imagemBotao != null) imagemBotao.color = normalColor;
            Debug.Log($"Botão {botao.name} desselecionado. Total: {Buttons_selecionados.Count}");
        }
        else
        {
            // Se ainda não atingiu o limite de seleções, seleciona o botão
            if (Buttons_selecionados.Count < requiredSelections)
            {
                Buttons_selecionados.Add(botao);
                if (imagemBotao != null) imagemBotao.color = selectedColor;
                Debug.Log($"Botão {botao.name} selecionado. Total: {Buttons_selecionados.Count}");
            }
            else
            {
                Debug.Log($"Você já selecionou {requiredSelections} botões!");
                // Opcional: exibir uma mensagem na UI informando o limite
            }
        }

        // Atualiza estado do botão de próximo
        if (nextButton != null)
        {
            // Habilita o botão Next apenas se o número exato de botões estiver selecionado
            nextButton.interactable = (Buttons_selecionados.Count == requiredSelections);
            Debug.Log($"NextButton interactable: {nextButton.interactable}");
        }
    }

    // Verifica se os botões selecionados estão corretos
    public void verificar_acertos()
    {
        int acertos = 0;

        foreach (GameObject button in Buttons_selecionados)
        {
            Image imagemBotao = button.GetComponent<Image>();

            if (Buttons_corretos.Contains(button))
            {
                acertos++;
                // Muda a cor para verde (correto)
                if (imagemBotao != null) imagemBotao.color = correctColor;
            }
            else if (Buttons_errados.Contains(button))
            {
                // Muda a cor para vermelho (errado)
                if (imagemBotao != null) imagemBotao.color = incorrectColor;
            }
        }

        // Destaca os botões corretos que não foram selecionados
        foreach (GameObject botaoCorreto in Buttons_corretos)
        {
            if (!Buttons_selecionados.Contains(botaoCorreto))
            {
                Image imagem = botaoCorreto.GetComponent<Image>();
                if (imagem != null)
                {
                    // Verde mais claro ou outro indicador para mostrar que era uma opção correta
                    Color corDestaque = new Color(0.7f, 1.0f, 0.7f);
                    imagem.color = corDestaque;
                }
            }
        }

        // Determina se o jogador acertou o suficiente para vencer
        if (acertos >= (int)(Buttons_corretos.Count / 2))
        {
            Debug.Log("Você acertou!");
            vitoria_fase_1 = true;
        }
        else
        {
            Debug.Log("Você errou!");
            vitoria_fase_1 = false;
        }

        acertos_garais = acertos;
        Debug.Log("Acertos: " + acertos_garais);

        // Marca que a verificação foi realizada
        verificacaoRealizada = true;
    }

    // Método chamado quando o botão de próximo é pressionado
    public void OnPressSeguir()
    {
        // Só permite avançar se o número correto de botões estiver selecionado
        if (Buttons_selecionados.Count != requiredSelections)
        {
            Debug.Log($"Selecione exatamente {requiredSelections} botões antes de continuar.");
            return;
        }

        // Executa a verificação e exibe feedback visual
        verificar_acertos();

        // Aqui você pode adicionar um pequeno delay antes de avançar para a próxima fase
        // para que o jogador possa ver o feedback visual
        Invoke("AvancarParaProximaFase", 2.0f);
    }

    // Método para avançar para a próxima fase após o delay
    private void AvancarParaProximaFase()
    {
        SceneManager.LoadScene("13_fase1_2minigame");
    }

    // Método para resetar o jogo (pode ser chamado por um botão de "Tentar Novamente")
    public void ResetarJogo()
    {
        Buttons_selecionados.Clear();
        verificacaoRealizada = false;
        ResetarCoresBotoes();

        if (nextButton != null)
        {
            nextButton.interactable = false;
        }
    }
}
