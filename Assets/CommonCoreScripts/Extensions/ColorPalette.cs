using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CommonCoreScripts.Extensions
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "Color Palette", order = 1)]
    public class ColorPalette : SerializedScriptableObject
    {
        [SerializeField] private Color _primary = new (0, 0, 0, 255);
        [SerializeField] private Color _secondary = new (0, 0, 0, 255);
        [SerializeField] private Color _tertiary = new (0, 0, 0, 255);
        [SerializeField] private Color _text = new (0, 0, 0, 255);
        [SerializeField] private Color _grey = new (0, 0, 0, 255);
        [SerializeField] private Color _white = new (0, 0, 0, 255);
        [SerializeField] private Color _warning = new (0, 0, 0, 255);
        [SerializeField] private Color _error = new (0, 0, 0, 255);
        
        public Color Primary => _primary;
        public Color Secondary => _secondary;
        public Color Tertiary => _tertiary;
        public Color Text => _text;
        public Color Grey => _grey;
        public Color White => _white;
        public Color Warning => _warning;
        public Color Error => _error;
        
        public enum ColorPaletteType
        {
            Primary,
            Secondary,
            Tertiary,
            Text,
            Grey,
            White,
            Warning,
            Error
        }

        [Space] 
        [OdinSerialize] [TextArea(8, 12)] private readonly string _paletteInput = "";
        
        [Button("Apply Palette Input")]
        public void ApplyPaletteInput()
        {
            var split = _paletteInput.Split('\n');
            
            if (split.Length != 8)
            {
                Debug.LogError("Invalid palette input");
                return;
            }

            _primary = ParseColor(split[0]);
            _secondary = ParseColor(split[1]);
            _tertiary = ParseColor(split[2]);
            _text = ParseColor(split[3]);
            _grey = ParseColor(split[4]);
            _white = ParseColor(split[5]);
            _warning = ParseColor(split[6]);
            _error = ParseColor(split[7]);
        }

        /// <summary>
        /// Helper method, parses a color from its hex string representation.
        /// </summary>
        /// <param name="colorHex">The hex string representation of the input number.</param>
        /// <returns>The Color object containing the color represented by the input.</returns>
        private Color ParseColor(string colorHex)
        {
            var colorStripped = colorHex.Replace("#", "").Replace("0x", "");
            var r = byte.Parse(colorStripped.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(colorStripped.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(colorStripped.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            var a = (colorStripped.Length == 8) ? byte.Parse(colorStripped.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) : (byte)255;
            return new Color32(r, g, b, a);
        }
    }
}