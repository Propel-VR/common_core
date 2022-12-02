using System.Collections.Generic;
using RogoDigital.Lipsync;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CommonCoreScripts.DialogueSystem
{
    [CreateAssetMenu(menuName = "Create Dialogue Chunk", fileName = "DialogueChunk", order = 0)]
    public class DialogueChunk : SerializedScriptableObject
    {
        public string textEN;
        public string textFR;
        
        [Header("Speaker Settings")]
        public string speaker;
        public Sprite speakerSprite;
        
        [Header("Audio Clips and LipSyncPro data")]
        public AudioClip audioEN;
        public AudioClip audioFR;
        public LipSyncData lipSyncData;
        
        [Header("Animation Settings")]
        public List<string> emotions;
        public List<AnimationClip> animations;
        
        [Header("Answers")]
        public List<DialogueChunk> nextChunks;
        public List<string> nextChunkAnswersEN;
        public List<string> nextChunkAnswersFR;
        
        [Header("Delay [ONLY FOR STANDALONE USE]")]
        public float delay;

        public string GetCurrentLanguageText()
        {
            return (PlayerPrefs.GetString("Language") == "EN") ? 
                textEN
                : textFR;
        }
        
        public AudioClip GetCurrentLanguageAudio()
        {
            return (PlayerPrefs.GetString("Language") == "EN") ? 
                audioEN
                : audioFR;
        }
        
        public string GetCurrentLanguageAnswer(int index)
        {
            return (PlayerPrefs.GetString("Language") == "EN") ? 
                nextChunkAnswersEN[index]
                : nextChunkAnswersFR[index];
        }
    }
}