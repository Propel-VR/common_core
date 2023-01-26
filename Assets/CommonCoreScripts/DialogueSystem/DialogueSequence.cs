using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace CommonCoreScripts.DialogueSystem
{
    public class DialogueSequence : SerializedMonoBehaviour
    {
        public enum DialogueSequenceStatus
        {
            Idle,
            Playing,
            Finished
        }

        public struct NPCVariables
        {
            public AudioSource audioSource;
            public Animator animator;
        }

        public DialogueSequenceStatus Status { get; private set; } = DialogueSequenceStatus.Idle;

        public DialogueChunk currentChunk;
        public DialogueUI currentUI;
        public AudioSource audioSource;

        public bool usingContinueButton;
        
        public UnityEvent OnSequenceStart;
        public UnityEvent OnSequenceEnd;
        
        public Dictionary<DialogueChunk, UnityEvent> OnChunkStart;

        [Button("DEBUG: Start Dialogue")]
        public void StartSequence()
        {
            if (Status != DialogueSequenceStatus.Idle)
                return;

            Status = DialogueSequenceStatus.Playing;
            currentUI.gameObject.SetActive(true);
            currentUI.currentSequence = this;
            PlayDialogue();
            
            OnSequenceStart?.Invoke();
        }

        private void PlayDialogue()
        {
            // if there's no current chunk, or no current UI, end the dialogue.
            if (currentChunk == null || currentUI == null) return;

            currentUI.ShowDialogueText(currentChunk);
            currentUI.ShowSpeakerName(currentChunk);
            currentUI.ShowSpeakerPortrait(currentChunk);

            // set the audio source to the current chunk's audio clip and play it.
            var npc = DialogueHandler.Instance.GetNPC(currentChunk.speaker);
            
            if (npc != null)
            {
                audioSource = npc.audioSource;
                audioSource.clip = currentChunk.GetCurrentLanguageAudio();
                audioSource.Play();
            }

            // if the current chunk has more than one answer, or we are using confirmation continue buttons, show the option buttons.
            // otherwise, continue to the next chunk automatically when the audio finishes.
            #if UNITY_STANDALONE
                StartCoroutine(WaitForAudioToFinishAndAutoContinue(currentChunk.delay));
            #else
                currentUI.ShowDialogueOptions(currentChunk);
            #endif
        }

        public IEnumerator WaitForAudioToFinishAndAutoContinue (float delay = 0.1f)
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            yield return new WaitForSeconds(delay); // provides a very small buffer to prevent the audio from cutting off.
            if (currentChunk.nextChunks.Count > 1 || usingContinueButton) currentUI.ShowDialogueOptions(currentChunk);
            else MakeSelection(currentChunk.nextChunks.FirstOrDefault());
        }

        public void MakeSelection(DialogueChunk chunk)
        {
            if (chunk == null)
            {
                EndSequence();
                return;
            }
            currentChunk = chunk;
            if (OnChunkStart.ContainsKey(currentChunk)) OnChunkStart[currentChunk]?.Invoke();
            PlayDialogue();
        }

        public void MakeSelection(int chunkIndex)
        {
            if (chunkIndex > currentChunk.nextChunks.Count - 1 || chunkIndex < 0) return;
            MakeSelection(currentChunk.nextChunks[chunkIndex]);
            
            currentUI.HideDialogueOptions();
        }

        [Button("DEBUG: End Dialogue")]
        private void EndSequence()
        {
            Status = DialogueSequenceStatus.Finished;
            currentUI.HideDialogueText();
            currentUI.HideDialogueOptions();
            currentUI.HideSpeakerName();
            currentUI.HideSpeakerPortrait();
            currentUI.currentSequence = null;
            currentUI.gameObject.SetActive(false);
            
            OnSequenceEnd?.Invoke();
        }

        [Button("DEBUG: TOGGLE LANGUAGE")]
        private void ToggleLanguage()
        {
            if (PlayerPrefs.GetString("Language") == "EN")
                PlayerPrefs.SetString("Language", "FR");
            else 
                PlayerPrefs.SetString("Language", "EN");
        }
    }
}