using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CommonCoreScripts.InventorySystem
{
    public class ItemVisualHandler : SerializedMonoBehaviour
    {
        public static ItemVisualHandler Instance { get; set; }
        [OdinSerialize] private Dictionary<Item, ItemVisuals> _itemVisuals = new();
        public Dictionary<Item, ItemVisuals> ItemVisuals => _itemVisuals;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        
        public ItemVisuals GetItemVisuals(Item item)
        {
            return _itemVisuals.Any(x => x.Key.Equals(item)) ? _itemVisuals.First(x => x.Key.Equals(item)).Value : null;
        }
    }
}