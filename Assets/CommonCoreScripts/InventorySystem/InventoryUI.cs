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
    public abstract class InventoryUI : SerializedMonoBehaviour
    {
        [OdinSerialize]
        protected Inventory _inventory;

        public Inventory Inventory 
        { 
            get => _inventory;
            set
            {
                if (_inventory)
                {
                    _inventory.OnInventoryChanged -= UpdateUI;
                    _inventory.OnOpenInventory -= OpenUI;
                    _inventory.OnCloseInventory -= CloseUI;
                }

                _inventory = value;

                if (_inventory)
                {
                    _inventory.OnInventoryChanged += UpdateUI;
                    _inventory.OnOpenInventory += OpenUI;
                    _inventory.OnCloseInventory += CloseUI;
                }
            }
        }

        public abstract void OpenUI();

        public abstract void CloseUI();

        public abstract void UpdateUI();
    }
}