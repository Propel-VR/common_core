using NaughtyAttributes;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CommonCoreScripts.InventorySystem
{
    public class InventoryItemSlotDesktop : MonoBehaviour
    {
        [OdinSerialize] private ItemStack _itemDisplayed;
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI amountText;
        
        [SerializeField] private Button _button;
        
        [SerializeField] private InventoryUI _inventoryUI;

        private bool _clickedHold = false;
        public InventoryUI InventoryUI { get => _inventoryUI; set => _inventoryUI = value; }
        public ItemStack ItemDisplayed {get => _itemDisplayed; set => _itemDisplayed = value;}
        public bool ClickedHold { get => _clickedHold; set => _clickedHold = value; }

        public void UpdateUI()
        {
            if (iconSprite == null) SpriteUpdate();
            iconImage.sprite = iconSprite;
            amountText.text = ItemDisplayed.Count.ToString();
            
            var colorBlock = _button.colors;
            colorBlock.normalColor = InventoryUI.ColorPalette.Grey;
            colorBlock.highlightedColor = InventoryUI.ColorPalette.Secondary;
            colorBlock.pressedColor = InventoryUI.ColorPalette.Primary;
            _button.colors = colorBlock;
            
            amountText.color = InventoryUI.ColorPalette.White;
        }
        
        public void SpriteUpdate()
        {
            iconSprite = ItemVisualHandler.Instance.GetItemVisuals(ItemDisplayed.Item)?.MenuSprite;
        }

        public void OnHoverEnter()
        {
            InventoryUI.SetCurrentItemHovered(ItemDisplayed);
            InventoryUI.State = InventoryUI.UIState.Hovered;
        }
        
        public void OnHoverExit()
        {
            if (InventoryUI.State == InventoryUI.UIState.Clicked) return;
            InventoryUI.SetCurrentItemHovered(default(ItemStack));
            InventoryUI.State = InventoryUI.UIState.None;
        }
        
        public void OnClick()
        {
            InventoryUI.OpenFullTooltip(ItemDisplayed);
            InventoryUI.State = InventoryUI.UIState.Clicked;
        }
    }
}