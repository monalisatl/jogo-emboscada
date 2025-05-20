using UnityEngine;
using UnityEngine.UI;

namespace Fase_5.Respescagem_Scritps.Fase_2
{
    public class InstructionUIControllerRepescagem : MonoBehaviour
    {
        [SerializeField] private GameObject canvasInstructions;
        [Header("Botão para iniciar ordenação")]
        public Button startButton;

        [Header("Prefab da parte cronológica")]
        public GameObject chronoPrefab;

        void Start()
        {
            startButton.onClick.AddListener(OnStart);
        }

        void OnStart()
        {
            var inst = Instantiate(chronoPrefab, null);
            inst.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            inst.GetComponent<Canvas>().worldCamera = Camera.main;
            Destroy(canvasInstructions);
        }
    }
}