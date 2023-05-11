using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCoreScripts.SceneManagement;
using System;

namespace CommonCoreScripts
{

    /// <summary>
    /// This is the player class, which should be placed on all/the player/s. 
    /// Use this class to get or reference to a player or teleport them.
    /// </summary>
    public abstract class Player : MonoBehaviour
    {
        #region Static Methods/Variables

        static List<Player> s_players = new();

        /// <summary>
        /// Get the current player, or if there are multiple players, get the one at th egiven index.
        /// </summary>
        public static Player Get(int index = 0)
        {
            if (index < s_players.Count)
                return s_players[index];
            else
                return null;
        }

        public static T Get<T>(int index = 0) where T : Player
        {
            return Get(index) as T;
        }

        #endregion

        #region Private/Protected Fields

        protected Camera _camera;

        #endregion

        #region MonoBehaviour Methods

        protected virtual void Awake()
        {
            s_players.Add(this);
            _camera = GetComponentInChildren<Camera>();
        }

        private void OnDestroy()
        {
            s_players.Remove(this);
        }

        #endregion

        #region Public Accessers

        /// <summary>
        /// The player camera.
        /// </summary>
        public Camera Camera => _camera;

        /// <summary>
        /// Event called on teleport
        /// </summary>
        public Action OnTeleport { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Teleport the player to a new position and rotation, specified by the given transform.
        /// </summary>
        public virtual void Teleport(Transform target)
        {
            Debug.Log("[Player]: Teleporting player");
            transform.SetPositionAndRotation(target.position, target.rotation);

            OnTeleport?.Invoke();
        }

        /// <summary>
        /// Teleport the player to a new position and rotation, specified by the given transform. 
        /// Will fade the screen to black before teleporting, then fade back afterwards.
        /// </summary>
        public virtual Coroutine FadeAndTeleport(Transform target)
        {
            return StartCoroutine(FadeAndTeleportRoutine(target));
        }

        /// <summary>
        /// Teleport the player to a new position and rotation, specified by the given transform.
        /// Will do a spherecast toward the ground first to ensure the player ends up firmly on the
        /// floor.
        /// </summary>
        public virtual void TeleportToFloor(Transform target)
        {
            if (Physics.Raycast(target.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hitInfo))
            {
                target.position = hitInfo.point;
            }

            Teleport(target);
        }

        /// <summary>
        /// Teleport the player to a new position and rotation, specified by the given transform.
        /// Will do a spherecast toward the ground first to ensure the player ends up firmly on the
        /// floor. Will also fade the screen to black before teleporting, then fade back afterwards.
        /// </summary>
        public virtual Coroutine FadeAndTeleportToFloor(Transform target)
        {
            return StartCoroutine(FadeAndTeleportToFloorRoutine(target));
        }

        #endregion

        #region Private Methods

        IEnumerator FadeAndTeleportRoutine(Transform target)
        {
            if (ScreenFader.Instance == null)
                yield break;

            yield return ScreenFader.Instance.FadeOut();

            Teleport(target);

            yield return ScreenFader.Instance.FadeIn();
        }

        IEnumerator FadeAndTeleportToFloorRoutine(Transform target)
        {
            if (ScreenFader.Instance == null)
                yield break;

            yield return ScreenFader.Instance.FadeOut();

            TeleportToFloor(target);

            yield return ScreenFader.Instance.FadeIn();
        }

        #endregion
    }

}
