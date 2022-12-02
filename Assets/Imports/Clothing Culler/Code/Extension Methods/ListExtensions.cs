using System.Collections.Generic;

namespace Salvage.ClothingCuller.ExtensionMethods
{
    public static class ListExtensions
    {

        #region Constructor



        #endregion

        #region Private Methods



        #endregion

        #region Public Methods

        public static bool RemoveUnityNullValues<T>(this List<T> list) where T : UnityEngine.Object
        {
            bool hasRemovedAnyItems = false;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                {
                    hasRemovedAnyItems = true;

                    list.RemoveAt(i);
                }
            }

            return hasRemovedAnyItems;
        }

        #endregion

    }
}
