using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace CommonCoreScripts.RandomSystem
{
    /// <summary>
    /// Created by: Juan Hiedra Primera
    ///
    /// General purpose script that calls a random event through a list of events and weights;
    /// </summary>
    public class EventRandomizer : MonoBehaviour
    {
        [OdinSerialize] private List<RandomEvent> _events = new();

        /// <summary>
        /// Calls a random event, based on the weights of the events given
        /// </summary>
        public void Randomize()
        {
            // if there are no events, do nothing
            if (_events.Count == 0)
                return;

            // choose a number between 0 and the sum of all weights; this is the "random" number
            var weight = Random.Range(0, _events.Sum(e => e.GetChance()));

            // find the event that corresponds to the random number, by adding up the weights until we reach
            // the random number
            var tmp = 0;
            foreach (var randomEvent in _events)
            {
                tmp += randomEvent.GetChance();
                if (weight >= tmp) continue;
                var ev = randomEvent.GetEvent();
                ev?.Invoke();
                break;
            }
        }
    }

    /// <summary>
    /// Contains the data for a random event: the event, and its event probability.
    /// </summary>
    public class RandomEvent
    {
        /// <summary>
        /// The event to be executed.
        /// </summary>
        [OdinSerialize] private UnityEvent _randomEvent = new();
        
        /// <summary>
        /// The chance of the event to happen. See <c>EventRandomizer</c> for more info.
        /// </summary>
        [OdinSerialize] private int _chance = 1;
        
        public UnityEvent GetEvent()
        {
            return _randomEvent;
        }
        
        public int GetChance()
        {
            return _chance;
        }
    }
}