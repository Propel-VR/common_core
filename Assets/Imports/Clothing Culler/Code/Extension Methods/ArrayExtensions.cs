namespace Salvage.ClothingCuller.ExtensionMethods
{
    public static class ArrayExtensions
    {

        #region Constructor



        #endregion

        #region Private Methods



        #endregion

        #region Public Methods

        public static T ElementAtOrLast<T>(this T[] array, int index)
        {
            if (index < array.Length)
            {
                return array[index];
            }

            return array[array.Length - 1];
        }

        #endregion

    }
}
