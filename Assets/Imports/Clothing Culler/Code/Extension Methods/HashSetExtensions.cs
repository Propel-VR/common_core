using System.Collections.Generic;

namespace Salvage.ClothingCuller.ExtensionMethods
{
    public static class HashSetExtensions
    {

        #region Constructor



        #endregion

        #region Private Methods



        #endregion

        #region Public Methods

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                hashSet.Add(item);
            }
        }

        #endregion

    }
}
