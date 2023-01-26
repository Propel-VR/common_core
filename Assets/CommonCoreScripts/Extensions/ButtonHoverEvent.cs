using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CommonCoreScripts.Extensions
{
    public class ButtonHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent onHoverEnter;
        [SerializeField] private UnityEvent onHoverExit;
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            onHoverEnter.Invoke();    
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverExit.Invoke();
        }
    }
}