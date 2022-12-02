using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace CommonCoreScripts.InventorySystem
{
    public class Item : SerializedScriptableObject
    {
        public string itemName;
        public Dictionary<string, string> itemParameters = new ();
        public int stackSize = 64;

        public bool hasUse;
        public bool hasHold;
        public bool hasCombine;

        [ShowIf("hasUse")] public UnityEvent OnUse;
        [ShowIf("hasHold")] public UnityEvent OnHold;
        [ShowIf("hasCombine")] public UnityEvent OnCombine;
    }
}