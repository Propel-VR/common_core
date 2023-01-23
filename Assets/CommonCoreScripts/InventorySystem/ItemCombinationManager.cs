using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommonCoreScripts.InventorySystem
{

    public class ItemCombinationManager : MonoBehaviour
    {
        [SerializeField]
        List<ItemCombination> _itemCombinations;

        static Dictionary<Item, Dictionary<Item, Item>> s_lookUpTable = new ();

        private void Awake()
        {
            foreach (var combination in _itemCombinations)
            {
                AddItemPair(combination.Item1, combination.Item2, combination.Result);
                AddItemPair(combination.Item2, combination.Item1, combination.Result);
            }
        }

        public static bool CanCombineItems(Item item1, Item item2)
        {
            return s_lookUpTable.ContainsKey(item1) && s_lookUpTable[item1].ContainsKey(item2);
        }


        public static Item ResultOfCombination(Item item1, Item item2)
        {
            return s_lookUpTable[item1][item2];
        }

        #region Private Helper Methods

        void AddItemPair(Item item1, Item item2, Item result)
        {
            // ensure we have not already added this combination, and also check that if it was already added, that it at least has the same result
            if (CanCombineItems(item1, item2))
            {
                Debug.Assert(ResultOfCombination(item1, item2) == result, "[ItemCombinationManager]: Cannot register two item combinations with the same input items and a different result");
                return;
            }

            if (s_lookUpTable.ContainsKey(item1))
                s_lookUpTable[item1][item2] = result;
            else
            {
                s_lookUpTable[item1] = new Dictionary<Item, Item>();
                s_lookUpTable[item1][item2] = result;
            }
        }

        #endregion

    }

}
