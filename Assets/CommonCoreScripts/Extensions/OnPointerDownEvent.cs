using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace CommonCoreScripts.Extensions
{

    public class OnPointerDownEvent : MonoBehaviour, IPointerDownHandler
    {
        UnityEvent _onPointerDown;

        public Action onPointerDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            _onPointerDown?.Invoke();
            onPointerDown?.Invoke();
        }
    }

}
