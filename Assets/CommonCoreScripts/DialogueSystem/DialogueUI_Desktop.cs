using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCoreScripts.DialogueSystem
{
    /// <summary>
    /// Implements a dialogue UI that uses Unity UI for the Desktop versions.
    /// </summary>
    public class DialogueUI_Desktop : DialogueUI
    {
        public TMP_Text speakerText;
        public TMP_Text nameText;
        public List<DialogueSequenceButton> buttons;
        public Image portait;
        
        public override void ShowDialogueText(DialogueChunk chunk)
        {
            speakerText.text = chunk.GetCurrentLanguageText();
            nameText.text = chunk.speaker;
            portait.sprite = chunk.speakerSprite;
            //animator.SetTrigger("ShowDialogue");
        }
        
        public override void HideDialogueText()
        {
            //animator.SetTrigger("HideDialogue");
        }
        
        public override void ShowDialogueOptions(DialogueChunk chunk)
        {
            for (int i = 0; i < chunk.nextChunks.Count; i++)
            {
                buttons[i].SetText(chunk.GetCurrentLanguageAnswer(i));
                buttons[i].ShowButton();
            }
        }

        public override void HideDialogueOptions()
        {
            foreach (var button in buttons) button.HideButton();
        }

        public override void ShowSpeakerName(DialogueChunk chunk)
        {
            nameText.text = chunk.speaker;
            //animator.SetTrigger("ShowName");
        }

        public override void HideSpeakerName()
        {
            //animator.SetTrigger("HideName");
        }

        public override void ShowSpeakerPortrait(DialogueChunk chunk)
        {
            portait.sprite = chunk.speakerSprite;
            //animator.SetTrigger("ShowPortrait");
        }

        public override void HideSpeakerPortrait()
        {
            //animator.SetTrigger("HidePortrait");
        }
    }
}