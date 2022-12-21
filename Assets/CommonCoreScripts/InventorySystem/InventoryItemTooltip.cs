using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCoreScripts.InventorySystem
{
    public class InventoryItemTooltip : SerializedMonoBehaviour
    {
        [OdinSerialize] private InventoryUI _inventoryUI;
        [OdinSerialize] [NonSerialized] private ItemStack _itemStack;
        [OdinSerialize] private Animator _animator;
        [OdinSerialize] private TMP_Text _nameText;
        [OdinSerialize] private Button _use;
        [OdinSerialize] private Button _hold;
        [OdinSerialize] private Button _drop;
        [OdinSerialize] private Button _combine;
        [OdinSerialize] private Image _background;

        public ItemStack ItemStack
        {
            get => _itemStack;
            set
            {
                _itemStack = value;
            }
        }

        private IEnumerable<Button> AllButtons => new List<Button> {_use, _hold, _drop, _combine};
    
        public enum InventoryTooltipState
        {
            None,
            Name,
            Buttons
        }
    
        private InventoryTooltipState _state = InventoryTooltipState.None;
        private InventoryTooltipState _lastState = InventoryTooltipState.None;

        public void UpdateUI(InventoryTooltipState state)
        {
            _lastState = _state;
            _state = state;
            switch (_state)
            {
                case InventoryTooltipState.None:
                    _animator.SetTrigger("Hide");
                    foreach (var button in AllButtons) button.gameObject.SetActive(false);
                    break;
                case InventoryTooltipState.Name:
                    _nameText.text = _itemStack.Item.ItemName;
                    foreach (var button in AllButtons) button.gameObject.SetActive(false);
                    _animator.SetTrigger(_lastState == InventoryTooltipState.Buttons && _state != InventoryTooltipState.Buttons ? "HideButtons" : "ShowName");
                    break;
                case InventoryTooltipState.Buttons:
                    _nameText.text = _itemStack.Item.ItemName;
                    if (_itemStack.Item.HasUse) _use.gameObject.SetActive(true);
                    if (_itemStack.Item.HasHold) _hold.gameObject.SetActive(true);
                    if (_itemStack.Item.HasDrop) _drop.gameObject.SetActive(true);
                    if (_itemStack.Item.HasCombine) _combine.gameObject.SetActive(true);
                    _animator.SetTrigger("ShowButtons");
                    break;
                default:
                    Debug.LogWarning($"Unknown tooltip state {_state} for inventory item tooltip: {gameObject.transform.parent.name}");
                    break;
            }

            foreach (var button in AllButtons)
            {
                var colors = button.colors;
                colors.normalColor = _inventoryUI.ColorPalette.Primary;
                colors.highlightedColor = _inventoryUI.ColorPalette.Secondary;
                colors.pressedColor = _inventoryUI.ColorPalette.Grey;
                button.colors = colors;
            }

            _nameText.color = _inventoryUI.ColorPalette.Grey;
            _background.color = _inventoryUI.ColorPalette.Text;
        }

        public void UpdateUI(string stateName)
        {
            if (!Enum.TryParse(stateName, out InventoryTooltipState state))
            {
                Debug.LogWarning($"Unknown tooltip state {stateName} for inventory item tooltip: {gameObject.transform.parent.name}");
                return;
            }
            UpdateUI(state);
        }
        
        public void Use()
        {
            var itemName = _itemStack.Item.ItemName;
            if (!_inventoryUI.Inventory.TryUse(_itemStack.Item)) return;
            // TODO: notify player on result
            Debug.Log($"Used {itemName}");
        }
        
        public void Hold()
        {
            var itemName = _itemStack.Item.ItemName;
            if (!_inventoryUI.Inventory.TryHold(_itemStack.Item)) return;
            // TODO: notify player on result
            Debug.Log($"Held {itemName}");
        }
        
        public void Drop()
        {
            var itemName = _itemStack.Item.ItemName;
            if (!_inventoryUI.Inventory.TryRemoveItemFromStack(_itemStack)) return;
            // TODO: notify player on result
            Debug.Log($"Dropped {itemName}");
        }
        
        public void Combine()
        {
            // TODO: Implement UI for this
        }
    }
}
