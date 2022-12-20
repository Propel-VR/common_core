using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommonCoreScripts.OutOfBoundsChecker
{

    public class ExampleOutOfBoundsChecker : OutOfBoundsChecker
    {
        [Header("Player")]
        [SerializeField]
        Transform _player;

        public override Transform GetPlayer()
        {
            return _player;
        }
    }

}
