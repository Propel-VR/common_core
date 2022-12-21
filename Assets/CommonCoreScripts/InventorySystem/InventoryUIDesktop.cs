using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CommonCoreScripts.InventorySystem
{
    public class InventoryUIDesktop : InventoryUI
    {
        [SerializeField] private GameObject itemContainer;
        [SerializeField] private GameObject inventoryItemPrefab;
        [OdinSerialize] private Dictionary<ItemStack, InventoryItemSlotDesktop> _itemSlots = new();
        
        [SerializeField] private Image _inventoryBackground;
        [SerializeField] private Image _inventoryScrollBar;
        [SerializeField] private List<Image> _inventoryScrollButtons = new();
        
        [OdinSerialize] public InventoryItemTooltip Tooltip { get; set; }
        [OdinSerialize] public ItemStack CurrentItemStack { get; set; }

        public override void UpdateItems()
        {
            _itemSlots ??= new Dictionary<ItemStack, InventoryItemSlotDesktop>();

            foreach (var itemStack in Inventory.Items.Where(i => !_itemSlots.ContainsKey(i) && i.Item != null))
            {
                AddItem(itemStack);
            }

            var col = _itemSlots.Where(itemSlot => !Inventory.Items.Contains(itemSlot.Key));
            var toRemove = new List<ItemStack>();

            foreach (var itemSlot in col)
            {
                if (Inventory.Items.Contains(itemSlot.Key)) itemSlot.Value.UpdateUI();
                else
                {
                    SetCurrentItemHovered(null);
                    toRemove.Add(itemSlot.Key);
                }
            }
            
            foreach (var itemStack in toRemove)
            {
                RemoveItem(itemStack);
            }
            
            foreach (var itemSlot in _itemSlots)
            {
                itemSlot.Value.UpdateUI();
            }

            foreach (var scrollButton in _inventoryScrollButtons)
            {
                scrollButton.color = colorPalette.Primary;
            }
            _inventoryBackground.color = colorPalette.Text;
            _inventoryScrollBar.color = colorPalette.Grey;
        }

        public void Start()
        {
            UpdateItems();
        }

        private void RemoveItem(ItemStack itemSlotKey)
        {
            var itemSlot = _itemSlots[itemSlotKey];
            _itemSlots.Remove(itemSlotKey);
            Destroy(itemSlot.gameObject);
            
        }

        private void AddItem(ItemStack itemStack)
        {
            var itemSlot = Instantiate(inventoryItemPrefab, itemContainer.transform);
            var itemSlotScript = itemSlot.GetComponent<InventoryItemSlotDesktop>();
            itemSlotScript.ItemDisplayed = itemStack;
            itemSlotScript.InventoryUI = this;
            itemSlotScript.UpdateUI();
            _itemSlots.Add(itemStack, itemSlotScript);
        }
        
        public override void SetCurrentItemHovered(ItemStack itemStack)
        {
            CurrentItemStack = itemStack;
            Tooltip.ItemStack = itemStack;
            Tooltip.UpdateUI(itemStack == null ? 
                InventoryItemTooltip.InventoryTooltipState.None :
                InventoryItemTooltip.InventoryTooltipState.Name);
        }

        public override void OpenFullTooltip(ItemStack itemStack)
        {
            CurrentItemStack = itemStack;
            Tooltip.ItemStack = itemStack;
            Tooltip.UpdateUI( itemStack == default(ItemStack) ? 
                InventoryItemTooltip.InventoryTooltipState.None :
                InventoryItemTooltip.InventoryTooltipState.Buttons);
        }

        public void SetState(string status)
        {
            Enum.TryParse(status, out state);
        }
    }
}