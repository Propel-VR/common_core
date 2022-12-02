using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CommonCoreScripts.InventorySystem
{
    public class ItemVisualHandler : SerializedMonoBehaviour
    {
        public static ItemVisualHandler Instance { get; private set; }
        public Dictionary<Item, ItemVisuals> ItemVisuals = new ();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
    }
    
    public class ItemVisuals
    {
        public GameObject physicalModel;
        public Sprite menuSprite;
        public string displayName;
        
        public ItemVisuals(GameObject physicalModel, Sprite menuSprite, string displayName)
        {
            this.physicalModel = physicalModel;
            this.menuSprite = menuSprite;
            this.displayName = displayName;
        }
    }
}