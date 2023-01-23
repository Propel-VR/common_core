using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CommonCoreScripts.InventorySystem
{

    public class InventoryUIXR : InventoryUI
    {
        [SerializeField]
        Transform _itemContainer;

        [SerializeField]
        InventoryItemSlotXR _inventoryItemSlotPrefab;

        List<InventoryItemSlotXR> _itemSlots = new();

        Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            foreach (Transform child in _itemContainer)
                Destroy(child.gameObject);

            // setting property will subscribe necessary events
            Inventory = _inventory;

            CloseUI();
        }

        public override void OpenUI()
        {
            if (_animator)
                _animator.SetBool("IsOpen", true);

            UpdateUI();
        }

        public override void CloseUI()
        {
            if (_animator)
                _animator.SetBool("IsOpen", false);
        }

        public override void UpdateUI()
        {
            if (Inventory == null)
            {
                Debug.Log("[InventoryUIXR]: Inventory UI must reference an Inventory");
                return;
            }

            // add new slots
            foreach (var itemStack in Inventory.Items)
            {
                if (!_itemSlots.Exists(slot => slot.ItemStack == itemStack))
                {
                    var newSlot = Instantiate(_inventoryItemSlotPrefab, _itemContainer);
                    newSlot.ItemStack = itemStack;
                    _itemSlots.Add(newSlot);
                }
            }

            // remove unnecessary slots
            List<InventoryItemSlotXR> itemSlotsCopy = _itemSlots.ToList();

            foreach (var itemSlot in itemSlotsCopy)
            {
                if (!Inventory.Items.Exists(item => item == itemSlot.ItemStack))
                {
                    _itemSlots.Remove(itemSlot);
                    Destroy(itemSlot.gameObject);
                }
            }

            // update slots
            foreach (var itemSlot in _itemSlots)
                itemSlot.UpdateUI();
        }
    }
}
