using UnityEngine;
using Bizier.Enums;
using Bizier.Uttils;
using Bizier.Structures;
using System.Collections.Generic;

namespace Bizier {
    public class PathCreator :MonoBehaviour {
        [SerializeField] public PathCreatoreSettings curveSettings;
        [SerializeField] private Path path;
        [SerializeField] private int aproximationCount;
        [SerializeField] private int normalSegmentPerUnit;
        [SerializeField] private float colisionErrorFactor;
        [SerializeField] private float selectRadius;
        [SerializeField] private float controlLength;
        [SerializeField] private CollisionErrorType collisionErrorType;


        public float ColisionErrorFactor => colisionErrorFactor;
        public int AnchoreCount => path.AnchoreCount;
        public float AproximationCount => aproximationCount;
        public float SelectRadius => selectRadius;
        public float ControlLength => controlLength;
        public int NormalSegmentPerUnit => normalSegmentPerUnit;
        public BizierBound PathBound => path.bound;
        public IEnumerable<BizierBound> PathBounds => path.bounds;
        public int PointsCount => path.PointsCount;
        public int SegmentCount => path.SegmentCount;
        public Vector3 this[int i] => path[i];
        public CollisionErrorType CollisionErrorType => collisionErrorType;
        public bool IsClosed => path.IsClosed;
        public bool IsPath => path != null;


        public void SetColisionErrorType(CollisionErrorType type) {
            collisionErrorType = type;
        }

        public CurvePointData GetCurvePointData(float t) {
            return path.GetCurvePointData(t);
        }

        public float GetLength(int i) {
            return path.GetLength(i);
        }

        public float SetNormalSegmentPerUnit(int i) {
            return normalSegmentPerUnit = i;
        }

        public float GetAnchoreNormal(int i) {
            return path.GetAnchoreNormal(i);
        }

        public void SetAnchoreNormal(int i, float normal) {
            path.SetAnchoreNormal(i, normal);
        }

        public void SetColisionErrorFactor(float factor) {
            colisionErrorFactor = factor;
        }

        public void SetControlLength(float length) {
            controlLength = length;
        }

        public void SetSelectedRadius(float radius) {
            selectRadius = radius;
        }

        public void NormalizeCurve(float controlLength) {
            path.NormalizeCurve(controlLength);
        }

        public AnchoreTypes GetAnchoreType(int index) {
            return path.GetAnchoreType(index);
        }

        public void NextAnchoreType(int index) {
            path.NextAnchoreType(index);
        }

        public void SetAproximationCount(int count) {
            path.SetAproximationCount(count);
            aproximationCount = count;
        }

        public Vector3[] GetSegmentPoints(int i) {
            return path.GetSegmentPoints(i);
        }

        public void UpdatePoint(int i, Vector3 pos, bool isAlongDir) {
            path.UpdatePoint(i, pos, isAlongDir);
        }

        public void AddSegment(Vector3 anchorePos) {
            if (path.PointsCount < 4) {
                CreatePath();
            } else {
                path.AddSegment(anchorePos);
            }
        }

        public void RemoveSegment(int index) {
            if (PointsCount > 4) {
                path.RemoveSegment(index);
            }
        }

        public void ToggleIsClose() {
            path.ToggleIsClose();
        }

        public void CreatePath() {
            path = new Path(transform);
            curveSettings = new PathCreatoreSettings();
        }

        public List<int> GetColisionsIndexes(Vector3 point, CollisionErrorType collisionErrorType, float colisionErrorFactor = 1f) {
            var indexList = new List<int>();
            for (int i = 0; i < path.SegmentCount; i++) {
                if (path.IsColide(i, point, collisionErrorType, colisionErrorFactor)) {
                    var localIndex = i;
                    indexList.Add(localIndex);
                }
            }

            return indexList;
        }

        public List<BizierBound> GetColideBounds(Vector3 point) {
            var list = new List<BizierBound>();
            for (int i = 0; i < path.SegmentCount; i++) {
                if (path.IsColide(i, point, collisionErrorType, colisionErrorFactor)) {
                    list.Add(path.GetBound(i));
                }
            }

            return list;
        }

        public bool TryGetClosest(Vector3 point, out float t) {
            var bounds = GetColisionsIndexes(point, collisionErrorType, colisionErrorFactor);
            var segment = 20;
            var factor = 1f / segment;

            var minSegmentIndex = -1;
            var segmentT = 0.5f;
            var minDistance = float.MaxValue;
            for (int i = 0; i < bounds.Count; i++) {
                var segmentIndex = bounds[i];
                var points = path.GetSegmentPoints(segmentIndex);
                for (int j = 0; j < segment; j++) {
                    var t1 = factor * j;
                    var t2 = factor * (j + 1);

                    var pos1 = BizierUtility.GetBuizierPoint(points, t1);
                    var dir1 = BizierUtility.GetBuizierFirstDerivative(points, t1);

                    var pos2 = BizierUtility.GetBuizierPoint(points, t2);
                    var dir2 = BizierUtility.GetBuizierFirstDerivative(points, t2);

                    var dist1 = BizierUtility.GetDistanceToNormal(dir1, pos1, point);
                    var dist2 = BizierUtility.GetDistanceToNormal(dir2, pos2, point);

                    var persistance = 10;
                    if (dist1 >= 0 && dist2 <= 0) {
                        var localFactor = 1f / persistance;
                        for (int g = 0; g <= persistance; g++) {
                            var localT = Mathf.Lerp(t1, t2, localFactor * g);
                            var pos = BizierUtility.GetBuizierPoint(
                                points, localT);
                            var dist = Vector3.Distance(point, pos);
                            if (dist < minDistance) {
                                minSegmentIndex = segmentIndex;
                                segmentT = localT;
                                minDistance = dist;
                            }
                        }
                    }
                }
            }

            if (minSegmentIndex < 0) {
                t = 0;
                return false;
            }

            var length = path.GetLength(0, path.SegmentCount);
            var startSegmentLenght = path.GetLength(0, minSegmentIndex);
            var endSegmentLenght = path.GetLength(0, minSegmentIndex + 1);

            var startSegmentT = Mathf.InverseLerp(0, length, startSegmentLenght);
            var endSegmentT = Mathf.InverseLerp(0, length, endSegmentLenght);
            var segmentTDelta = Mathf.Lerp(0, endSegmentT - startSegmentT, segmentT);

            t = startSegmentT + segmentTDelta;
            return true;
        }

        public void UpdatePath() {
            path.UpdatePath();
        }

        public void CenterCurve() {
            path.CenterCurve();
        }
    }
}
