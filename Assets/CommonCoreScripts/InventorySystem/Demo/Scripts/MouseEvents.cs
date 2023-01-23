using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;


namespace CommonCore.InventorySystem.Demo
{

    public class MouseEvents : MonoBehaviour
    {

        [SerializeField]
        UnityEvent _onMouseDown;

        [SerializeField]
        UnityEvent _onMouseUp;

        [SerializeField]
        UnityEvent _onMouseOver;

        [SerializeField]
        UnityEvent _onMouseEnter;

        [SerializeField]
        UnityEvent _onMouseExit;

        public Action onMouseDown;
        public Action onMouseUp;
        public Action onMouseOver;
        public Action onMouseEnter;
        public Action onMouseExit;

        public void OnMouseDown()
        {
            _onMouseDown?.Invoke();
            onMouseDown?.Invoke();
        }

        public void OnMouseUp()
        {
            _onMouseUp?.Invoke();
            onMouseUp?.Invoke();
        }

        public void OnMouseOver()
        {
            _onMouseOver?.Invoke();
            onMouseOver?.Invoke();
        }

        public void OnMouseEnter()
        {
            _onMouseEnter?.Invoke();
            onMouseEnter?.Invoke();
        }

        public void OnMouseExit()
        {
            _onMouseExit?.Invoke();
            onMouseExit?.Invoke();
        }
    }

}
