using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CommonCoreScripts.InventorySystem
{
    /// <summary>
    /// An item defines an object which can be placed in the inventory, as well as 
    /// it's behaviour in the inventory. You may extend this class further to also
    /// allow for additional behaviour not intended for use in the inventory system.
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "CommonCore/Item", order = 1)]
    public class Item : SerializedScriptableObject
    {
        #region Serialized Fields

        [OdinSerialize] public Dictionary<string, string> ItemParameters { get; } = new();

        /// <summary>
        /// The name of this item 
        /// </summary>
        [Tooltip("The name of this item")]
        [OdinSerialize] string _itemName;

        /// <summary>
        /// The number of this item that can fit into one stack
        /// </summary>
        [Tooltip("The number of this item that can fit into one stack")]
        [BoxGroup("Basic Info")]
        [OdinSerialize] int _stackSize;
        
        /// <summary>
        /// If consumeable, the number of items in the inventory should decrement when it is used 
        /// </summary>
        [Tooltip("If consumeable, the number of items in the inventory should decrement when it is used")]
        [BoxGroup("Basic Info")]
        [OdinSerialize] bool _isConsumeable;

        /// <summary>
        /// If can use, this item should be useable from the inventory
        /// </summary>
        [Tooltip("If can use, this item should be useable from the inventory")]
        [BoxGroup("Basic Info")]
        [OdinSerialize] bool _canUse;

        /// <summary>
        /// If can use, this item should have the option to be helkd when in the inventory
        /// </summary>
        [Tooltip("If can use, this item should have the option to be helkd when in the inventory")]
        [BoxGroup("Basic Info")]
        [OdinSerialize] bool _canHold;

        /// <summary>
        /// If can drop, this item can be dropped from the inventory
        /// </summary>
        [Tooltip("If can drop, this item can be dropped from the inventory")]
        [BoxGroup("Basic Info")]
        [OdinSerialize] bool _canDrop;

        [HorizontalGroup("Visuals")]

        /// <summary>
        /// The physical model spawned in XR versions of the inventory (usually a grabbable)
        /// </summary>
        [Tooltip("The physical model spawned in XR versions of the inventory (usually a grabbable)")]
        [BoxGroup("Visuals/Physical Model"), PreviewField(100, ObjectFieldAlignment.Left), HideLabel]
        [OdinSerialize] GameObject _physicalModel;

        /// <summary>
        /// The sprite tpo be used in the inventory UI for sprite-based systems
        /// </summary>
        [Tooltip("The sprite tpo be used in the inventory UI for sprite-based systems")]
        [BoxGroup("Visuals/Menu Sprite"), PreviewField(100, ObjectFieldAlignment.Left), HideLabel]
        [OdinSerialize] Sprite _menuSprite;

        [BoxGroup("Cursor")]

        /// <summary>
        /// The cursor texture to be used when dragging the item in a mouse-driven inventory UI
        /// </summary>
        [Tooltip("The cursor texture to be used when dragging the item in a mouse-driven inventory UI")]
        [HorizontalGroup("Cursor/CursorH", 50), PreviewField(50, ObjectFieldAlignment.Left), HideLabel]
        [OdinSerialize] Texture2D _cursor;

        /// <summary>
        /// The point within the cursor texture (between [0,0] - [1,1]) where a click event occurs
        /// </summary>
        [Tooltip("The point within the cursor texture (between [0,0] - [1,1]) where a click event occurs")]
        [HorizontalGroup("Cursor/CursorH")]
        [OdinSerialize] Vector2 _cursorHotspot;

        #endregion

        #region Public Accessors

        /// <summary>
        /// The name of this item 
        /// </summary>
        public string ItemName => _itemName;

        /// <summary>
        /// The number of this item that can fit into one stack
        /// </summary>
        public int StackSize => _stackSize;

        /// <summary>
        /// If consumeable, the number of items in the inventory should decrement when it is used 
        /// </summary>
        public bool IsConsumable => _isConsumeable;

        /// <summary>
        /// If can use, this item should be useable from the inventory
        /// </summary>
        public bool CanUse => _canUse;

        /// <summary>
        /// If can use, this item should have the option to be helkd when in the inventory
        /// </summary>
        public bool CanHold => _canHold;

        /// <summary>
        /// If can drop, this item can be dropped from the inventory
        /// </summary>
        public bool CanDrop => _canDrop;

        /// <summary>
        /// The physical model spawned in XR versions of the inventory (usually a grabbable)
        /// </summary>
        public GameObject PhysicalModel => _physicalModel;

        /// <summary>
        /// The sprite tpo be used in the inventory UI for sprite-based systems
        /// </summary>
        public Sprite MenuSprite => _menuSprite;

        /// <summary>
        /// The cursor texture to be used when dragging the item in a mouse-driven inventory UI
        /// </summary>
        public Texture2D Cursor => _cursor;

        /// <summary>
        /// The point within the cursor texture (between [0,0] - [1,1]) where a click event occurs
        /// </summary>
        public Vector2 CursorHotspot => _cursorHotspot;

        public Action OnUse { get; set; }

        public Action OnHold { get; set; }

        public Action OnCombine { get; set; }

        public Action OnDrop { get; set; }

        #endregion

        #region Public Methods

        public bool CanCombineWith(Item other)
        {
            return ItemCombinationManager.CanCombineItems(this, other);
        }

        public Item ResultOfCombineWith(Item other)
        {
            return ItemCombinationManager.ResultOfCombination(this, other);
        }

        public override bool Equals(object other)
        {
            if (other is Item item)
            {
                return ItemName == item.ItemName 
                       && ItemParameters.Keys == item.ItemParameters.Keys
                       && ItemParameters.Values == item.ItemParameters.Values;
            }

            return false;
        }

        #endregion
    }
}