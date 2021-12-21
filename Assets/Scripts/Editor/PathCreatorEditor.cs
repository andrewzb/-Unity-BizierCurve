using UnityEngine;
using UnityEditor;
using Bizier.Enums;
using Bizier.Assets;
using Bizier.Uttils;

namespace Bizier.CustomEditorScripts {
    [CustomEditor(typeof(PathCreator))]
    public class PathCreatorEditor : Editor {
        private PathCreator pathCreater;
        private BizierDisplaySetting displaySettings;
        private Editor displaySettingsEditor;

        private Vector3 prevTranPos;
        private Vector3 prevTranScale;
        private Quaternion prevTranQuantarion;

        private bool isNeedUpdate;
        private bool isShowDrawSettings;
        private bool isShowNormalize;
        private bool isShowAproximationCount;
        private bool isShowAction;
        private bool isShowSelectionRadius;
        private bool isShowColisionError;
        private bool isShowInfo;
        private bool isShowOffset;

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField($"Curve stats: Segments => {pathCreater.SegmentCount}; " +
                $"points => {pathCreater.PointsCount}");

            EditorGUILayout.Space(5, true);
            DrawInfo();
            EditorGUILayout.Space(5, true);
            DrawToggleIsClose();
            EditorGUILayout.Space(5, true);
            DrawOffset();
            EditorGUILayout.Space(5, true);
            DrawActionButtons();
            EditorGUILayout.Space(5, true);
            DrawNormalizeCurve();
            EditorGUILayout.Space(5, true);
            DrawColisionErrorFactor();
            EditorGUILayout.Space(5, true);
            DrawAproximationCount();
            EditorGUILayout.Space(5, true);
            DrawSelectionRadius();
            EditorGUILayout.Space(5, true);
            DrawCurveSettings();
            EditorGUILayout.Space(5, true);
            DrawViewSettings();
            EditorGUILayout.Space(5, true);
        }

        public void OnSceneGUI() {
            //input
            InputCreateHandler();
            InputRemoveHandler();
            InputNextAnchoreType();
            CheckTransform();

            // display
            DrawCurve();
            DrawBoxColisions();
            DrawCurveHanles();
            DrawNormalHanles();
            DrawNormals();
            DrawAnchoreNumber();

            if (isNeedUpdate) {
                pathCreater.UpdatePath();
                isNeedUpdate = false;
            }
        }

        #region View
        private void DrawCurve() {
            for (int i = 0; i < pathCreater.SegmentCount; i++) {
                var points = pathCreater.GetSegmentPoints(i);
                Handles.color = displaySettings.controlLine;
                if (pathCreater.curveSettings.IsDisplayControlPoints) {
                    Handles.DrawLine(points[0], points[1]);
                    Handles.DrawLine(points[2], points[3]);
                }
                Handles.DrawBezier(
                    points[0], points[3], points[1], points[2], displaySettings.bezierPath, null, 2);
            }
        }

        private void DrawBoxColisions() {
            if (pathCreater.curveSettings.IsShowPathBounds) {
                var bound = pathCreater.PathBound;
                Handles.color = displaySettings.commonBox;
                Handles.DrawWireCube(bound.Center, bound.Size);
            }
            if (pathCreater.curveSettings.IsShowPerSegmentBounds) {
                var bounds = pathCreater.PathBounds;
                Handles.color = displaySettings.separetBox;
                foreach (var bound in bounds) {
                    Handles.DrawWireCube(bound.Center, bound.Size);
                }
            }
            if (pathCreater.curveSettings.IsShowPerSegmentColisionBound) {
                var bounds = pathCreater.PathBounds;
                Handles.color = displaySettings.colisionBox;
                var localCollisionErrorType = pathCreater.CollisionErrorType;
                foreach (var bound in bounds) {
                    var size = bound.Size;
                    switch (localCollisionErrorType) {
                        case CollisionErrorType.Multiply:
                        size *= pathCreater.ColisionErrorFactor;
                        break;
                        case CollisionErrorType.Additional:
                        size += Vector3.one * pathCreater.ColisionErrorFactor;
                        break;
                        default:
                        size *= pathCreater.ColisionErrorFactor;
                        break;
                    }
                    Handles.DrawWireCube(bound.Center, size);
                }
            }
        }

        private void DrawCurveHanles() {
            var guiEvent = Event.current;
            for (int i = 0; i < pathCreater.PointsCount; i++) {
                var size = 0.1f;
                if (i % 3 == 0) {
                    if (!pathCreater.curveSettings.IsDisplayAnchorPoints) {
                        continue;
                    }
                    size = displaySettings.anchorSize;
                    var type = pathCreater.GetAnchoreType(i / 3);
                    if (type == AnchoreTypes.free) {
                        Handles.color = displaySettings.anchoreFree;
                    } else if (type == AnchoreTypes.symetric) {
                        Handles.color = displaySettings.anchoreSymetric;
                    } else {
                        Handles.color = displaySettings.anchoreSymetricDirection;
                    }

                } else {
                    if (!pathCreater.curveSettings.IsDisplayControlPoints) {
                        continue;
                    }
                    size = displaySettings.controlSize;
                    Handles.color = displaySettings.control;
                }

                var newPos = Handles.FreeMoveHandle(
                    pathCreater[i], Quaternion.identity, size, Vector3.zero, Handles.SphereHandleCap);


                if (pathCreater[i] != newPos) {
                    Undo.RecordObject(pathCreater, "Update point position");
                    pathCreater.UpdatePoint(i, newPos, guiEvent.shift);
                    isNeedUpdate = true;
                }
            }
        }

        private void DrawNormalHanles() {
            if (pathCreater.curveSettings.IsShowNormalsHandles) {
                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = displaySettings.LabelFontSize;
                style.normal.textColor = displaySettings.normalLabel;

                var guiEvent = Event.current;
                var pointerRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
                var upPosition = HandleUtility.GUIPointToWorldRay(
                    new Vector2(guiEvent.mousePosition.x, guiEvent.mousePosition.y + 1));
                var rightPosition = HandleUtility.GUIPointToWorldRay(
                    new Vector2(guiEvent.mousePosition.x + 1, guiEvent.mousePosition.y));
                var upDirection = (pointerRay.origin - upPosition.origin).normalized;
                var rightDirection = (rightPosition.origin - pointerRay.origin).normalized;
                var direction = SceneView.currentDrawingSceneView.camera.transform.forward;
                for (int i = 0; i < pathCreater.PointsCount; i++) {
                    var size = displaySettings.normalSize;
                    if (i % 3 == 0) {
                        Handles.color = displaySettings.normalHandle;
                        var degree = pathCreater.GetAnchoreNormal(i / 3);

                        Handles.DrawWireArc(
                            pathCreater[i], -direction, upDirection, 360, size);
                        Handles.DrawSolidArc(
                            pathCreater[i], -direction, upDirection, degree % 360, size);

                        Handles.Label(pathCreater[i], ((int)(degree / 360)).ToString(), style);

                        var handlePos = BizierUtility.GetNormalHandlePosition(
                            pathCreater[i], upDirection, rightDirection, size, degree);

                        Handles.color = displaySettings.anchoreFree;
                        var newPos = Handles.FreeMoveHandle(handlePos,
                            Quaternion.identity, 0.01f, Vector3.zero, Handles.SphereHandleCap);

                        if (newPos != handlePos) {
                            var startAngle = BizierUtility.GetAndgle(pathCreater[i], upDirection,
                                rightDirection, handlePos, displaySettings.normalSize);
                            var projectedPosition = BizierUtility.GetSnapPosition(
                                pathCreater[i], direction, newPos, displaySettings.normalSize);
                            var endAngle = BizierUtility.GetAndgle(pathCreater[i], upDirection,
                                rightDirection, projectedPosition, displaySettings.normalSize);
                            var angleDiff = BizierUtility.GetAngleDiff(startAngle, endAngle);
                            pathCreater.SetAnchoreNormal(i / 3, degree + angleDiff);
                            isNeedUpdate = true;
                            SceneView.RepaintAll();
                        }
                    }
                }
            }
        }

        private void DrawNormals() {
            if (pathCreater.curveSettings.IsShowNormals) {
                Handles.color = displaySettings.normals;
                for (int i = 0; i < pathCreater.SegmentCount; i++) {
                    var points = pathCreater.GetSegmentPoints(i);
                    var startRotation = pathCreater.GetAnchoreNormal(i);
                    var endRotation = pathCreater.GetAnchoreNormal(i + 1);
                    var localCount = (int)(pathCreater.GetLength(i)
                        * displaySettings.normalSegmentPerUnit);
                    var factor = 1f / localCount;

                    for (int j = 0; j < localCount; j++) {
                        var t = factor * j;
                        var pos = BizierUtility.GetBuizierPoint(points, t);
                        var forwardDir = BizierUtility.GetBuizierFirstDerivative(points, t);
                        var upPoint = pos + Vector3.up;
                        var upDistToPlane = BizierUtility.GetDistanceToNormal(
                            forwardDir, pos, upPoint);
                        var upPlanePoint = upPoint - forwardDir * upDistToPlane;
                        var uoPlaneDir = (upPlanePoint - pos).normalized;
                        var rightPlaneDir = (
                            Quaternion.AngleAxis(90, forwardDir) * uoPlaneDir).normalized;
                        var degre = Mathf.Lerp(startRotation, endRotation, t);
                        var radAngle = Mathf.Deg2Rad * degre;
                        var newPos = pos
                            + (uoPlaneDir * displaySettings.normalsLength * Mathf.Sin(radAngle))
                            + (rightPlaneDir * displaySettings.normalsLength * Mathf.Cos(radAngle));
                        Handles.DrawLine(pos, newPos);
                    }
                }
            }
        }

        private void DrawAnchoreNumber() {
            if (pathCreater.curveSettings.IsDisplayAnchoreNumber) {
                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = displaySettings.anchoreNumberLabelSize;
                style.normal.textColor = displaySettings.anchoreLabelNumber;
                for (int i = 0; i < pathCreater.PointsCount; i++) {
                    if (i % 3 == 0) {
                        Handles.Label(pathCreater[i], (i / 3).ToString(), style);
                    }
                }
            }
        }
        #endregion

        #region Input
        private void CheckTransform() {
            if (pathCreater.IsPath) {
                var tran = pathCreater.transform;
                if (prevTranPos != tran.position || prevTranScale != tran.lossyScale
                    || prevTranQuantarion != tran.rotation) {
                    pathCreater.UpdatePath();
                    prevTranPos = tran.position;
                    prevTranScale = tran.lossyScale;
                    prevTranQuantarion = tran.rotation;
                    isNeedUpdate = true;
                }
            }
        }

        private void InputCreateHandler() {
            var guiEvent = Event.current;
            var pointerRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            var mousePos = pointerRay.origin;
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift && guiEvent.alt) {
                var dist = BizierUtility.GetDistanceToNormal(
                    -pointerRay.direction, pathCreater.transform.position, mousePos);
                var newPos = mousePos + pointerRay.direction * dist;
                Undo.RecordObject(pathCreater, "add new segment");
                pathCreater.AddSegment(newPos);
                isNeedUpdate = true;
                SceneView.RepaintAll();
            }
        }

        private void InputRemoveHandler() {
            var guiEvent = Event.current;
            var pointerRay = guiEvent.mousePosition;
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.control && guiEvent.shift) {
                if (TryGetAnchorePoint(pointerRay, out var resultIndex)) {
                    Undo.RecordObject(pathCreater, "remove segment");
                    pathCreater.RemoveSegment(resultIndex);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
            }
        }

        private void InputNextAnchoreType() {
            var guiEvent = Event.current;
            var pointerRay = guiEvent.mousePosition;
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.alt) {
                if (TryGetAnchorePoint(pointerRay, out var resultIndex)) {
                    pathCreater.SetNextAnchoreType(resultIndex / 3);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
            }
        }
        #endregion

        #region Inspector
        private void DrawOffset() {
            isShowOffset = EditorGUILayout.Foldout(isShowOffset, "Show Offset");
            if (isShowOffset) {
                var localOffsetType = (OffsetType)EditorGUILayout.EnumPopup("Offset Type", pathCreater.OffsetType);
                if (localOffsetType != pathCreater.OffsetType) {
                    pathCreater.SetOffsetType(localOffsetType);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }

                var localOffset = EditorGUILayout.Vector3Field("Offset", pathCreater.Offset);
                if (localOffset != pathCreater.Offset) {
                    pathCreater.UpdateOffset(localOffset);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
            }
        }

        private void DrawInfo() {
            isShowInfo = EditorGUILayout.Foldout(isShowInfo, "Show Info");
            if (isShowInfo) {
                EditorGUILayout.LabelField($"Add shift + alt + click(LMB)");
                EditorGUILayout.LabelField($"Remove shift + ctrl + click(LMB)");
                EditorGUILayout.LabelField($"Change Anchore Type shift + alt + click(LMB)");
                EditorGUILayout.LabelField($"Select control + shift drag along direction");
            }
        }

        private void DrawColisionErrorFactor() {
            isShowColisionError = EditorGUILayout.Foldout(isShowColisionError, "Show Collision Error");
            if (isShowColisionError) {
                var localIsColisionErrorType = (CollisionErrorType)EditorGUILayout.EnumPopup(
                    "Collision Error Type", pathCreater.CollisionErrorType);
                if (localIsColisionErrorType != pathCreater.CollisionErrorType) {
                    pathCreater.SetColisionErrorType(localIsColisionErrorType);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
                var localColisionErrorFactor = pathCreater.ColisionErrorFactor;
                localColisionErrorFactor = EditorGUILayout.Slider(pathCreater.ColisionErrorFactor, 0, 2);

                if (localColisionErrorFactor != pathCreater.ColisionErrorFactor) {
                    pathCreater.SetColisionErrorFactor(localColisionErrorFactor);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
            }
        }

        private void DrawSelectionRadius() {
            isShowSelectionRadius = EditorGUILayout.Foldout(isShowSelectionRadius, "Show Selection Radius");
            if (isShowSelectionRadius) {
                var localSelectRadius = EditorGUILayout.Slider(pathCreater.SelectRadius, 10, 100);
                if (localSelectRadius != pathCreater.SelectRadius) {
                    pathCreater.SetSelectedRadius(localSelectRadius);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
            }
        }

        private void DrawNormalizeCurve() {
            isShowNormalize = EditorGUILayout.Foldout(isShowNormalize, "Show Normilize Curve");
            if (isShowNormalize) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Controls Length");
                var localControlLength = EditorGUILayout.Slider(pathCreater.ControlLength, 0.1f, 2);
                if (localControlLength != pathCreater.ControlLength) {
                    pathCreater.SetControlLength(localControlLength);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("normalaze")) {
                    Undo.RecordObject(pathCreater, "normalaze");
                    pathCreater.NormalizeCurve(pathCreater.ControlLength);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawAproximationCount() {
            isShowAproximationCount = EditorGUILayout.Foldout(isShowAproximationCount, "Show Aproximation Count");
            if (isShowAproximationCount) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Aproximation Count");
                var localAproximationCount = EditorGUILayout.Slider(pathCreater.AproximationCount, 10, 50);
                if (localAproximationCount != pathCreater.AproximationCount) {
                    Undo.RecordObject(pathCreater, "Change aproximation Count");
                    pathCreater.SetAproximationCount((int)localAproximationCount);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawViewSettings() {
            EditorGUILayout.LabelField($"ViewSettings");
            isShowDrawSettings = EditorGUILayout.InspectorTitlebar(isShowDrawSettings, displaySettings);
            if (isShowDrawSettings) {
                CreateCachedEditor(displaySettings, null, ref displaySettingsEditor);
                displaySettingsEditor.OnInspectorGUI();
                isNeedUpdate = true;
                SceneView.RepaintAll();
            }
        }

        private void DrawCurveSettings() {
            var curveSettings = serializedObject.FindProperty("curveSettings");
            EditorGUILayout.PropertyField(curveSettings);
            if (serializedObject.ApplyModifiedProperties()) {
                isNeedUpdate = true;
                SceneView.RepaintAll();
            }
        }

        private void DrawActionButtons() {
            isShowAction = EditorGUILayout.Foldout(isShowAction, "Show Actions");
            if (isShowAction) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("reset")) {
                    Undo.RecordObject(pathCreater, "reset");
                    pathCreater.CreatePath();
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }

                if (GUILayout.Button("force Update")) {
                    Undo.RecordObject(pathCreater, "force Update");
                    pathCreater.UpdatePath(pathCreater.transform);
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("center")) {
                    Undo.RecordObject(pathCreater, "centrilize");
                    pathCreater.CenterCurve();
                    isNeedUpdate = true;
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawToggleIsClose() {
            var isClosed = EditorGUILayout.Toggle("isClose", pathCreater.IsClosed);
            if (pathCreater.IsClosed != isClosed) {
                Undo.RecordObject(pathCreater, "toggle is close");
                pathCreater.ToggleIsClose();
                isNeedUpdate = true;
                SceneView.RepaintAll();
            }
        }
        #endregion

        private bool TryGetAnchorePoint(Vector2 screnPoint, out int closestPoint) {
            var index = -1;
            var minDist = float.MaxValue;

            for (int i = 0; i < pathCreater.PointsCount; i++) {
                if (i % 3 == 0) {
                    var point = pathCreater[i];
                    var screenPos = HandleUtility.WorldToGUIPoint(point);
                    var dist = Vector2.Distance(screnPoint, screenPos);
                    if (dist < minDist) {
                        index = i;
                        minDist = dist;
                    }
                }
            }

            if (minDist < pathCreater.SelectRadius) {
                closestPoint = index;
                return true;
            }

            closestPoint = -1;
            return false;
        }

        private void LoadDisplaySettings() {
            displaySettings = BizierDisplaySetting.Load();
        }

        void OnDisable() {
            Tools.hidden = false;
        }

        void OnEnable() {
            pathCreater = (PathCreator)target;

            if (!pathCreater.IsPath) {
                pathCreater.CreatePath();
            }
            pathCreater.UpdatePath(pathCreater.transform);
            LoadDisplaySettings();
        }
    }
}