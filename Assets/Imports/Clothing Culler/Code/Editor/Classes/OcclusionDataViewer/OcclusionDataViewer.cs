using System.Linq;
using UnityEngine;
using UnityEditor;
using Salvage.ClothingCuller.Editor.Configuration;
using Salvage.ClothingCuller.ExtensionMethods;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Salvage.ClothingCuller.Configuration;
using Salvage.ClothingCuller.EditorComponents;

namespace Salvage.ClothingCuller.Editor
{
    [InitializeOnLoad]
    public static class OcclusionDataViewer
    {
        private const string sceneName = "View Occlusion Data";
        private const int vertexHandleBaseId = 1000000;
        private static OcclusionDataViewerState occlusionDataViewerState;
        private static ViewableLOD[] occludeeLODs;
        private static ViewableLOD[] occluderLODs;
        private static string[] occludeeLODDropDownItems;
        private static int selectedLODIndex;
        private static ViewableLOD selectedOccludeeLOD;
        private static ViewableLOD selectedOccluderLOD;
        private static Rect controlsWindowRect;
        private static bool canHandleDrag;
        private static bool wasClickedTriangleOccluded;
        private static ViewableMesh clickedViewableMesh;

        #region Properties

        public static bool IsSceneOpen
        {
            get
            {
                return occlusionDataViewerState != null;
            }
        }

        public static OccludeeConfiguration Occludee
        {
            get
            {
                if (occlusionDataViewerState == null)
                {
                    return null;
                }

                return occlusionDataViewerState.Occludee;
            }
        }

        public static OccludeeConfiguration Occluder
        {
            get
            {
                if (occlusionDataViewerState == null)
                {
                    return null;
                }

                return occlusionDataViewerState.Occluder;
            }
        }

        #endregion

        #region Constructor

        static OcclusionDataViewer()
        {
            EditorApplication.wantsToQuit += onEditorApplicationWantsToQuit;
            EditorSceneManager.sceneClosing += onEditorSceneClosing;

            if (SceneManager.GetActiveScene().name != sceneName)
            {
                return;
            }

            occlusionDataViewerState = Object.FindObjectOfType<OcclusionDataViewerState>();
            if (occlusionDataViewerState == null)
            {
                return;
            }

            unload();
            load();
        }

        #endregion

        #region OnSceneGUI

        private static void onSceneGUI(SceneView sceneView)
        {
            if (occludeeLODs != null && occluderLODs != null)
            {
                drawWireFrames();

                if (occlusionDataViewerState.IsDebugVisible)
                {
                    drawVertexHandlesAndHandleSelection(sceneView);
                }
                else if (OcclusionDataViewerConfiguration.PersistentValues.PaintWithRadius)
                {
                    handleRadiusPainting(sceneView);
                }
                else
                {
                    handleFaceSelection(sceneView);
                }
            }

            Handles.BeginGUI();
            drawControlsWindow(sceneView);
            Handles.EndGUI();

            HandleUtility.Repaint();
        }

        #region Controls Window

        private static void drawControlsWindow(SceneView sceneView)
        {
            var style = new GUIStyle(GUI.skin.window) { richText = true };
            Rect sceneViewRect = sceneView.camera.pixelRect;
            sceneViewRect.size /= EditorGUIUtility.pixelsPerPoint;

            int targetWidth = 320;
            int targetHeight = 195;
            int margin = 5;

            if (occlusionDataViewerState.IsDebugVisible)
            {
                targetHeight += 18;
            }
            if (occlusionDataViewerState.IsAdvancedVisible)
            {
                targetHeight += 41;
            }
            if (OcclusionDataViewerConfiguration.PersistentValues.PaintWithRadius)
            {
                targetHeight += 36;
            }

            controlsWindowRect = new Rect(sceneViewRect.width - targetWidth - margin, sceneViewRect.height - targetHeight - margin, targetWidth, targetHeight);
            GUILayout.BeginArea(controlsWindowRect, "<b>Controls</b>", style);

            drawOccludeeLODDropDownAndFocusButton();
            drawSelectionButtons();
            drawPaintButton();
            if (OcclusionDataViewerConfiguration.PersistentValues.PaintWithRadius)
            {
                drawRadiusSlider();
            }
            GUILayout.Space(10f);
            drawOccluderAlphaSlider();
            drawShowWireFrameButtons();
            drawDebugButtonAndMaskField();
            drawHorizontalSpacingLine();
            if (occlusionDataViewerState.IsAdvancedVisible)
            {
                drawAdvancedSettings();
            }
            drawAdvancedAndDoneButton();

            GUILayout.EndArea();
        }

        private static void drawOccludeeLODDropDownAndFocusButton()
        {
            var popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.margin.top += 1;
            popupStyle.fixedHeight = 18;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Oclcudee", GUILayout.Width(160));

            EditorGUI.BeginChangeCheck();
#if UNITY_2019_1_OR_NEWER
            int width = 95;
#else
            int width = 92;
#endif
            selectedLODIndex = EditorGUILayout.Popup(selectedLODIndex, occludeeLODDropDownItems ?? new string[] { }, popupStyle, GUILayout.Width(width));
            if (EditorGUI.EndChangeCheck())
            {
                onSelectedLODIndexChanged(selectedLODIndex);
            }
            else if (GUILayout.Button("Focus", GUILayout.Width(50)))
            {
                onFocusButtonClicked();
            }

            GUILayout.EndHorizontal();
        }

        private static void drawSelectionButtons()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Selection", GUILayout.Width(160));

#if UNITY_2019_1_OR_NEWER
            int width = 95;
#else
            int width = 92;
#endif
            if (GUILayout.Button("Generate", GUILayout.Width(width)))
            {
                onGenerateButtonClicked();
            }
            else if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                onClearButtonClicked();
            }

            GUILayout.EndHorizontal();
        }

        private static void drawOccluderAlphaSlider()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Occluder Alpha", GUILayout.Width(160));

            EditorGUI.BeginChangeCheck();
            OcclusionDataViewerConfiguration.PersistentValues.OccluderAlpha = EditorGUILayout.Slider(OcclusionDataViewerConfiguration.PersistentValues.OccluderAlpha, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                Color newColor = OcclusionDataViewerConfiguration.OccluderMaterial.Value.color;
                newColor.a = OcclusionDataViewerConfiguration.PersistentValues.OccluderAlpha;

                UnityRenderPipelineHelper.SetColor(OcclusionDataViewerConfiguration.OccluderMaterial.Value, newColor);
            }

            GUILayout.EndHorizontal();
        }

        private static void drawPaintButton()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Paint tool", GUILayout.Width(160));
            OcclusionDataViewerConfiguration.PersistentValues.PaintWithRadius = GUILayout.Toggle(OcclusionDataViewerConfiguration.PersistentValues.PaintWithRadius, "Brush", new GUIStyle("Button"));

            GUILayout.EndHorizontal();
        }

        private static void drawRadiusSlider()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Brush radius", GUILayout.Width(160));
            OcclusionDataViewerConfiguration.PersistentValues.PaintRadius = EditorGUILayout.Slider(OcclusionDataViewerConfiguration.PersistentValues.PaintRadius * 10, .10000001f, 2f) / 10;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var labelStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, richText = true };
            GUILayout.Label("    Hold <b>CTRL</b> to deselect faces", labelStyle, GUILayout.Width(160 * 2));

            GUILayout.EndHorizontal();
        }

        private static void drawShowWireFrameButtons()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Show Wireframe", GUILayout.Width(160));
            OcclusionDataViewerConfiguration.PersistentValues.ShowOccludeeWireFrame = GUILayout.Toggle(OcclusionDataViewerConfiguration.PersistentValues.ShowOccludeeWireFrame, "Occludee", new GUIStyle("Button"));
            OcclusionDataViewerConfiguration.PersistentValues.ShowOccluderWireFrame = GUILayout.Toggle(OcclusionDataViewerConfiguration.PersistentValues.ShowOccluderWireFrame, "Occluder", new GUIStyle("Button"));

            GUILayout.EndHorizontal();
        }

        private static void drawDebugButtonAndMaskField()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Show Debug", GUILayout.Width(160));

            EditorGUI.BeginChangeCheck();
            occlusionDataViewerState.IsDebugVisible = GUILayout.Toggle(occlusionDataViewerState.IsDebugVisible, "Vertex Handles", new GUIStyle("Button"));
            if (EditorGUI.EndChangeCheck() && !occlusionDataViewerState.IsDebugVisible && clickedViewableMesh != null)
            {
                clickedViewableMesh.SelectedTriangleIndex = null;
                clickedViewableMesh.ClearSelectedVertexHandle();
                clickedViewableMesh = null;
            }

            GUILayout.EndHorizontal();

            if (occlusionDataViewerState.IsDebugVisible)
            {
                var enumFlagsStyle = new GUIStyle(EditorStyles.popup);
                enumFlagsStyle.margin.top += 1;
                enumFlagsStyle.fixedHeight = 18;

                GUILayout.BeginHorizontal();

                GUILayout.Label("Show Raycasts", GUILayout.Width(160));
                occlusionDataViewerState.RaycastVisibilityOptions = (int)(RaycastVisibilityOptions)EditorGUILayout.EnumFlagsField((RaycastVisibilityOptions)occlusionDataViewerState.RaycastVisibilityOptions, enumFlagsStyle);

                GUILayout.EndHorizontal();
            }
        }

        private static void drawHorizontalSpacingLine()
        {
            GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);

#if UNITY_2019_1_OR_NEWER
            GUILayout.Space(10f);
#endif
        }

        private static void drawAdvancedSettings()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Poke-through raycast distance", GUILayout.ExpandWidth(true));
            occlusionDataViewerState.PokeThroughRaycastDistance = EditorGUILayout.FloatField(occlusionDataViewerState.PokeThroughRaycastDistance, GUILayout.Width(50));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Raycasts per vertex", GUILayout.ExpandWidth(true));
            occlusionDataViewerState.AmountOfRaycastsPerVertex = EditorGUILayout.IntField(occlusionDataViewerState.AmountOfRaycastsPerVertex, GUILayout.Width(50));

            GUILayout.EndHorizontal();

#if UNITY_2019_1_OR_NEWER
            int space = 1;
#else
            int space = 5;
#endif
            GUILayout.Space(space);
        }

        private static void drawAdvancedAndDoneButton()
        {
            GUILayout.BeginHorizontal();
            occlusionDataViewerState.IsAdvancedVisible = GUILayout.Toggle(occlusionDataViewerState.IsAdvancedVisible, "Advanced", new GUIStyle("Button"));

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Done", GUILayout.Width(50)))
            {
                onDoneButtonClicked();
            }

            GUILayout.EndHorizontal();
        }

        private static void onFocusButtonClicked()
        {
            selectedOccludeeLOD.FocusInSceneView();
        }

        private static void onGenerateButtonClicked()
        {
            selectedOccludeeLOD.GenerateOcclusionData(occlusionDataViewerState.PokeThroughRaycastDistance, occlusionDataViewerState.AmountOfRaycastsPerVertex);
        }

        private static void onClearButtonClicked()
        {
            if (!EditorUtility.DisplayDialog("Clear Occlusion Data", $"Are you sure you want to clear the occlusion data for LOD {selectedLODIndex}?", "Clear", "Cancel"))
            {
                return;
            }

            selectedOccludeeLOD.ClearOcclusionData();
        }

        private static void onDoneButtonClicked()
        {
            if (!showDoneConfirmationDialog())
            {
                return;
            }

            loadPreviousScene();
        }

        #endregion

        private static void drawWireFrames()
        {
            if (selectedOccludeeLOD != null && OcclusionDataViewerConfiguration.PersistentValues.ShowOccludeeWireFrame)
            {
                selectedOccludeeLOD.DrawWireFrames(Color.black);
            }

            if (selectedOccluderLOD != null && OcclusionDataViewerConfiguration.PersistentValues.ShowOccluderWireFrame)
            {
                selectedOccluderLOD.DrawWireFrames(Color.white);
            }
        }

        private static void drawVertexHandlesAndHandleSelection(SceneView sceneView)
        {
            if (selectedOccludeeLOD == null)
            {
                return;
            }

            switch (Event.current.type)
            {
                case EventType.Layout:
                    if (clickedViewableMesh != null)
                    {
                        clickedViewableMesh.DrawVertexHandles(vertexHandleBaseId, EventType.Layout);
                    }

                    HandleUtility.AddDefaultControl(vertexHandleBaseId - 1);
                    break;
                case EventType.Repaint:
                    if (clickedViewableMesh != null)
                    {
                        clickedViewableMesh.DrawVertexHandles(vertexHandleBaseId, EventType.Repaint);
                        clickedViewableMesh.DrawTriangleOutline(clickedViewableMesh.SelectedTriangleIndex.Value);
                        clickedViewableMesh.DrawRaycasts((RaycastVisibilityOptions)occlusionDataViewerState.RaycastVisibilityOptions);
                        return;
                    }

                    if (controlsWindowRect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }

                    if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hoverHitInfo) || !(hoverHitInfo.collider is MeshCollider hoveredMeshCollider))
                    {
                        return;
                    }
                    EditorGUIUtility.AddCursorRect(sceneView.camera.pixelRect, MouseCursor.Zoom);

                    selectedOccludeeLOD.GetViewableMesh(hoveredMeshCollider).DrawTriangleOutline(hoverHitInfo.triangleIndex);
                    break;
                case EventType.MouseDown:
                    if (Event.current.button != 0 || controlsWindowRect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }

                    if (clickedViewableMesh != null)
                    {
                        int nearestControlId = HandleUtility.nearestControl - vertexHandleBaseId;
                        if (nearestControlId != -1 && (!clickedViewableMesh.SelectedVertexHandleIndex.HasValue || clickedViewableMesh.SelectedVertexHandleIndex.Value != nearestControlId))
                        {
                            clickedViewableMesh.SelectVertexHandle(nearestControlId, occlusionDataViewerState.PokeThroughRaycastDistance, occlusionDataViewerState.AmountOfRaycastsPerVertex);
                            return;
                        }

                        clickedViewableMesh.ClearSelectedVertexHandle();
                    }

                    if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit clickHitInfo) || !(clickHitInfo.collider is MeshCollider clickedMeshCollider))
                    {
                        return;
                    }

                    ViewableMesh newClickedViewableMesh = selectedOccludeeLOD.GetViewableMesh(clickedMeshCollider);
                    if (clickedViewableMesh != newClickedViewableMesh)
                    {
                        if (clickedViewableMesh != null)
                        {
                            clickedViewableMesh.SelectedTriangleIndex = null;
                            clickedViewableMesh.ClearSelectedVertexHandle();
                        }

                        clickedViewableMesh = newClickedViewableMesh;
                    }

                    if (!clickedViewableMesh.SelectedTriangleIndex.HasValue)
                    {
                        clickedViewableMesh.SelectedTriangleIndex = clickHitInfo.triangleIndex;
                        return;
                    }

                    if (clickedViewableMesh.SelectedTriangleIndex == clickHitInfo.triangleIndex)
                    {
                        clickedViewableMesh.SelectedTriangleIndex = null;
                        clickedViewableMesh = null;
                    }
                    break;
            }
        }

        private static void handleFaceSelection(SceneView sceneView)
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(sceneName.GetHashCode(), FocusType.Passive));
                    break;
                case EventType.Repaint:
                    if (controlsWindowRect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }

                    if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hoverHitInfo) || !(hoverHitInfo.collider is MeshCollider hoveredMeshCollider))
                    {
                        return;
                    }

                    ViewableMesh hoveredViewableMesh = selectedOccludeeLOD.GetViewableMesh(hoveredMeshCollider);

                    MouseCursor mouseCursor;
                    if (canHandleDrag)
                    {
                        mouseCursor = wasClickedTriangleOccluded ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus;
                    }
                    else
                    {
                        mouseCursor = hoveredViewableMesh.GetOcclusionDataForHitTriangle(hoverHitInfo.triangleIndex) ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus;
                    }
                    EditorGUIUtility.AddCursorRect(sceneView.camera.pixelRect, mouseCursor);

                    hoveredViewableMesh.DrawTriangleOutline(hoverHitInfo.triangleIndex);
                    break;
                case EventType.MouseDown:
                    if (Event.current.button != 0 || controlsWindowRect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }

                    if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit clickHitInfo) || !(clickHitInfo.collider is MeshCollider clickedMeshCollider))
                    {
                        return;
                    }

                    canHandleDrag = true;

                    ViewableMesh clickedViewableMesh = selectedOccludeeLOD.GetViewableMesh(clickedMeshCollider);

                    wasClickedTriangleOccluded = clickedViewableMesh.GetOcclusionDataForHitTriangle(clickHitInfo.triangleIndex);
                    clickedViewableMesh.SetOcclusionDataForHitTriangle(clickHitInfo.triangleIndex, !wasClickedTriangleOccluded);
                    break;
                case EventType.MouseDrag:
                    if (!canHandleDrag)
                    {
                        return;
                    }

                    if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit dragHitInfo) || !(dragHitInfo.collider is MeshCollider draggedMeshCollider))
                    {
                        return;
                    }

                    ViewableMesh draggedViewableMesh = selectedOccludeeLOD.GetViewableMesh(draggedMeshCollider);

                    draggedViewableMesh.SetOcclusionDataForHitTriangle(dragHitInfo.triangleIndex, !wasClickedTriangleOccluded);
                    break;
                case EventType.MouseUp:
                    canHandleDrag = false;
                    break;
            }
        }

        private static void handleRadiusPainting(SceneView sceneView)
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(sceneName.GetHashCode(), FocusType.Passive));
                    break;
                case EventType.Repaint:
                    if (controlsWindowRect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }

                    if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hoverHitInfo) || !(hoverHitInfo.collider is MeshCollider hoveredMeshCollider))
                    {
                        return;
                    }

                    ViewableMesh hoveredViewableMesh = selectedOccludeeLOD.GetViewableMesh(hoveredMeshCollider);

                    MouseCursor mouseCursor = Event.current.control ? MouseCursor.ArrowMinus : MouseCursor.ArrowPlus;
                    EditorGUIUtility.AddCursorRect(sceneView.camera.pixelRect, mouseCursor);

                    hoveredViewableMesh.DrawRadiusPaintCircle(hoverHitInfo.point, sceneView);
                    break;
                case EventType.MouseDown:
                case EventType.MouseDrag:
                    if (Event.current.button != 0 || controlsWindowRect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }

                    if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit clickHitInfo) || !(clickHitInfo.collider is MeshCollider clickedMeshCollider))
                    {
                        return;
                    }

                    ViewableMesh clickedViewableMesh = selectedOccludeeLOD.GetViewableMesh(clickedMeshCollider);
                    clickedViewableMesh.SetOcclusionDataForHitTriangle(clickHitInfo.triangleIndex, !Event.current.control);
                    clickedViewableMesh.SetOcclusionDataForRadius(clickHitInfo.point, !Event.current.control, sceneView.camera.transform.position);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private static bool onEditorApplicationWantsToQuit()
        {
            if (IsSceneOpen)
            {
                loadPreviousScene();
            }

            return true;
        }

        private static void onEditorSceneClosing(Scene scene, bool removingScene)
        {
            if (scene.name != sceneName)
            {
                return;
            }

            EditorUtility.SetDirty(occlusionDataViewerState.Occludee);
            AssetDatabase.SaveAssets();

            if (SceneView.lastActiveSceneView != null)
            {
                UnityVersionHelper.SetIsSceneLightingOn(occlusionDataViewerState.WasSceneViewLightingOn);
                SceneView.lastActiveSceneView.sceneViewState.showSkybox = occlusionDataViewerState.WasSkyboxOn;
            }

            occlusionDataViewerState = null;

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= onSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= onSceneGUI;
#endif
        }

        private static void load()
        {
            #region Defense

            if (occlusionDataViewerState == null)
            {
                return;
            }

            if (occlusionDataViewerState.Occludee == null || occlusionDataViewerState.Occludee.Prefab == null || !occlusionDataViewerState.Occludee.IsOcclusionDataValid())
            {
                return;
            }

            if (occlusionDataViewerState.Occluder == null || occlusionDataViewerState.Occluder.Prefab == null)
            {
                return;
            }

            #endregion

            if (!tryCreateViewableLODS())
            {
                return;
            }

            occludeeLODDropDownItems = createLODDropDownItems(occludeeLODs.Length);
            selectedLODIndex = 0;

            onSelectedLODIndexChanged(selectedLODIndex);
            selectedOccludeeLOD.FocusInSceneView();

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += onSceneGUI;
#else
            SceneView.onSceneGUIDelegate += onSceneGUI;
#endif
        }

        private static void unload()
        {
            if (occlusionDataViewerState.Animator != null)
            {
                Object.DestroyImmediate(occlusionDataViewerState.Animator);
            }

            for (int i = occlusionDataViewerState.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(occlusionDataViewerState.transform.GetChild(i).gameObject);
            }

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= onSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= onSceneGUI;
#endif
        }

        private static bool tryCreateViewableLODS()
        {
            Renderer[][] occludeeRenderersByLOD = occlusionDataViewerState.Occludee.Prefab.GetRenderersByLOD();
            Renderer[][] occluderRenderersByLOD = occlusionDataViewerState.Occluder.Prefab.GetRenderersByLOD();

            tryCreateRig(occludeeRenderersByLOD, out Animator animator, out Transform rigRootTransform);
            occlusionDataViewerState.Animator = animator;

            var occludeeGameObject = new GameObject("Occludee") { hideFlags = HideFlags.NotEditable };
            var occluderGameObject = new GameObject("Occluder") { hideFlags = HideFlags.NotEditable };

            occludeeGameObject.transform.SetParent(occlusionDataViewerState.transform);
            occluderGameObject.transform.SetParent(occlusionDataViewerState.transform);

            var newOccludeeLODs = new ViewableLOD[occludeeRenderersByLOD.Length];
            var newOccluderLODs = new ViewableLOD[occludeeRenderersByLOD.Length];

            for (int i = 0; i < newOccludeeLODs.Length; i++)
            {
                Mesh[] occludeeSharedMeshes = occludeeRenderersByLOD[i].Select(x => x.GetSharedMesh()).ToArray();
                Mesh[] occluderSharedMeshes = occluderRenderersByLOD.ElementAtOrLast(i).Select(x => x.GetSharedMesh()).ToArray();

                var occluderLOD = ViewableLOD.Create(occluderGameObject.transform, $"LOD {i}", rigRootTransform, occluderRenderersByLOD.ElementAtOrLast(i), occlusionDataViewerState.Occluder.LodOcclusionData.ElementAtOrLast(i), null, OcclusionDataViewerConfiguration.OccluderMaterial.Value);
                if (occluderLOD == null)
                {
                    return false;
                }

                var occludeeLOD = ViewableLOD.Create(occludeeGameObject.transform, $"LOD {i}", rigRootTransform, occludeeRenderersByLOD[i], occlusionDataViewerState.Occludee.LodOcclusionData.ElementAtOrDefault(i), occluderLOD, OcclusionDataViewerConfiguration.OccludeeMaterial.Value);
                if (occludeeLOD == null)
                {
                    return false;
                }

                newOccludeeLODs[i] = occludeeLOD;
                newOccluderLODs[i] = occluderLOD;
            }

            occludeeLODs = newOccludeeLODs;
            occluderLODs = newOccluderLODs;

            return true;
        }

        private static bool tryCreateRig(Renderer[][] renderersByLOD, out Animator animator, out Transform rigRootTransform)
        {
            animator = null;
            rigRootTransform = null;

            #region Defense

            if (!ClothingCullerConfiguration.PersistentValues.ViewMeshesAsSkinned)
            {
                return false;
            }

            ModelImporter modelImporter = tryGetModelImporterForHumanoid(renderersByLOD.SelectMany(x => x).OfType<SkinnedMeshRenderer>().ToArray());
            if (modelImporter == null)
            {
                Debug.LogWarning("Unable to view meshes as skinned - occludee must have at least one SkinnedMeshRenderer of which it's original model is set to 'Humanoid' and has an Avatar.");
                return false;
            }

            GameObject modelGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(modelImporter.assetPath);

            Avatar avatar = getAvatar(modelImporter, modelGameObject);
            if (avatar == null || !avatar.isValid)
            {
                Debug.LogWarning("Unable to view meshes as skinned - the avatar is invalid.");
                return false;
            }

            Transform rigRoot = getRigRoot(modelGameObject, modelImporter.humanDescription);
            if (rigRoot == null)
            {
                Debug.LogWarning("Unable to view meshes as skinned - could not find the rig's root.");
                return false;
            }

            #endregion

            GameObject rigCopy = Object.Instantiate(rigRoot.gameObject);
            rigCopy.name = rigRoot.name;
            rigCopy.transform.SetParent(occlusionDataViewerState.transform);

            foreach (Component component in rigCopy.GetComponentsInChildren<Component>())
            {
                if (component is Transform)
                {
                    component.gameObject.hideFlags = HideFlags.NotEditable;
                    continue;
                }

                Object.DestroyImmediate(component);
            }

            animator = occlusionDataViewerState.gameObject.AddComponent<Animator>();
            animator.avatar = avatar;
            rigRootTransform = rigCopy.transform;

            return true;
        }

        private static ModelImporter tryGetModelImporterForHumanoid(SkinnedMeshRenderer[] skinnedMeshRenderers)
        {
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
                {
                    continue;
                }

                var modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(skinnedMeshRenderer.sharedMesh)) as ModelImporter;
                if (modelImporter != null && modelImporter.animationType == ModelImporterAnimationType.Human && modelImporter.avatarSetup != ModelImporterAvatarSetup.NoAvatar)
                {
                    return modelImporter;
                }
            }

            return null;
        }

        private static Transform getRigRoot(GameObject modelGameObject, HumanDescription humanDescription)
        {
            HumanBone hips = humanDescription.human.FirstOrDefault(x => x.humanName == HumanBodyBones.Hips.ToString());
            if (string.IsNullOrEmpty(hips.boneName))
            {
                return null;
            }

            Transform hipsBone = modelGameObject.GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name == hips.boneName);
            if (hipsBone == null)
            {
                return null;
            }

            Transform rigRoot = hipsBone;

            while (rigRoot.parent != null && rigRoot.parent != modelGameObject.transform)
            {
                rigRoot = rigRoot.parent;
            }

            return rigRoot;
        }

        private static Avatar getAvatar(ModelImporter modelImporter, GameObject modelGameObject)
        {
            switch (modelImporter.avatarSetup)
            {
                case ModelImporterAvatarSetup.CreateFromThisModel:
                    Animator animator = modelGameObject.GetComponent<Animator>();
                    if (animator == null)
                    {
                        return null;
                    }
                    return animator.avatar;
                case ModelImporterAvatarSetup.CopyFromOther:
                    return modelImporter.sourceAvatar;
                default:
                    return null;
            }
        }

        private static string[] createLODDropDownItems(int lodCount)
        {
            string[] lodDropDownItems = new string[lodCount];

            for (int i = 0; i < lodCount; i++)
            {
                lodDropDownItems[i] = $"LOD {i}";
            }

            return lodDropDownItems;
        }

        private static void onSelectedLODIndexChanged(int selectedLodIndex)
        {
            if (occludeeLODs == null || occluderLODs == null)
            {
                return;
            }

            selectedOccludeeLOD = occludeeLODs[selectedLODIndex];
            selectedOccluderLOD = occluderLODs.ElementAtOrLast(selectedLODIndex);

            for (int i = 0; i < occludeeLODs.Length; i++)
            {
                occludeeLODs[i].Activate(i == selectedLodIndex);
                occludeeLODs[i].EnableRenderers(i == selectedLodIndex);
                occludeeLODs[i].EnableMeshColliders(i == selectedLodIndex);
            }

            for (int i = 0; i < occluderLODs.Length; i++)
            {
                occluderLODs[i].Activate(i == selectedLodIndex);
                occluderLODs[i].EnableRenderers(i == selectedLodIndex);
                occluderLODs[i].EnableMeshColliders(false);
            }
        }

        private static bool showDoneConfirmationDialog()
        {
            if (occludeeLODs == null)
            {
                return true;
            }

            if (occludeeLODs.All(x => x.HasOcclusionData()))
            {
                return true;
            }

            return EditorUtility.DisplayDialog("Incomplete Occlusion Data", $"You haven't generated occlusion data for all LODs yet, are you sure you are done?", "Yes", "No");
        }

        private static void loadPreviousScene()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.LookAtDirect(occlusionDataViewerState.PreviousScenePivot, occlusionDataViewerState.PreviousSceneRotation, occlusionDataViewerState.PreviousSceneCameraDistance / 2);
            }

            EditorSceneManager.OpenScene(occlusionDataViewerState.PreviousScenePath, OpenSceneMode.Single);
        }

        #endregion

        #region Public Methods

        public static void OpenScene(OccludeeConfiguration occludee, OccludeeConfiguration occluder)
        {
            if (IsSceneOpen)
            {
                return;
            }

            string previousScenePath = SceneManager.GetActiveScene().path;
            Vector3 previousScenePivot = Vector3.zero;
            Quaternion previousSceneRotation = Quaternion.identity;
            float previousSceneCameraDistance = 0f;
            bool wasSceneViewLightingOn = true;
            bool wasSkyboxOn = true;

            if (SceneView.lastActiveSceneView != null)
            {
                previousScenePivot = SceneView.lastActiveSceneView.pivot;
                previousSceneRotation = SceneView.lastActiveSceneView.rotation;
                previousSceneCameraDistance = SceneView.lastActiveSceneView.cameraDistance;
                wasSceneViewLightingOn = UnityVersionHelper.GetIsSceneLightingOn();
                wasSkyboxOn = SceneView.lastActiveSceneView.sceneViewState.showSkybox;

                UnityVersionHelper.SetIsSceneLightingOn(false);
                SceneView.lastActiveSceneView.sceneViewState.showSkybox = false;
            }

            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            newScene.name = sceneName;

            var gameObject = new GameObject(nameof(OcclusionDataViewer)) { hideFlags = HideFlags.NotEditable };
            occlusionDataViewerState = gameObject.AddComponent<OcclusionDataViewerState>();
            occlusionDataViewerState.Occludee = occludee;
            occlusionDataViewerState.Occluder = occluder;
            occlusionDataViewerState.PokeThroughRaycastDistance = OcclusionDataViewerConfiguration.PersistentValues.DefaultPokeThroughRaycastDistance;
            occlusionDataViewerState.AmountOfRaycastsPerVertex = OcclusionDataViewerConfiguration.PersistentValues.DefaultAmountOfRaycastsPerVertex;
            occlusionDataViewerState.PreviousScenePath = previousScenePath;
            occlusionDataViewerState.PreviousScenePivot = previousScenePivot;
            occlusionDataViewerState.PreviousSceneRotation = previousSceneRotation;
            occlusionDataViewerState.PreviousSceneCameraDistance = previousSceneCameraDistance;
            occlusionDataViewerState.WasSceneViewLightingOn = wasSceneViewLightingOn;
            occlusionDataViewerState.WasSkyboxOn = wasSkyboxOn;

            load();
        }

        public static void ReloadScene(OccludeeConfiguration occludee, OccludeeConfiguration occluder)
        {
            if (!IsSceneOpen || !showDoneConfirmationDialog())
            {
                return;
            }

            occlusionDataViewerState.Occludee = occludee;
            occlusionDataViewerState.Occluder = occluder;

            unload();
            load();
        }

        #endregion

    }
}
