using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CommonCoreScripts.InventorySystem
{
    public class Inventory : SerializedMonoBehaviour
    {
        /// <summary>
        /// Holds items and their count in the inventory
        /// </summary>
        private List<Stack> Items = new();

        /// <summary>
        /// Maximum number of slots in the inventory
        /// </summary>
        [SerializeField] private int _maxSlots = 27;

        /// <summary>
        /// Adds an item to the inventory
        /// </summary>
        /// <param name="item">Item to be added to the inventory.</param>
        /// <returns><c>True</c> if the item was successfully added, <c>False</c> if not.</returns>
        public bool TryAddItem(Item item)
        {
            if (Items.Any(x => x.Item == item))
            {
                var stack = Items.FirstOrDefault(x => x.Item == item && x.Count < item.stackSize);
                
                if (stack !=null)
                {
                    stack.Count++;
                    return true;
                }
            }

            if (Items.Count >= _maxSlots) return false;
            
            Items.Add(new Stack(item));
            return true;
        }
        
        /// <summary>
        /// Removes an item from the inventory
        /// </summary>
        /// <param name="item">The item type to be removed</param>
        /// <returns><c>True</c> if the item was successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItem(Item item)
        {
            if (Items.All(x => x.Item != item)) return false;

            var stack = Items.FirstOrDefault(x => x.Item == item && x.Count > 0);
            if (stack == null) return false;
                
            stack.Count--;
                
            if (stack.Count == 0) Items.Remove(stack);
                
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
            var itemCount = Items.Where(x => x.Item == item).Sum(x => x.Count);
            if (count < itemCount) return false;
            
            for (int i = 0; i < count; i++)
            {
                TryRemoveItem(item);
            }

            return true;
        }

        /// <summary>
        /// Removes an item from the stack
        /// </summary>
        /// <param name="s">The stack from which to remove the item</param>
        /// <returns><c>True</c> if the item was successfully removed, <c>False</c> if not.</returns>
        public bool TryRemoveItemFromStack(Stack s)
        {
            s.Count--;
            if (s.Count == 0) Items.Remove(s);
            return true;
        }
    }

    public class Stack
    {
        public Item Item;
        public int Count;
        
        public Stack(Item i)
        {
            Item = i;
            Count = 1;
        }
    }
}