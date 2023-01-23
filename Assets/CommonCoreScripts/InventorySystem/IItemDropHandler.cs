using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommonCoreScripts.InventorySystem
{
    interface IItemDropHandler
    {
        public bool OnItemDrop(Item item);
    }

}
