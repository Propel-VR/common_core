using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace CommonCoreScripts.InventorySystem
{
    public class Inventory : SerializedMonoBehaviour
    {
        /// <summary>
        /// Holds items and their count in the inventory
        /// </summary>
        [OdinSerialize] private List<ItemStack> items = new();
        public List<ItemStack> Items => items;
        
        [OdinSerialize] private InventoryUI inventoryUI;

        [OdinSerialize] private bool _isPlayerInventory;
        public bool IsPlayerInventory => _isPlayerInventory;
        
        public UnityEvent OnInventoryChanged = new ();

        /// <summary>
        /// Maximum number of slots in the inventory
        /// </summary>
        [SerializeField] private int _maxSlots = 27;
        
        [SerializeField] private Item debugItem;

        /// <summary>
        /// Adds an item to the inventory
        /// </summary>
        /// <param name="item">Item to be added to the inventory.</param>
        /// <returns><c>True</c> if the item was successfully added, <c>False</c> if not.</returns>
        public bool TryAddItem(Item item)
        {
            if (items.Any(x => x.Item == item))
            {
                var stack = items.FirstOrDefault(x => x.Item == item && x.Count < item.StackSize);
                
                if (EqualityComparer<ItemStack>.Default.Equals(stack, default))
                {
                    stack.Count++;
                    return true;
                }
            }

            if (items.Count >= _maxSlots) return false;
            
            items.Add(new ItemStack(item));
            
            OnInventoryChanged.Invoke();
            
            return true;
        }
        
        /// <summary>
        /// Removes an item from the inventory
        /// </summary>
        /// <param name="item">The item type to be removed</param>
        /// <returns><c>True</c> if the item was successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItem(Item item)
        {
            if (items.All(x => x.Item != item)) return false;

            var stack = items.FirstOrDefault(x => x.Item == item && x.Count > 0);
            if (EqualityComparer<ItemStack>.Default.Equals(stack, default)) return false;
                
            stack.Count--;
                
            if (stack.Count == 0) items.Remove(stack);
            
            OnInventoryChanged.Invoke();
                
            return true;
        }
        
        /// <summary>
        /// Removes an amount of the same item from the inventory
        /// </summary>
        /// <param name="item">The item type to be removed</param>
        /// <param name="count">The amount of items to be removed</param>
        /// <returns><c>True</c> if the items were successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItems(Item item, int count)
        {
            var itemCount = items.Where(x => x.Item == item).Sum(x => x.Count);
            if (count < itemCount) return false;
            
            for (int i = 0; i < count; i++)
            {
                TryRemoveItem(item);
            }
            
            OnInventoryChanged.Invoke();

            return true;
        }

        /// <summary>
        /// Removes an item from the itemStack
        /// </summary>
        /// <param name="s">The itemStack from which to remove the item</param>
        /// <returns><c>True</c> if the item was successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItemFromStack(ItemStack s)
        {
            s.Count--;
            if (s.Count == 0) items.Remove(s);
            
            OnInventoryChanged.Invoke();
            
            return true;
        }
        
        /// <summary>
        /// Checks if the inventory contains the item.
        /// </summary>
        /// <param name="item">The item to be searched.</param>
        /// <returns>True if the inventory contains the item, false otherwise.</returns>
        public bool ContainsItem(Item item)
        {
            return items.Any(x => x.Item == item);
        }

        /// <summary>
        /// Tries to use <paramref name="item"/>, removing one from the inventory if successful.
        /// </summary>
        /// <param name="item">The item to be used</param>
        /// <returns>The result of the operation; True if the item was successfully used, false otherwise</returns>
        public bool TryUse(Item item)
        {
            // if the item is not null and is usable...
            if (item is not { HasUse: true }) return false;
            // and the item is either consumable and able to be removed...
            if (item.IsConsumable)
            {
                if (!TryRemoveItem(item)) return false;
            }
            // or the item is not consumable but contained in the inventory...
            else if (!ContainsItem(item)) return false;
            
            // then use the item, and return true; otherwise, return false
            item.OnUse?.Invoke();
            
            OnInventoryChanged.Invoke();
            
            return true;
        }

        /// <summary>
        /// Tries to hold <paramref name="item"/>, removing one from the inventory if successful.
        /// NOTE: the item holding implementation must be handled by the <see cref="Item"/>'s <see cref="Item.OnHold"/>
        /// event (spawn in worldspace, spawn in hand, add to UI as sprite for desktop, etc).
        /// </summary>
        /// <param name="item">The item to be held</param>
        /// <returns>The result of the operation; True if the item was successfully held, false otherwise</returns>
        public bool TryHold(Item item)
        {
            // if the item is not null and is holdable...
            if (item is not { HasHold: true }) return false;
            // and the item is able to be removed...
            if (!TryRemoveItem(item)) return false;
            // then call hold events, and return true; otherwise, return false
            item.OnHold?.Invoke();
            
            OnInventoryChanged.Invoke();
            
            return true;
        }

        
        public bool TryCombine(Item item, Item other)
        {
            // if the item is not null and is combinable...
            if (item is not { HasCombine: true }) return false;
            // and the item is able to be combined with the other item...
            if (!item.CanCombineWith(other)) return false;
            // and the items are both in the inventory...
            if (ContainsItem(item) && ContainsItem(other)) return false;
            
            // then remove both items...
            TryRemoveItem(item);
            TryRemoveItem(other);
            // add new item to inventory...
            TryAddItem(item.CombineResults[other]);
            // call combine events...
            item.OnCombine?.Invoke();
            other.OnCombine?.Invoke();
            
            OnInventoryChanged.Invoke();
            
            // and return true
            return true;
            // otherwise, return false
        }
        
        [Button("Test Add")]
        public void TestAdd()
        {
            TryAddItem(debugItem);
        }

        public void RefreshUI()
        {
            inventoryUI.UpdateItems();
        }
    }

    [Serializable]
    public class ItemStack
    {
        [OdinSerialize] [ShowInInspector] private Item _item;
        [OdinSerialize] [ShowInInspector] private int _count;
        
        public Item Item { get => _item; set => _item = value; }
        public int Count { get => _count; set => _count = value; }
        
        public ItemStack(Item i)
        {
            _item = i;
            _count = 1;
        }
    }
}