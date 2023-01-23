using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCoreScripts.InventorySystem
{
    public class InventoryItemTooltipPC : SerializedMonoBehaviour
    {
        [OdinSerialize] private InventoryUIPC _inventoryUI;
        [OdinSerialize] private Animator _animator;
        [OdinSerialize] private TMP_Text _nameText;
        [OdinSerialize] private Button _use;
        [OdinSerialize] private Button _hold;
        [OdinSerialize] private Button _drop;
        [OdinSerialize] private Button _combine;
        //[OdinSerialize] private Image _background;

        InventoryItemSlotPC _activeSlot; // the currently selected slot
        InventoryItemSlotPC _hoveredSlot; // the slot currently hovered 

        private IEnumerable<Button> AllButtons => new List<Button> { _use, _hold, _drop, _combine };

        public enum InventoryTooltipState
        {
            None,
            Name,
            Buttons
        }

        private InventoryTooltipState _state = InventoryTooltipState.None;
        private InventoryTooltipState _lastState = InventoryTooltipState.None;

        private void Awake()
        {
            Debug.Assert(_inventoryUI != null, "[InventoryItemTooltipPC]: Inventory tooltip requires a reference to an Inventory UI");

            _use.onClick.AddListener(Use);
            _hold.onClick.AddListener(Hold);
            _drop.onClick.AddListener(Drop);
            _combine.onClick.AddListener(Combine);

            _inventoryUI.OnShowUI += OnShowInventory;
            _inventoryUI.OnHideUI += OnHideInventory;
            _inventoryUI.OnSlotHoverEnter += OnSlotHoverEnter;
            _inventoryUI.OnSlotHoverExit += OnSlotHoverExit;
            _inventoryUI.OnSlotClick += OnSlotClick;
            _inventoryUI.OnSlotBeginDrag += OnSlotBeginDrag;
            _inventoryUI.OnBackgroundClick += OnBackgroundClick;

            // start in hidden state
            UpdateUI(InventoryTooltipState.None);
        }

        void OnShowInventory()
        {
        }

        void OnHideInventory()
        {
            UpdateUI(InventoryTooltipState.None);
        }

        void OnSlotHoverEnter(InventoryItemSlotPC itemSlot)
        {
            SetHoveredSlot(itemSlot);
        }

        void OnSlotHoverExit(InventoryItemSlotPC itemSlot)
        {
            if (_hoveredSlot == itemSlot)
                SetHoveredSlot(null);
        }

        void OnSlotClick(InventoryItemSlotPC itemSlot)
        {
            SetActiveSlot(itemSlot);
        }

        void OnSlotBeginDrag(InventoryItemSlotPC itemSlot)
        {
            SetActiveSlot(null);
        }

        void OnBackgroundClick()
        {
            SetActiveSlot(null);
        }

        void SetHoveredSlot(InventoryItemSlotPC itemSlot)
        {
            _hoveredSlot = itemSlot;

            if (_state != InventoryTooltipState.Buttons)
            {
                if (_hoveredSlot == null)
                    UpdateUI(InventoryTooltipState.None);
                else
                    UpdateUI(InventoryTooltipState.Name);
            }
        }

        void SetActiveSlot(InventoryItemSlotPC itemSlot)
        {
            _activeSlot = itemSlot;

            if (_activeSlot != null)
            {
                UpdateUI(InventoryTooltipState.Buttons);
                return;
            }

            // default back to hovered state if there is a hovered slot, otherwise, hide tooltip
            if (_hoveredSlot != null)
                UpdateUI(InventoryTooltipState.Name);
            else
                UpdateUI(InventoryTooltipState.None);
        }

        void UpdateUI(InventoryTooltipState state)
        {
            _lastState = _state;
            _state = state;
            switch (_state)
            {
                case InventoryTooltipState.None:
                    _animator.SetBool("IsShown", false);
                    break;
                case InventoryTooltipState.Name:
                    _nameText.text = _hoveredSlot.ItemStack.Item.ItemName;
                    foreach (var button in AllButtons) button.gameObject.SetActive(false);
                    _animator.SetBool("IsShown", true);
                    break;
                case InventoryTooltipState.Buttons:
                    _nameText.text = _activeSlot.ItemStack.Item.ItemName;
                    _use.gameObject.SetActive(_activeSlot.ItemStack.Item.CanUse);
                    _hold.gameObject.SetActive(_activeSlot.ItemStack.Item.CanHold);
                    _drop.gameObject.SetActive(_activeSlot.ItemStack.Item.CanDrop);
                    //_combine.gameObject.SetActive(_activeSlot.ItemStack.Item.HasCombine);
                    _animator.SetBool("IsShown", true);
                    break;
                default:
                    Debug.LogWarning($"Unknown tooltip state {_state} for inventory item tooltip: {gameObject.transform.parent.name}");
                    break;
            }

            //foreach (var button in AllButtons)
            //{
            //    var colors = button.colors;
            //    colors.normalColor = _inventoryUI.ColorPalette.Primary;
            //    colors.highlightedColor = _inventoryUI.ColorPalette.Secondary;
            //    colors.pressedColor = _inventoryUI.ColorPalette.Grey;
            //    button.colors = colors;
            //}
            //
            //_nameText.color = _inventoryUI.ColorPalette.Grey;
            //_background.color = _inventoryUI.ColorPalette.Text;
        }

        public void Use()
        {
            _inventoryUI.Inventory.TryUseItem(_activeSlot.ItemStack);

            if (_activeSlot.ItemStack.Count == 0)
                SetActiveSlot(null);
        }

        public void Hold()
        {
            _inventoryUI.Inventory.TryHoldItem(_activeSlot.ItemStack);

            if (_activeSlot.ItemStack.Count == 0)
                SetActiveSlot(null);
        }

        public void Drop()
        {
            _inventoryUI.Inventory.TryRemoveItem(_activeSlot.ItemStack);

            if (_activeSlot.ItemStack.Count == 0)
                SetActiveSlot(null);
        }

        public void Combine()
        {
            // TODO: Implement UI for this
        }
    }
}
