using System;
using UnityEngine;

namespace Salvage.ClothingCuller.Serialization
{
    [Serializable]
    public class SerializableBoolArray
    {
        public bool[] Value
        {
            get { return serializedValue; }
            set { serializedValue = value; }
        }

        #region SerializeFields

        [SerializeField]
        private bool[] serializedValue;

        #endregion

        #region Constructor



        #endregion

        #region Private Methods



        #endregion

        #region Public Methods



        #endregion

    }
}
