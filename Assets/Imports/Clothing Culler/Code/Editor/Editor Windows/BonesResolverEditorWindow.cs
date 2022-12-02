using System;
using System.Linq;
using Salvage.ClothingCuller.Configuration;
using UnityEditor;
using UnityEngine;

namespace Salvage.ClothingCuller.Editor.EditorWindows
{
    public class BonesResolverEditorWindow : EditorWindow
    {
        private Transform sourceRigRoot;
        private SkinnedMeshRenderer targetSkinnedMeshRenderer;
        private Vector2 scrollViewPosition;
        [NonSerialized] private SkinnedMeshData skinnedMeshData;
        [NonSerialized] private Transform newRootBone;
        [NonSerialized] private Transform[] newBones;

        #region EditorWindow Methods

        [MenuItem("Window/Clothing Culler/Resolve Skinned Mesh Bones")]
        private static void show()
        {
            GetWindow<BonesResolverEditorWindow>("Resolve Bones");
        }

        private void OnEnable()
        {
            onSourceOrTargetChanged();
        }

        #endregion

        #region OnGUI

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            drawSourceFields();
            GUILayout.Space(10f);
            drawOutputFields();
            drawResolveButton();

            EditorGUILayout.EndVertical();
        }

        private void drawSourceFields()
        {
            EditorGUILayout.LabelField("Bones transform source", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            sourceRigRoot = EditorGUILayout.ObjectField("Rig Root", sourceRigRoot, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck())
            {
                onSourceOrTargetChanged();
            }
        }

        private void onSourceOrTargetChanged()
        {
            if (targetSkinnedMeshRenderer == null || sourceRigRoot == null)
            {
                skinnedMeshData = null;
                newRootBone = null;
                newBones = null;
                return;
            }

            skinnedMeshData = SkinnedMeshData.Create(targetSkinnedMeshRenderer);
            if (skinnedMeshData == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(skinnedMeshData.RootBoneName))
            {
                newRootBone = skinnedMeshData.FindBoneInChildren(sourceRigRoot, skinnedMeshData.RootBoneName);
            }

            newBones = new Transform[skinnedMeshData.BoneNames.Length];

            for (int i = 0; i < skinnedMeshData.BoneNames.Length; i++)
            {
                string boneName = skinnedMeshData.BoneNames[i];

                if (!string.IsNullOrEmpty(boneName))
                {
                    newBones[i] = skinnedMeshData.FindBoneInChildren(sourceRigRoot, boneName);
                }
            }
        }

        private void drawOutputFields()
        {
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

            drawValidationFields();

            EditorGUI.BeginChangeCheck();
            targetSkinnedMeshRenderer = EditorGUILayout.ObjectField("Target", targetSkinnedMeshRenderer, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            if (EditorGUI.EndChangeCheck())
            {
                onSourceOrTargetChanged();
            }

            if (skinnedMeshData == null)
            {
                return;
            }

            drawBoneField("Root Bone", skinnedMeshData.RootBoneName, ref newRootBone);

            string bonesCount = newBones != null ? $"{newBones.Where(x => x != null).Count()} / {newBones.Length}" : string.Empty;
            EditorGUILayout.LabelField("Assigned bones:", bonesCount);

            scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition, EditorStyles.helpBox, GUILayout.Height(150f));

            for (int i = 0; i < newBones.Length; i++)
            {
                drawBoneField(string.Empty, skinnedMeshData.BoneNames[i], ref newBones[i]);
            }

            EditorGUILayout.EndScrollView();
        }

        private void drawBoneField(string label, string originalBoneName, ref Transform newBone)
        {
            var richTextLabelStyle = new GUIStyle(EditorStyles.label) { richText = true };
            originalBoneName = string.IsNullOrEmpty(originalBoneName) ? "null" : originalBoneName;

            EditorGUILayout.BeginHorizontal();

            newBone = EditorGUILayout.ObjectField(label, newBone, typeof(Transform), true) as Transform;
            EditorGUILayout.LabelField(newBone == null ? $"Original bone name: <b>{originalBoneName}</b>" : string.Empty, richTextLabelStyle);

            EditorGUILayout.EndHorizontal();
        }

        private void drawValidationFields()
        {
            if (sourceRigRoot == null || targetSkinnedMeshRenderer == null)
            {
                return;
            }

            if (targetSkinnedMeshRenderer.sharedMesh == null)
            {
                EditorGUILayout.HelpBox($"SkinnedMeshRenderer '{targetSkinnedMeshRenderer.name}' does not have a sharedMesh assigned.", MessageType.Error);
                return;
            }

            if (skinnedMeshData == null)
            {
                EditorGUILayout.HelpBox($"Original Mesh '{AssetDatabase.GetAssetPath(targetSkinnedMeshRenderer.sharedMesh)}' does not have a SkinnedMeshRenderer component.", MessageType.Error);
            }
        }

        private void drawResolveButton()
        {
            EditorGUI.BeginDisabledGroup(skinnedMeshData == null);

            if (GUILayout.Button("Resolve"))
            {
                onResolveButtonClicked();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void onResolveButtonClicked()
        {
            if (newRootBone == null || newBones.Any(x => x == null))
            {
                if (!EditorUtility.DisplayDialog("Resolving is incomplete", "Not all bones have been resolved, are you sure you want to continue?", "Yes", "Cancel"))
                {
                    return;
                }
            }

            targetSkinnedMeshRenderer.rootBone = newRootBone;
            targetSkinnedMeshRenderer.bones = newBones;

            targetSkinnedMeshRenderer = null;
            skinnedMeshData = null;
            newRootBone = null;
            newBones = null;
        }

        #endregion

        #region Private Methods



        #endregion

        #region Public Methods



        #endregion

    }
}

