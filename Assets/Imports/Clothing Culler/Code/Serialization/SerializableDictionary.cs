using System.Collections.Generic;
using UnityEngine;

namespace Salvage.ClothingCuller.Serialization
{
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {

        #region SerializeFields

        [SerializeField]
        private List<TKey> keys;

        [SerializeField]
        private List<TValue> values;

        #endregion

        #region Constructor

        public SerializableDictionary()
        {
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        #endregion

        #region ISerializationCallbackReceiver Members

        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
            {
                Debug.LogError($"Unable to deserialize SerializableDictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - amount of serialized keys does not match the amount of serialized values");
                return;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        #endregion

        #region Private Methods



        #endregion

        #region Public Methods



        #endregion

    }
}
