using CommonCoreScripts.DialogueSystem;
using UnityEngine;

namespace CommonCoreScripts.Extensions
{
    /// <summary>
    /// Created by: Juan Hiedra Primera
    ///
    /// Helper method for the Dialogue system. Sets the answer dialogue chunk of its target chunk to the one
    /// specified by the replacement chunk.
    ///
    /// <see cref="DialogueChunk"/>
    /// </summary>
    public class DialogueSetter : MonoBehaviour
    {
        private DialogueChunk _targetChunk;
        private int _targetIndex;

        private DialogueChunk _replacementChunk;
        
        /// <summary>
        /// Sets the <c>_targetIndex</c>'th answer of the target chunk to the replacement chunk.
        /// </summary>
        public void Replace()
        {
            if (_replacementChunk == null)
                return;
            
            if (_replacementChunk.nextChunks.Count <= _targetIndex)
                return;
            
            _targetChunk.nextChunks[_targetIndex] = _replacementChunk;
        }

        /// <summary>
        /// Access method for <c>_replacementChunk</c>. Sets the value of <c>_replacementChunk</c> to the value of <c>replacementChunk</c>.
        /// </summary>
        /// <param name="replacementChunk">The value to set <c>_replacementChunk</c></param>
        public void SetReplacementChunk(DialogueChunk replacementChunk)
        {
            _replacementChunk = replacementChunk;
        }
    }
}