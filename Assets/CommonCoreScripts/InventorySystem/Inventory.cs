using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities.Editor;
#endif

namespace CommonCoreScripts.InventorySystem
{
    public class Inventory : SerializedMonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Holds items and their count in the inventory
        /// </summary>
        [ListDrawerSettings(CustomAddFunction = "EditorAddItemStack")]
        [ValidateInput("ValidateItems", "Stacks must assign an item and must have a count between 1 and the stack size of that item")]
        [OdinSerialize] private List<ItemStack> _items = new();

#if UNITY_EDITOR
        ItemStack EditorAddItemStack()
        {
            return new ItemStack(null);
        }

        bool ValidateItems()
        {
            foreach (var itemStack in _items)
                if (itemStack == null || itemStack.Item == null || itemStack.Count <= 0 || itemStack.Count > itemStack.Item.StackSize)
                    return false;

            return true;
        }
#endif

        /// <summary>
        /// Maximum number of slots in the inventory
        /// </summary>
        [SerializeField] private int _maxSlots = 27;

        [Header("Events")]

        /// <summary>
        /// Event fired whenever the items in the inventory change
        /// </summary>
        [SerializeField] UnityEvent _onInventoryChanged;

        /// <summary>
        /// Fired whenever the inventory is opened via the Open method on the Inventory class 
        /// </summary>
        [SerializeField] UnityEvent _onOpenInventory;

        /// <summary>
        /// Fired whenever the inventory is closed via the Close method on the Inventory class 
        /// </summary>
        [SerializeField] UnityEvent _onCloseInventory;

        #endregion

        #region Public Accessors

        /// <summary>
        /// Event fired whenever the items in the inventory change
        /// </summary>
        public Action OnInventoryChanged { get; set; }

        /// <summary>
        /// Fired whenever the inventory is opened via the Open method on the Inventory class 
        /// </summary>
        public Action OnOpenInventory { get; set; }

        /// <summary>
        /// Fired whenever the inventory is closed via the Close method on the Inventory class 
        /// </summary>
        public Action OnCloseInventory { get; set; }

        /// <summary>
        /// This inventory's items
        /// </summary>
        public List<ItemStack> Items => _items;

        #endregion

        /// <summary>
        /// Checks if the inventory contains the item.
        /// </summary>
        /// <param name="item">The item to be searched.</param>
        /// <returns>True if the inventory contains the item, false otherwise.</returns>
        public bool ContainsItem(Item item)
        {
            return _items.Any(x => x.Item == item);
        }

        /// <summary>
        /// Adds an item to the inventory
        /// </summary>
        /// <param name="item">Item type to be added to the inventory.</param>
        /// <returns><c>True</c> if the item was successfully added, <c>False</c> if not.</returns>
        public bool TryAddItem(Item item)
        {
            if (item == null)
                return false;

            if (_items.Any(x => x.Item == item && x.Count < item.StackSize))
            {
                var stack = _items.Find(x => x.Item == item && x.Count < item.StackSize);

                stack.Count++;
            }
            else
            {
                if (_items.Count >= _maxSlots) 
                    return false;

                _items.Add(new ItemStack(item));
            }

            RefreshUI();

            return true;
        }

        /// <summary>
        /// Add an item to the given stack in this inventory.
        /// 
        /// If the stack is full, item will be added normally (or return false if could not).
        /// </summary>
        /// <param name="stack">Stack to add item to. Assumed to be in this Inventory.</param>
        /// <returns><c>True</c> if the item was successfully added, <c>False</c> if not.</returns>
        public bool TryAddItem(ItemStack stack)
        {
            if (stack == null)
                return false;

            if (!stack.IsFull)
            {
                stack.Count++;
                RefreshUI();
                return true;
            }

            // stack is full so will try to add another one
            return TryAddItem(stack.Item);
        }
        
        /// <summary>
        /// Removes an item from the inventory
        /// </summary>
        /// <param name="item">The item type to be removed</param>
        /// <returns><c>True</c> if the item was successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItem(Item item)
        {
            if (item == null)
                return false;

            var stack = _items.Find(x => x.Item == item && x.Count > 0);

            if (stack == null) 
                return false;
                
            stack.Count--;
                
            if (stack.Count == 0) 
                _items.Remove(stack);
            
            RefreshUI();
                
            return true;
        }

        /// <summary>
        /// Remove an item from a stack in this inventory.
        /// 
        /// If the stack is empty, will fail.
        /// </summary>
        /// <param name="stack">Stack to remove item from. Assumed to be in this Inventory.</param>
        /// <returns><c>True</c> if the item was successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItem(ItemStack stack)
        {
            // sanity check
            if (stack.Count <= 0)
                return false;

            // remove item (and possibly stack as well)
            stack.Count--;

            if (stack.Count == 0) 
                _items.Remove(stack);

            RefreshUI();
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
            for (int i = 0; i < count; i++)
            {
                if (!TryRemoveItem(item))
                    return false;
            }
            
            RefreshUI();

            return true;
        }

        /// <summary>
        /// Removes an amount of the same item from the inventory, starting with
        /// the given stack.
        /// </summary>
        /// <param name="stack">The stack whose items should be removed first. Assumed to be in this inventory</param>
        /// <param name="count">The amount of items to be removed</param>
        /// <returns><c>True</c> if the items were successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItems(ItemStack stack, int count)
        {
            int remaining = count;

            while (stack.Count > 0 && remaining > 0)
            {
                if (TryRemoveItem(stack))
                {
                    remaining--;
                }
                else
                {
                    _items.Remove(stack);
                    stack = _items.Find(s => s.Item == stack.Item);

                    if (stack == null) 
                        return false;
                }
            }

            RefreshUI();

            return remaining == 0;
        }

        /// <summary>
        /// Tries to use <paramref name="item"/>, removing one from the inventory if successful.
        /// </summary>
        /// <param name="item">The item to be used</param>
        /// <returns>The result of the operation; True if the item was successfully used, false otherwise</returns>
        public bool TryUseItem(Item item)
        {
            // if the item is not null and is usable...
            if (item is not { CanUse: true }) return false;
            // and the item is either consumable and able to be removed...
            if (item.IsConsumable)
            {
                if (!TryRemoveItem(item)) return false;
            }
            // or the item is not consumable but contained in the inventory...
            else if (!ContainsItem(item)) return false;
            
            // then use the item, and return true; otherwise, return false
            item.OnUse?.Invoke();
            
            RefreshUI();

            Debug.Log($"Used {item.ItemName}");
            return true;
        }

        /// <summary>
        /// Tries to use an item from the given stack, removing one from it if successful.
        /// 
        /// If the stack is empty, will fail.
        /// </summary>
        /// <param name="stack">The stack to use, asssumed to be in this inventory</param>
        /// <returns>The result of the operation; True if the item was successfully used, false otherwise</returns>
        public bool TryUseItem(ItemStack stack)
        {
            // sanity checks
            if (stack.Item == null || !stack.Item.CanUse)
                return false;

            if (stack.Count == 0)
                return false;

            // consume item if it is consumable
            if (stack.Item.IsConsumable)
                if (!TryRemoveItem(stack)) 
                    return false;

            // use item
            stack.Item.OnUse?.Invoke();

            RefreshUI();

            Debug.Log($"Used {stack.Item.ItemName}");
            return true;
        }

        /// <summary>
        /// Tries to hold <paramref name="item"/>, removing one from the inventory if successful.
        /// NOTE: the item holding implementation must be handled by the <see cref="Item"/>'s <see cref="Item.OnHold"/>
        /// event (spawn in worldspace, spawn in hand, add to UI as sprite for desktop, etc).
        /// </summary>
        /// <param name="item">The item to be held</param>
        /// <returns>The result of the operation; True if the item was successfully held, false otherwise</returns>
        public bool TryHoldItem(Item item)
        {
            // checks
            if (item is not { CanHold: true }) return false;

            // hold and remove item
            if (!TryRemoveItem(item)) 
                return false;

            item.OnHold?.Invoke();
            
            RefreshUI();

            Debug.Log($"Held {item.ItemName}");
            return true;
        }

        /// <summary>
        /// Tries to hold an item from the given stack. 
        /// 
        /// If the stack is empty, will fail.
        /// </summary>
        /// <param name="stack">The stack to use, assumed to be in this inventory</param>
        /// <returns>The result of the operation; True if the item was successfully held, false otherwise</returns>
        public bool TryHoldItem(ItemStack stack)
        {
            // sanity checks
            if (stack.Item == null || !stack.Item.CanHold)
                return false;

            // hold and remove item
            if (!TryRemoveItem(stack)) 
                return false;

            stack.Item.OnHold?.Invoke();

            RefreshUI();

            Debug.Log($"Held {stack.Item.ItemName}");
            return true;
        }

        /// <summary>
        /// Tries to combine two items in the inventory.
        /// </summary>
        /// <param name="item">one of the items to combine</param>
        /// <param name="other">the other item to combine</param>
        /// <returns>The result of the operation; True if the item was successfully held, false otherwise</returns>
        public bool TryCombineItems(Item item, Item other)
        {
            // sanity checks
            if (!item.CanCombineWith(other)) 
                return false;

            if (!ContainsItem(item) || !ContainsItem(other)) 
                return false;

            // remove both items and add new result
            if (!TryRemoveItem(item) || !TryRemoveItem(other))
                return false;

            if (!TryAddItem(item.ResultOfCombineWith(other)))
            {
                // if add failed, add old items back (this should succeed)
                if (!TryAddItem(item) || !TryAddItem(other))
                    Debug.LogError("[Inventory]: Unexpected fail when adding original item back to inventory after attempting combine");

                return false;
            }
            

            // call events
            item.OnCombine?.Invoke();
            other.OnCombine?.Invoke();
            
            RefreshUI();

            Debug.Log($"Combined {item.ItemName} and {other.ItemName} to create {item.ResultOfCombineWith(other)}");
            return true;
        }

        /// <summary>
        /// Tries to combine an item in the inventory with one in the given stack.
        /// 
        /// If the stack has only 1 item, that same stack will be used to hold the new item
        /// </summary>
        /// <param name="item">the item type to combine (needs to be in the inventory)</param>
        /// <param name="stack">the stack holding the second item to try and combine, assumed to be in the inventory</param>
        /// <returns>The result of the operation; True if the item was successfully held, false otherwise</returns>
        public bool TryCombineItems(Item item, ItemStack stack)
        {
            // sanity checks
            if (item == null || stack.Item == null)
                return false;

            if (!ContainsItem(item) || stack.Count == 0)
                return false;

            if (!item.CanCombineWith(stack.Item))
                return false;

            // get item from inventory
            if (!TryRemoveItem(item))
                return false;

            // get each item involved in combination (so we can call events later)
            Item stackItem = stack.Item;
            Item result = item.ResultOfCombineWith(stack.Item);

            // remove 1 item from each stack and add the result item
            if (stack.Count == 1)
            {
                // if stack has only 1 item, simply change it's item to be the new result item
                stack.Item = result;
            }
            else
            {
                if (!TryRemoveItem(stack))
                    return false;

                if (!TryAddItem(result))
                {
                    // if add failed, add old item back (this should succeed)
                    if (!TryAddItem(item) || !TryAddItem(stack))
                        Debug.LogError("[Inventory]: Unexpected fail when adding original item back to inventory after attempting combine");

                    return false;
                }
            }

            item.OnCombine?.Invoke();
            stackItem.OnCombine?.Invoke();

            RefreshUI();

            Debug.Log($"Combined {item.ItemName} and {stackItem.ItemName} to create {item.ResultOfCombineWith(stackItem)}");
            return true;
        }

        /// <summary>
        /// Tries to combine items from two stacks in the inventory
        /// 
        /// If a stack has only 1 item, that same stack will be used to hold the new item
        /// </summary>
        /// <param name="stack1">the stack holding the first item to try and combine, assumed to be in the inventory</param>
        /// <param name="stack2">the stack holding the second item to try and combine, assumed to be in the inventory</param>
        /// <returns>The result of the operation; True if the item was successfully held, false otherwise</returns>
        public bool TryCombineItems(ItemStack stack1, ItemStack stack2)
        {
            // sanity checks
            if (stack1.Item == null || stack2.Item == null)
                return false;

            if (stack1.Count == 0 || stack2.Count == 0)
                return false;

            if (!stack1.Item.CanCombineWith(stack2.Item))
                return false;

            // get each item involved in combination (so we can call events later)
            Item item1 = stack1.Item;
            Item item2 = stack2.Item;
            Item result = item1.ResultOfCombineWith(item2);

            // remove 1 item from each stack and add the result item
            // (if a stack has only 1 item, change it's item to be the new result item)
            if (stack1.Count == 1)
            {
                if (!TryRemoveItem(stack2))
                    return false;

                stack1.Item = result;
            }
            else if (stack2.Count == 1)
            {
                if (!TryRemoveItem(stack1))
                    return false;

                stack2.Item = result;
            }
            else
            {
                if (!TryRemoveItem(stack1) || !TryRemoveItem(stack2))
                    return false;

                if (!TryAddItem(result))
                {
                    // if add failed, add old items back (this should succeed)
                    if (!TryAddItem(stack1) || !TryAddItem(stack2))
                        Debug.LogError("[Inventory]: Unexpected fail when adding original item back to inventory after attempting combine");

                    return false;
                }
            }

            item1.OnCombine?.Invoke();
            item2.OnCombine?.Invoke();

            RefreshUI();

            Debug.Log($"Combined {item1.ItemName} and {item2} to create {item1.ResultOfCombineWith(item2)}");
            return true;
        }

        /// <summary>
        /// Open the inventory (events only)
        /// </summary>
        public void Open()
        {
            _onOpenInventory?.Invoke();
            OnOpenInventory?.Invoke();
        }

        /// <summary>
        /// Close the inventory (events only)
        /// </summary>
        public void Close()
        {
            _onCloseInventory?.Invoke();
            OnCloseInventory?.Invoke();
        }

        /// <summary>
        /// Refreshes any inventory UI (events only)
        /// </summary>
        public void RefreshUI()
        {
            _onInventoryChanged?.Invoke();
            OnInventoryChanged?.Invoke();
        }

        #region Debug

        [Header("Debug")]
        [OdinSerialize] private Item _debugItem;

        [Button("Test Add")]
        public void TestAdd()
        {
            TryAddItem(_debugItem);
        }

        #endregion
    }

    [Serializable]
    [InlineEditor]
    public class ItemStack
    {
        [OdinSerialize] [ShowInInspector] private Item _item;
        [OdinSerialize] [ShowInInspector] private int _count;
        
        //[HorizontalGroup("Split")]
        public Item Item { get => _item; set => _item = value; }
        public int Count { get => _count; set => _count = value; }

        public bool IsFull => Count == Item.StackSize;
        
        public ItemStack(Item i)
        {
            _item = i;
            _count = 1;
        }
    }
}