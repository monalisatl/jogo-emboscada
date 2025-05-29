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
        if (EmboscadaController.gameData.classificacao < EmboscadaController.Classificacao.Amador)
            EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Amador;
        else if (EmboscadaController.gameData.classificacao > EmboscadaController.Classificacao.Sênior)
            EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Sênior;
        PlayerPrefs.SetInt("classificacao", (int)EmboscadaController.gameData.classificacao);
        EmboscadaController.gameData.selectedCharacterId = PlayerPrefs.GetInt("selectedCharacterId", 0);
        EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Detetive");
        EmboscadaController.gameData.currentLevel = 100;
        PlayerPrefs.SetInt("currentLevel", 100);
        PlayerPrefs.Save();
        return EmboscadaController.gameData;
    }

    private void ConfigurarCertificado()
    {
       
        Sprite[] certificadosArray;
        if ( (int)_gameData.classificacao < 0)
            _gameData.classificacao = EmboscadaController.Classificacao.Amador;
        if ((int)_gameData.classificacao > 3)
            _gameData.classificacao = EmboscadaController.Classificacao.Sênior;
        certificadosArray = _gameData.classificacao switch
        {
            EmboscadaController.Classificacao.Amador => certificadosAmador,
            EmboscadaController.Classificacao.Estagiário => certificadosEstagiario,
            EmboscadaController.Classificacao.Júnior => certificadosJunior,
            EmboscadaController.Classificacao.Sênior => certificadosSenior,
            _ => certificadosAmador
        };

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

