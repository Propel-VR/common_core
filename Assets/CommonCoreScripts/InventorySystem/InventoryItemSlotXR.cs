using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;
using Autohand;

namespace CommonCoreScripts.InventorySystem
{
    public class InventoryItemSlotXR : MonoBehaviour
    {
        [SerializeField]
        Transform _itemParent;

        [SerializeField]
        TextMeshProUGUI _amountText;

        [Header("Events")]
        [SerializeField]
        UnityEvent _onHoverEnter;

        [SerializeField]
        UnityEvent _onHoverExit;

        InventoryUI _inventoryUI;
        List<Hand> _hands = new();
        GameObject _item;

        public ItemStack ItemStack { get; set; }

        private void Awake()
        {
            _inventoryUI = GetComponentInParent<InventoryUI>();
        }

        public void UpdateUI()
        {
            _amountText.text = ItemStack.Count.ToString();

            if (!_item && ItemStack.Count > 0 && _hands.Count == 0)
            {
                _item = Instantiate(ItemStack.Item.PhysicalModel);
                StartCoroutine(DelaySetParent(_item));

                Grabbable grabbable = _item.GetComponent<Grabbable>();
                Debug.Assert(grabbable, $"Item {_item.name} must have Grabbable component to be used with XR inventory system");
                grabbable.OnGrabEvent += OnGrabItem;

                Rigidbody rb = _item.GetComponent<Rigidbody>();
                Debug.Assert(grabbable, $"Item {_item.name} must have Rigidbody component to be used with XR inventory system");
                rb.isKinematic = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                Hand hand = other.GetComponent<Hand>();

                if (hand && _hands.Count == 0)
                    _onHoverEnter?.Invoke();

                if (hand)
                    _hands.Add(hand);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                Hand hand = other.GetComponent<Hand>();

                if (hand)
                    _hands.Remove(hand);

                if (hand && _hands.Count == 0)
                {
                    _onHoverExit?.Invoke();
                    UpdateUI();
                }
            }
        }

        void OnGrabItem(Hand hand, Grabbable item)
        {
            item.transform.parent = null;
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.OnGrabEvent -= OnGrabItem;
            _item = null;

            // for XR, grabbing the item counts as holding and as using the item
            if (ItemStack.Item.CanHold)
                ItemStack.Item.OnHold?.Invoke();

            if (ItemStack.Item.CanUse)
                ItemStack.Item.OnUse?.Invoke();

            if (ItemStack.Item.IsConsumable)
                _inventoryUI.Inventory.TryRemoveItem(ItemStack);
        }

        /// <summary>
        /// Waits for the next frame, then sets the parent of the given item to the 
        /// slot item container.
        /// 
        /// Workaround for Autohand Grabbables, since in the Grabbable Awake method, 
        /// the originalParent is set to the item's then current parent, which we do not 
        /// want to be the slot container, otherwise when releasing the item, it's parent 
        /// will be set back to the container (this cannot be simply fixed by setting
        /// parent to null on release, since by then the scale/position/rotation could
        /// have changed).
        /// </summary>
        IEnumerator DelaySetParent(GameObject item)
        {
            yield return new WaitForEndOfFrame();

            // item will keep its position and rotationn as a relative position and rotation to its parent
            item.transform.SetParent(_itemParent, false);
        }
    }

}
