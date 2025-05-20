using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;

public class CertificadoManager : MonoBehaviour
{
    [Header("Referências UI")] [SerializeField]
    private Image imagemCertificado;

    [SerializeField] private TextMeshProUGUI textoNomeJogador;
    [SerializeField] private TextMeshProUGUI textoData;
    [SerializeField] private Button botaoMenuPrincipal;

    [Header("Imagens de Certificados")] [SerializeField]
    private Sprite[] certificadosAmador;

    [SerializeField] private Sprite[] certificadosEstagiario;
    [SerializeField] private Sprite[] certificadosJunior;
    [SerializeField] private Sprite[] certificadosSenior;

    [Header("Configurações")] [SerializeField]
    private string cenaMenuPrincipal = "MainMenu";

    private EmboscadaController.GameData _gameData;

    void Start()
    {
        // Carrega os dados do jogador
        _gameData = LoadGameData();

        // Configura a imagem do certificado correta
        ConfigurarCertificado();

        // Preenche os textos
        PreencherTextos();

        // Configura os botões
        ConfigurarBotoes();
    }

    private EmboscadaController.GameData LoadGameData()
    {
        EmboscadaController.gameData ??= new EmboscadaController.GameData();
        EmboscadaController.gameData.classificacao =
            (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
        EmboscadaController.gameData.selectedCharacterId = PlayerPrefs.GetInt("selectedCharacterId", 0);
        EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Detetive");
        EmboscadaController.gameData.currentLevel = 100;
        PlayerPrefs.SetInt("currentLevel", 100);
        PlayerPrefs.Save();
        return EmboscadaController.gameData;
    }

    private void ConfigurarCertificado()
    {
        // Seleciona o array correto baseado na classificação
        Sprite[] certificadosArray;

        switch (_gameData.classificacao)
        {
            case EmboscadaController.Classificacao.Amador:
                certificadosArray = certificadosAmador;
                break;
            case EmboscadaController.Classificacao.Estagiário:
                certificadosArray = certificadosEstagiario;
                break;
            case EmboscadaController.Classificacao.Júnior:
                certificadosArray = certificadosJunior;
                break;
            case EmboscadaController.Classificacao.Sênior:
                certificadosArray = certificadosSenior;
                break;
            default:
                certificadosArray = certificadosAmador;
                break;
        }

        int personagemIndex = Mathf.Clamp(_gameData.selectedCharacterId, 0, certificadosArray.Length - 1);
        imagemCertificado.sprite = certificadosArray[personagemIndex];
    }

    private void PreencherTextos()
    {
        if (textoNomeJogador != null)
        {
            textoNomeJogador.text = _gameData.playerName;
        }

        if (textoData != null)
        {
            textoData.text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    private void ConfigurarBotoes()
    {

        if (botaoMenuPrincipal != null)
        {
            botaoMenuPrincipal.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(cenaMenuPrincipal);
            });
        }
    }


}

