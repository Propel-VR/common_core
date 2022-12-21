using System;
using System.Collections.Generic;
using CommonCoreScripts.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CommonCoreScripts.InventorySystem
{
    /// <summary>
    /// Abstract class representing inventory UI.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class InventoryUI : SerializedMonoBehaviour
    {
        [OdinSerialize] [ShowInInspector] [Required] protected Animator animator;
        [OdinSerialize] [ShowInInspector] [Required] protected Inventory inventory;

        [OdinSerialize] [ShowInInspector] [Required] [AssetsOnly] protected ColorPalette colorPalette;

        public Animator Animator { get => animator; }
        public Inventory Inventory 
        { 
            get => inventory;
            set
            {
                inventory = value;
            }
        }
        public ColorPalette ColorPalette => colorPalette;
        public InventoryItemTooltip Tooltip { get; protected set; }
        
        public enum UIState
        {
            None,
            Hovered,
            Clicked,
            Combining
        }

        protected UIState state;

        public UIState State
        {
            get => state;
            set => state = value;
        }

        public abstract void UpdateItems();
        public abstract void SetCurrentItemHovered(ItemStack itemStack);
        public abstract void OpenFullTooltip(ItemStack itemStack);
    }
}