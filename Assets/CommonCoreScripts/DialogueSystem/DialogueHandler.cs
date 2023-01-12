using System.Collections.Generic;
using System.Linq;
using CommonCoreScripts.DialogueSystem.Custom_Assets;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CommonCoreScripts.DialogueSystem
{
    public class DialogueHandler : SerializedMonoBehaviour
    {
        public static DialogueHandler Instance { get; private set; }
        [OdinSerialize] private HashSet<NPCVariable> npcs = new();
        
        // awake singleton
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public NPCVariable GetNPC(string npcName)
        {
            return npcs.FirstOrDefault(npc => npc.name == npcName);
        }
    }
}