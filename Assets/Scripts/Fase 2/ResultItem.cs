using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ResultItem : MonoBehaviour {
   [SerializeField] private Button button;
   [SerializeField] private Text label;
   [SerializeField] private Image background;
   public void Setup(int zoneId, bool isCorrect, UnityEngine.Events.UnityAction onClick) {
       label.text = "Campo " + zoneId;
       background.color = isCorrect ? Color.green : Color.red;
       button.onClick.RemoveAllListeners(); button.onClick.AddListener(onClick);
   }
}
