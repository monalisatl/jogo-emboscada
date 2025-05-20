using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Fase_2
{
    public class LongPressButton : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler
    {
        public float holdThreshold = 0.5f;
        public UnityEvent onLongPress =  new UnityEvent();
        private bool isPointerDown = false;
        private float pointerDownTimer = 0f;

        void Update()
        {
            if (isPointerDown)
            {
                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer >= holdThreshold)
                {
                    isPointerDown = false;
                    pointerDownTimer = 0f;
                    onLongPress?.Invoke();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            pointerDownTimer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            pointerDownTimer = 0f;
        }
    }
}