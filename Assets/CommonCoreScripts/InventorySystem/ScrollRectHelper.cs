using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace CommonCoreScripts.InventorySystem
{

    /// <summary>
    /// Stops the ScrollRect drag if we are currently dragging an item from the inventory
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectHelper : MonoBehaviour, IDragHandler
    {
        ScrollRect _scrollRect;
        InventoryUIPC _inventoryUI;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _inventoryUI = GetComponentInParent<InventoryUIPC>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_inventoryUI.CurrentDraggedItem != null)
                eventData.pointerDrag = null;
        }
    }

}
