using NaughtyAttributes;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using CommonCoreScripts.InventorySystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace CommonCoreScripts.InventorySystem
{
    [RequireComponent(typeof(Button))]
    public class InventoryItemSlotPC : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IItemDropHandler
    {
        [SerializeField]
        Image _iconImage;

        [SerializeField]
        TextMeshProUGUI _amountText;

        [Header("Events")]
        [SerializeField]
        UnityEvent _onHoverEnter;

        [SerializeField]
        UnityEvent _onHoverExit;

        InventoryUIPC _inventoryUI;
        Button _button;

        public ItemStack ItemStack { get; set; }
        //public bool ClickedHold { get => _clickedHold; set => _clickedHold = value; }

        private void Awake()
        {
            _inventoryUI = GetComponentInParent<InventoryUIPC>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);
        }

        public void UpdateUI()
        {
            _iconImage.sprite = ItemStack.Item.MenuSprite;
            _amountText.text = ItemStack.Count.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _inventoryUI.OnSlotHoverEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inventoryUI.OnSlotHoverExit?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_inventoryUI.Inventory.TryRemoveItem(ItemStack))
                return;

            _inventoryUI.SetCurrentlyDraggedItem(ItemStack.Item, this);

            _inventoryUI.OnSlotBeginDrag?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void RemoveItemFromSlot()
        {
            _inventoryUI.Inventory.TryRemoveItem(ItemStack);
        }

        public bool OnItemDrop(Item item)
        {
            if (TryCombineWith(item))
                return true;

            // if combine failed, try to add item back to inventory
            if (_inventoryUI.Inventory.TryAddItem(item))
                return true;

            return false;
        }

        bool TryCombineWith(Item item)
        {
            if (!item.CanCombineWith(ItemStack.Item))
                return false;

            // store items to call events later
            var stackItem = ItemStack.Item;
            var result = item.ResultOfCombineWith(stackItem);

            if (ItemStack.Count == 1)
            {
                ItemStack.Item = result;
                UpdateUI();
            }
            else
            {
                if (!_inventoryUI.Inventory.TryRemoveItem(ItemStack))
                    return false;

                if (!_inventoryUI.Inventory.TryAddItem(result))
                {
                    // if add failed, add back old item (this should succeed)
                    if (!_inventoryUI.Inventory.TryAddItem(ItemStack))
                        Debug.LogError("[InventoryItemSlotPC]: Unexpected fail when adding original item back to inventory after attempting combine");

                    return false;
                }
            }

            item.OnCombine?.Invoke();
            stackItem.OnCombine?.Invoke();

            Debug.Log($"Combined {item.ItemName} and {stackItem.ItemName} to create {result}");
            return true;
        }

        /// <summary>
        /// Handle button click
        /// </summary>
        void OnButtonClick()
        {
            _inventoryUI.OnSlotClick(this);
        }

        /// <summary>
        /// Handle right click
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _inventoryUI.Inventory.TryUseItem(ItemStack);
            }
        }
    }
}