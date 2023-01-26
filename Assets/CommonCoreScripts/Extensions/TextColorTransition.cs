using System;
using System.Collections;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

namespace CommonCoreScripts.Extensions
{
    public class TextColorTransition : MonoBehaviour
    {
        [SerializeField] private ColorPalette colorPalette;
        [SerializeField] private TMP_Text text;
        [SerializeField] private float transitionDurationSeconds = 1;

        public void TransitionToColor(string colorName)
        {
            Enum.TryParse(colorName, out ColorPalette.ColorPaletteType paletteType);
            var color = paletteType switch
            {
                ColorPalette.ColorPaletteType.Primary => colorPalette.Primary,
                ColorPalette.ColorPaletteType.Secondary => colorPalette.Secondary,
                ColorPalette.ColorPaletteType.Tertiary => colorPalette.Tertiary,
                ColorPalette.ColorPaletteType.Text => colorPalette.Text,
                ColorPalette.ColorPaletteType.Grey => colorPalette.Grey,
                ColorPalette.ColorPaletteType.White => colorPalette.White,
                ColorPalette.ColorPaletteType.Warning => colorPalette.Warning,
                ColorPalette.ColorPaletteType.Error => colorPalette.Error,
                _ => Color.black
            };

            StartCoroutine(LerpColors(color));
        }
        
        private IEnumerator LerpColors(Color color)
        {
            var elapsedTime = 0f;
            var startColor = text.color;
            while (elapsedTime < transitionDurationSeconds)
            {
                text.color = Color.Lerp(startColor, color, elapsedTime / transitionDurationSeconds);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
