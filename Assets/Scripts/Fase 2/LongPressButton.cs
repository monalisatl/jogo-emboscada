using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Fase_2
{
    public class LongPressButton : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public float holdThreshold = 0.5f;
        public UnityEvent onLongPress = new UnityEvent();

        private bool isPointerDown = false;
        private float pointerDownTimer = 0f;

        void Update()
        {
            if (isPointerDown)
            {
                pointerDownTimer += Time.deltaTime;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            pointerDownTimer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isPointerDown && pointerDownTimer >= holdThreshold)
            {
                onLongPress.Invoke();
            }
            Reset();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            Reset();
        }

        private void Reset()
        {
            isPointerDown = false;
            pointerDownTimer = 0f;
        }
    }
}