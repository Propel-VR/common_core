using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using Sirenix.OdinInspector;

namespace CommonCoreScripts.SceneManagement
{

	/// <summary>
	/// Holds a reference to a scene object (or rather, to its name, since holding an
	/// actual reference can cause load time issues).
	/// </summary>
	[Serializable]
    [HideReferenceObjectPicker]
    public class SceneRef : ISerializationCallbackReceiver
    {
		public SceneRef() { }

		public SceneRef(string name, string path)
		{
			_name = name;
			_path = path;
		}

		public SceneRef(Scene scene)
		{
			if (scene != null)
			{
				_name = scene.name;
				_path = scene.path;
			}
		}

		[SerializeField]
		[LabelText("Scene")]
		Object _sceneAssetObject = null;

        [SerializeField, HideInInspector]
        string _name = null;
        
        [SerializeField, HideInInspector]
        string _path = null;

		/// <summary>
		/// The scene name.
		/// </summary>
		public string name => _name;

		/// <summary>
		/// The path to the actual scene asset object.
		/// </summary>
		public string path => _path;

		/// <summary>
		/// True if this reference actually refers to a scene object, false otherwise.
		/// </summary>
        public bool IsSet() => !String.IsNullOrEmpty(_name);

        public override string ToString()
		{
			return path;
		}

		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			if (_sceneAssetObject)
            {
				string prevPath = _path;
                SetSceneAsset(_sceneAssetObject);

                if (!Application.isPlaying && prevPath != _path)
					UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
#endif
		}

		public void OnAfterDeserialize()
		{

		}

//        public void OnPreprocessBuild(BuildReport report)
//        {
//#if UNITY_EDITOR
//            if (_sceneAssetObject)
//                SetSceneAsset(_sceneAssetObject);
//
//            _sceneAssetObject = null;
//#endif
//        }

		void SetSceneAsset(Object sceneAsset)
        {
#if UNITY_EDITOR
            _name = sceneAsset.name;
            _path = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
#endif
		}
    }
}
