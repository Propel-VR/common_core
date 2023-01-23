using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;


namespace CommonCoreScripts.InventorySystem
{

    [CreateAssetMenu(fileName = "Item", menuName = "CommonCore/ItemCombination", order = 2)]
    public class ItemCombination : SerializedScriptableObject
    {
        [OdinSerialize]
        Item _item1;

        [OdinSerialize]
        Item _item2;

        [OdinSerialize]
        Item _result;

        public Item Item1 => _item1;

        public Item Item2 => _item2;

        public Item Result => _result;
    }

}
