using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using Unity.Tutorials.Core.Editor;

namespace Autohand.Demo{
    public class TextPlacePointEvent : MonoBehaviour{
        public TextChanger changer;
        public PlacePoint point;
        public float fadeTime = 5;
        [TextArea]
        public string placeMessage;
        [TextArea]
        public string highlightMessage;

        private void Start() {
            if(point == null && GetComponent<PlacePoint>() != null)
                point = GetComponent<PlacePoint>();
            point.OnPlaceEvent += OnGrab;
            point.OnHighlightEvent += OnHighlight;
        }
        
        void OnGrab(PlacePoint hand, Grabbable grab) {
            if(changer && placeMessage.IsNotNullOrEmpty())
                changer.UpdateText(placeMessage);
        }

        void OnHighlight(PlacePoint hand, Grabbable grab) {
            if (changer && highlightMessage.IsNotNullOrEmpty())
                changer.UpdateText(highlightMessage);
        }
        

    }
}
