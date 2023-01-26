using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace CommonCoreScripts.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "CommonCore/Item", order = 1)]
    public class Item : SerializedScriptableObject
    {
        [OdinSerialize] public string ItemName { get; set; }
        [OdinSerialize] public Dictionary<string, string> ItemParameters { get; } = new();
        [OdinSerialize] public int StackSize { get; set; }
        [OdinSerialize] public bool IsConsumable { get; set; }

        [OdinSerialize] public bool HasUse { get; private set; }
        [OdinSerialize] public bool HasHold { get; private set; }
        [OdinSerialize] public bool HasCombine { get; private set; }
        [OdinSerialize] public bool HasDrop { get; private set; }

        [OdinSerialize] private Dictionary<Item, Item> _combineResults = new();
        public Dictionary<Item, Item> CombineResults => _combineResults;

        [Space]
        
        [OdinSerialize] [NonSerialized] private UnityEvent _onUse = new();
        [OdinSerialize] [NonSerialized] private UnityEvent _onHold = new();
        [OdinSerialize] [NonSerialized] private UnityEvent _onCombine = new();
        
        public UnityEvent OnUse => _onUse;
        public UnityEvent OnHold => _onHold;
        public UnityEvent OnCombine => _onCombine;

        public bool CanCombineWith(Item item)
        {
            return CombineResults.ContainsKey(item);
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
    }
}