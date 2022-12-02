using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RiskObject))]
public class RiskObjectEditor : Editor
{
    private Editor assetEditor;
    private bool assetFoldout;

    SerializedProperty spAsset;
    SerializedProperty onInteractEventP;
    SerializedProperty onAssesEventP;
    SerializedProperty contextPointP;

    void OnEnable ()
    {
        spAsset = serializedObject.FindProperty("asset");
        onInteractEventP = serializedObject.FindProperty("onInteract");
        onAssesEventP = serializedObject.FindProperty("onAssesRisk");
        contextPointP = serializedObject.FindProperty("contextPoint");
    }

    public override void OnInspectorGUI ()
    {
        RiskObject riskObject = (RiskObject)target;

        EditorGUILayout.PropertyField(spAsset);
        bool isObjectAssigned = spAsset.objectReferenceValue != null;
        EditorGUI.BeginDisabledGroup(isObjectAssigned);
        if(GUILayout.Button("Create asset"))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                title: "Create new danger object", 
                defaultName: "DangerAsset", 
                extension: "asset", 
                message: "Please enter a file name to save the asset to", 
                path: "Assets/_Project/ScriptableObjects/Hazards/");
            if(!string.IsNullOrEmpty(path))
            {
                RiskAsset asset = ScriptableObject.CreateInstance<RiskAsset>();

                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                spAsset.objectReferenceValue = asset;
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();

        if (isObjectAssigned)
        {
            assetFoldout = EditorGUILayout.Foldout(assetFoldout, "Parameters");
            if(assetFoldout)
            {
                CreateCachedEditor(spAsset.objectReferenceValue, null, ref assetEditor);
                assetEditor.OnInspectorGUI();
            }
        }

        EditorGUILayout.PropertyField(onInteractEventP);
        EditorGUILayout.PropertyField(onAssesEventP);

        bool contextPointAssigned = contextPointP.objectReferenceValue != null;
        EditorGUI.BeginDisabledGroup(contextPointAssigned);
        if (GUILayout.Button("Create context point"))
        {
            GameObject context = new GameObject("Context Point");
            context.transform.parent = riskObject.transform;
            context.transform.position = riskObject.transform.position;
            contextPointP.objectReferenceValue = context;
            Selection.activeGameObject = context;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(contextPointP);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif