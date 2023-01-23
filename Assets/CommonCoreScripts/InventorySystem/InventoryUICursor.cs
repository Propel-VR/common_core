using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;


namespace CommonCoreScripts.InventorySystem
{

    public class InventoryUICursor : MonoBehaviour
    {
        #region Static fields

        static InventoryUICursor _current;

        public static InventoryUICursor Current => _current;

        #endregion

        [SerializeField]
        LayerMask _raycastLayerMask;

        public Action<Item> OnBeginDragItem { get; set; }

        public Action<Item> OnReleaseItem { get; set; }

        public Action<Item> OnCatchItemNotHandled { get; set; }

        Item _currentHeldItem;

        public Item CurrentDraggedItem => _currentHeldItem;

        private void Awake()
        {
            if (_current != null)
                Debug.LogWarning("[InventoryUICursor]: There are multiple InventoryUICursor's in the scene, this is not currently supported. Please ensure only once cursor is loaded at a time");
            
            _current = this;
        }

        public void BeginDragItem(Item item)
        {
            OnBeginDragItem?.Invoke(item);

            _currentHeldItem = item;
            Vector2 cursorHotspot = new Vector2(item.CursorHotspot.x * item.Cursor.width, item.CursorHotspot.y * item.Cursor.height);
            Cursor.SetCursor(item.Cursor, cursorHotspot, CursorMode.ForceSoftware);
        }

        public void ReleaseItem()
        {
            OnReleaseItem?.Invoke(_currentHeldItem);

            _currentHeldItem = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void Update()
        {
            if (_currentHeldItem && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, raycastResults);

                foreach (var result in raycastResults)
                {
                    foreach (var dropArea in result.gameObject.GetComponents<IItemDropHandler>())
                    {
                        if (dropArea != null)
                        {
                            if (dropArea.OnItemDrop(_currentHeldItem))
                            {
                                ReleaseItem();
                                return;
                            }
                        }
                    }
                }

                if (_currentHeldItem)
                {
                    OnCatchItemNotHandled?.Invoke(_currentHeldItem);
                    ReleaseItem();
                }
            }

            //if (_currentHeldItem != null && Mouse.current.leftButton.wasReleasedThisFrame)
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    
            //    if (Physics.Raycast(ray, out RaycastHit hit, 100, _raycastLayerMask, QueryTriggerInteraction.Collide))
            //    {
            //        foreach (var handler in hit.transform.GetComponents<IItemDropHandler>())
            //        {
            //            if (handler != null)
            //                handler.OnItemDrop(_currentHeldItem);
            //
            //            // check if item was used
            //            if (_currentHeldItem == null)
            //                break;
            //        }
            //    }
            //
            //    // if item was not used, drop it
            //    if (_currentHeldItem != null)
            //        DropHeldItem();
            //}
        }

    }

}