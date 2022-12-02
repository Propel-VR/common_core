using UnityEngine;

namespace CommonCoreScripts.DialogueSystem
{
    /// <summary>
    /// Abstract class representing dialogue UI for the dialogue system.
    /// </summary>
    public abstract class DialogueUI : MonoBehaviour
    {
        public Animator animator;
        public DialogueSequence currentSequence;
        
        /// <summary>
        /// Shows the text on the UI.
        /// </summary>
        /// <param name="chunk">The current dialogue chunk.</param>
        public abstract void ShowDialogueText(DialogueChunk chunk);
        
        /// <summary>
        /// Hides the text on the UI.
        /// </summary>
        public abstract void HideDialogueText();
        
        /// <summary>
        /// Set values for and show following dialogue options.
        /// </summary>
        /// <param name="chunk">Current DialogueChunk holding the options to be shown.</param>
        public abstract void ShowDialogueOptions(DialogueChunk chunk);
        
        /// <summary>
        /// Hides following dialogue options.
        /// </summary>
        public abstract void HideDialogueOptions();
        
        
        public abstract void ShowSpeakerName(DialogueChunk chunk);
        
        
        public abstract void HideSpeakerName();
        
        
        public abstract void ShowSpeakerPortrait(DialogueChunk chunk);
        
        
        public abstract void HideSpeakerPortrait();
    }
}