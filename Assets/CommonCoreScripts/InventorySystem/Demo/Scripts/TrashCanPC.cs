using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommonCoreScripts.InventorySystem.Demo
{

    public class TrashCanPC : MonoBehaviour, IItemDropHandler
    {
        public bool OnItemDrop(Item item)
        {
            Debug.Log($"Threw {item.ItemName} into the trash.");

            // because we are no longer in the inventory, we must handle any use/hold/drop of items
            item.OnDrop?.Invoke();

            // return true to signify that the item drop has been handled
            return true;
        }
    }

}
