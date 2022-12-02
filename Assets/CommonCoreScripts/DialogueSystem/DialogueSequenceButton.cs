using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCoreScripts.DialogueSystem
{
    /// <summary>
    /// Button for the Dialogue UI
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DialogueSequenceButton : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text text;
        public DialogueUI currentUI; 
        public void ShowButton()
        {
            animator.SetTrigger("Show");
        }
        
        public void HideButton()
        {
            animator.SetTrigger("Hide");
        }

        public void SetText(string buttonText)
        {
            if (text != null)
                text.text = buttonText;
        }

        public void MakeSelectionForCurrentSequence(int option)
        {
            currentUI.currentSequence.MakeSelection(option - 1);
        }
    }
}