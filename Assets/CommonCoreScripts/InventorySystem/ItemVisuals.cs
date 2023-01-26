using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CommonCoreScripts.InventorySystem
{
    [CreateAssetMenu(fileName = "ItemVisuals", menuName = "CommonCore/ItemVisuals", order = 0)]
    public class ItemVisuals : SerializedScriptableObject
    {
        [OdinSerialize] private GameObject _physicalModel;
        public GameObject PhysicalModel => _physicalModel;
        
        [OdinSerialize] private Sprite _menuSprite;
        public Sprite MenuSprite => _menuSprite;

        [OdinSerialize] private string _displayName;
        public string DisplayName => _displayName;

        public ItemVisuals(GameObject physicalModel, Sprite menuSprite, string displayName)
        {
            this._physicalModel = physicalModel;
            this._menuSprite = menuSprite;
            this._displayName = displayName;
        }
    }
}