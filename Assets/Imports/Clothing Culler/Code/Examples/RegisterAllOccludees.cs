using UnityEngine;

namespace Salvage.ClothingCuller.Components
{
    [RequireComponent(typeof(ClothingCuller))]
    public class RegisterAllOccludees : MonoBehaviour
    {
        #region SerializeFields

        [field: SerializeField]
        private ClothingCuller clothingCuller;

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            foreach (Occludee occludee in GetComponentsInChildren<Occludee>())
            {
                clothingCuller.Register(occludee);
            }
        }

#if UNITY_EDITOR

        private void Reset()
        {
            clothingCuller = GetComponent<ClothingCuller>();
        }

#endif

        #endregion

        #region Private Methods



        #endregion

        #region Public Methods



        #endregion
    }
}