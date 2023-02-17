using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using CommonCoreScripts.Extensions;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace CommonCoreScripts.InventorySystem
{
    public class InventoryUIPC : InventoryUI
    {
        #region Serialized Fields

        [SerializeField]
        InventoryUICursor _cursor;

        [SerializeField]
        InventoryItemSlotPC _inventoryItemSlotPrefab;

        [SerializeField]
        Transform _itemContainer;

        [SerializeField]
        Image _background;

        [Header("Events")]
        [SerializeField]
        UnityEvent _onShowUI;

        [SerializeField]
        UnityEvent _onHideUI;

        #endregion

        #region Private Fields

        bool _isOpen = false;
        bool _wasOpenedThisFrame = false; // used to determine if clicked outside the UI

        Animator _animator;

        List<InventoryItemSlotPC> _itemSlots = new();

        InventoryItemSlotPC _heldItemSlot; // the slot that the currently held item came from

        #endregion

        #region Public Accessors

        public Item CurrentDraggedItem => _cursor.CurrentDraggedItem;

        public Action OnShowUI { get; set; }

        public Action OnHideUI { get; set; }

        public Action<InventoryItemSlotPC> OnSlotHoverEnter { get; set; }

        public Action<InventoryItemSlotPC> OnSlotHoverExit { get; set; }

        public Action<InventoryItemSlotPC> OnSlotClick { get; set; }

        public Action<InventoryItemSlotPC> OnSlotBeginDrag { get; set; }

        public Action OnBackgroundClick { get; set; }

        #endregion

        #region Monobehaviour Methods

        private void Awake()
        {
            if (_background)
                _background.gameObject.AddComponent<OnPointerDownEvent>().onPointerDown += () => OnBackgroundClick?.Invoke();

            _animator = GetComponent<Animator>();

            foreach (Transform child in _itemContainer)
                Destroy(child.gameObject);

            // setting property will subscribe necessary events
            Inventory = _inventory;

            CloseUI();
        }

        private void OnEnable()
        {
            _cursor.OnReleaseItem += OnCursorReleaseItem;
            _cursor.OnCatchItemNotHandled += OnCursorCatchItemNotHandled;
        }

        private void OnDisable()
        {
            _cursor.OnReleaseItem -= OnCursorReleaseItem;
            _cursor.OnCatchItemNotHandled -= OnCursorCatchItemNotHandled;
        }

        private void Update()
        {
            if (_isOpen && !_wasOpenedThisFrame && Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
                Inventory.Close();

            _wasOpenedThisFrame = false;
        }

        #endregion

        public override void OpenUI()
        {
            if (_isOpen)
                return;

            if (_animator)
                _animator.SetBool("IsOpen", true);

            UpdateUI();

            _isOpen = true;
            _wasOpenedThisFrame = true;

            _onShowUI?.Invoke();
            OnShowUI?.Invoke();
        }

        public override void CloseUI()
        {
            if (!_isOpen)
                return;

            if (_animator)
                _animator.SetBool("IsOpen", false);

            _isOpen = false;

            _onHideUI?.Invoke();
            OnHideUI?.Invoke();
        }

        public override void UpdateUI()
        {
            Debug.Assert(Inventory != null, "[InventoryUIPC]: Inventory UI must reference an Inventory");

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
            List<InventoryItemSlotPC> itemSlotsCopy = _itemSlots.ToList();

            foreach (var itemSlot in itemSlotsCopy)
            {
                if (!Inventory.Items.Exists(item => item == itemSlot.ItemStack))
                {
                    _itemSlots.Remove(itemSlot);
                    Destroy(itemSlot.gameObject);
                }
            }

            foreach (var itemSlot in _itemSlots)
                itemSlot.UpdateUI();

            //foreach (var scrollButton in _inventoryScrollButtons)
            //{
            //    scrollButton.color = colorPalette.Primary;
            //}
            //_inventoryBackground.color = colorPalette.Text;
            //_inventoryScrollBar.color = colorPalette.Grey;
        }

        public void SetCurrentlyDraggedItem(Item item, InventoryItemSlotPC itemSlot = null)
        {
            _cursor.BeginDragItem(item);
            _heldItemSlot = itemSlot;
        }

        public void OnCursorReleaseItem(Item item)
        {
            _heldItemSlot = null;
        }

        public void OnCursorCatchItemNotHandled(Item item)
        {
            if (_heldItemSlot != null)
                Inventory.TryAddItem(_heldItemSlot.ItemStack);
            else
                Inventory.TryAddItem(item);
        }
    }
}