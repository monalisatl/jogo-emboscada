using System;
using System.Collections;
using Fase_5;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_5.Respescagem_Scritps.Fase_4
{
    public class Fase4VitoriaScript : MonoBehaviour
    {
        [SerializeField] private GameObject pista;
        [SerializeField] private GameObject load;

        private void Awake()
        {
            pista.SetActive(false);
        }

        void Start()
        {
            OnSaveVitoria();
        }
        
        void OnSaveVitoria()
        {
            Fase5Comeco.Repescagens[2] = true;
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisRepescagem[2] = true;
            PlayerPrefs.SetInt("repescagem3", 1);
            PlayerPrefs.SetInt("currentLevel", 83);
            RepescagemManager.SetLoadingPagePrefab(load);
            RepescagemManager.CheckAllRepescagensComplete();
        }

        public void OpenPista()
        {
            pista.SetActive(true);
        }

        public void ClosePistaAndLoadNextScene()
        {
            RepescagemManager.SetLoadingPagePrefab(load);
            RepescagemManager.CheckAllRepescagensComplete();
        }
    }
}