using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LM.Management;


namespace Dev.Josh
{

    [RequireComponent(typeof(TMP_Dropdown))]
    public class SelectPresetDropdown : MonoBehaviour
    {
        [SerializeField]
        List<Preset> _presets;

        TMP_Dropdown _dropdown;

        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.ClearOptions();
            _dropdown.AddOptions(_presets.ConvertAll(preset => preset.name));
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }

        void OnValueChanged(int index)
        {
            Configurator.Instance.LoadPreset(_presets[index]);
        }
    }

}
