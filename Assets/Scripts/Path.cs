using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Bizier.Structures;
using Bizier.Uttils;
using Bizier.Enums;


namespace Bizier {
    [Serializable]
    public class Path {
        public Path(Transform tran) {
            var center = tran.position;
            points = new List<Vector3>() {
            center + Vector3.left,
            center + (Vector3.left + Vector3.up) * 0.5f,
            center + (Vector3.right + Vector3.down) * 0.5f,
            center + Vector3.right,
        };
            bounds = new List<BizierBound>();
            lengths = new List<float>();
            anchoreTypes = new List<AnchoreTypes>();
            normals = new List<float>();

            normals.Add(0);
            normals.Add(0);
            anchoreTypes.Add(AnchoreTypes.free);
            anchoreTypes.Add(AnchoreTypes.free);
            aproximationSegmentCount = 30;
            transform = tran;
            CalculateCurveLength();
            CalcBound();
        }


        [SerializeField] private Transform transform;
        [SerializeField] private int aproximationSegmentCount;
        [SerializeField] public List<Vector3> points;
        [SerializeField] public List<BizierBound> bounds;
        [SerializeField] public List<float> lengths;
        [SerializeField] private List<AnchoreTypes> anchoreTypes;
        [SerializeField] public List<float> normals;
        [SerializeField] public BizierBound bound;
        [SerializeField] private bool isClosed;


        public int SegmentCount => points.Count / 3;
        public int AnchoreCount => normals.Count;
        public int PointsCount => points.Count;
        public bool IsClosed => isClosed;
        public Vector3 this[int i] => NormalazeToTransformPoint(i);
        public BizierBound GetBound(int index) => bounds[index];
        public float GetLength(int index) => lengths[index];
        public float GetLength(int index, int count) => lengths.GetRange(index, count).Sum();


        public CurvePointData GetCurvePointData(float t) {
            var index = GetCurvePointSegmentIndex(t, out var segmentT);
            var startRotation = GetAnchoreNormal(index);
            var endRotation = GetAnchoreNormal(index + 1);
            var segmentPoints = GetSegmentPoints(index);
            var pos = BizierUtility.GetBuizierPoint(segmentPoints, segmentT);
            var forwardDir = BizierUtility.GetBuizierFirstDerivative(segmentPoints, segmentT);
            var upPoint = pos + Vector3.up;
            var upDistToPlane = BizierUtility.GetDistanceToNormal(
                forwardDir, pos, upPoint);
            var upPlanePoint = upPoint - forwardDir * upDistToPlane;
            var uoPlaneDir = (upPlanePoint - pos).normalized;
            var rightPlaneDir = (Quaternion.AngleAxis(90, forwardDir) * uoPlaneDir).normalized;
            var degre = Mathf.Lerp(startRotation, endRotation, t);
            var radAngle = Mathf.Deg2Rad * degre;
            var normalDir = pos + (uoPlaneDir * Mathf.Sin(radAngle))
                + (rightPlaneDir * Mathf.Cos(radAngle));
            return new CurvePointData(pos, forwardDir, normalDir);
        }

        public float GetAnchoreNormal(int i) {
            return normals[LoopAnchoreIndex(i)];
        }

        public void SetAnchoreNormal(int i, float normal) {
            normals[LoopAnchoreIndex(i)] = normal;
        }

        public AnchoreTypes GetAnchoreType(int index) {
            return anchoreTypes[index];
        }

        public void NextAnchoreType(int index) {
            AnchoreTypes nextType;
            var type = anchoreTypes[index];
            if (type == AnchoreTypes.free) {
                nextType = AnchoreTypes.symetric;
            } else if (type == AnchoreTypes.symetric) {
                nextType = AnchoreTypes.symertricDirection;
            } else {
                nextType = AnchoreTypes.free;
            }
            anchoreTypes[index] = nextType;
        }

        public void SetAproximationCount(int count) {
            aproximationSegmentCount = count;
        }

        public void NormalizeCurve(float controlsLength = 1f) {
            var localIsClosed = isClosed;
            if (!isClosed) {
                SetIsClosed(true);
            }

            if (IsClosed) {
                for (int i = 0; i < PointsCount; i++) {
                    if (i % 3 == 0) {
                        var prevAnchoreIndex = LoopPointIndex(i - 2);
                        var nextAnchoreIndex = LoopPointIndex(i + 2);
                        var currentAnchore = points[i];
                        var prevAnchore = points[prevAnchoreIndex];
                        var nextAnchore = points[nextAnchoreIndex];
                        var prevDir = (prevAnchore - currentAnchore).normalized;
                        var nextDir = (nextAnchore - currentAnchore).normalized;
                        var dirPrev = (prevDir - nextDir).normalized;
                        var dirNext = (nextDir - prevDir).normalized;
                        var nextControlIndex = LoopPointIndex(i + 1);
                        var prevControlIndex = LoopPointIndex(i - 1);
                        points[nextControlIndex] = currentAnchore + dirNext * controlsLength;
                        points[prevControlIndex] = currentAnchore + dirPrev * controlsLength;
                    }

                }
            }

            if (isClosed != localIsClosed) {
                SetIsClosed(localIsClosed);
            }
        }

        public void AddSegment(Vector3 anchorePos) {
            var normalizePos = TransformToNormalizePoint(anchorePos);
            var localIsClosed = isClosed;
            if (isClosed) {
                SetIsClosed(false);
            }
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add(points[points.Count - 1] + normalizePos * .5f);
            points.Add(normalizePos);
            anchoreTypes.Add(AnchoreTypes.free);
            normals.Add(0);
            lengths.Add(0);
            if (isClosed != localIsClosed) {
                SetIsClosed(localIsClosed);
            }
        }

        public void UpdatePoint(int i, Vector3 pos, bool isAlongDir) {
            var normalizePos = TransformToNormalizePoint(pos);
            AnchoreTypes type = AnchoreTypes.free;
            if (i % 3 == 0) {
                type = anchoreTypes[LoopSegmentIndex(i / 3)];
            } else if (i % 3 == 1) {
                type = anchoreTypes[LoopSegmentIndex((i - 1) / 3)];
            } else if (i % 3 == 2) {
                type = anchoreTypes[LoopSegmentIndex((i + 1) / 3)];
            }

            if (i % 3 == 0) {
                var delta = normalizePos - points[i];
                points[i] = normalizePos;
                if (i + 1 < points.Count || isClosed) {
                    points[LoopPointIndex(i + 1)] += delta;
                }
                if (i - 1 > 0 || isClosed) {
                    points[LoopPointIndex(i - 1)] += delta;
                }
            } else {
                var isNextAnchor = (i + 1) % 3 == 0;
                var correspondingIndex = isNextAnchor ? i + 2 : i - 2;
                var anchoreIndex = isNextAnchor ? i + 1 : i - 1;
                if (isAlongDir) {
                    var dist = (normalizePos - points[LoopPointIndex(anchoreIndex)]).magnitude;
                    var dir = (points[LoopPointIndex(i)] - points[LoopPointIndex(anchoreIndex)]).normalized;
                    points[LoopPointIndex(i)] = points[LoopPointIndex(anchoreIndex)] + dir * dist;

                } else {
                    points[LoopPointIndex(i)] = normalizePos;
                }

                if (correspondingIndex > 0 && correspondingIndex < points.Count || isClosed) {
                    if (type == AnchoreTypes.symetric) {
                        var dist = (points[LoopPointIndex(anchoreIndex)] - points[LoopPointIndex(i)]).magnitude;
                        var dir = (points[LoopPointIndex(anchoreIndex)] - points[LoopPointIndex(i)]).normalized;
                        points[LoopPointIndex(correspondingIndex)] = points[LoopPointIndex(anchoreIndex)] + dir * dist;
                    }
                    if (type == AnchoreTypes.symertricDirection) {
                        var dist = (points[LoopPointIndex(anchoreIndex)] - points[LoopPointIndex(correspondingIndex)]).magnitude;
                        var dir = (points[LoopPointIndex(anchoreIndex)] - points[LoopPointIndex(i)]).normalized;
                        points[LoopPointIndex(correspondingIndex)] = points[LoopPointIndex(anchoreIndex)] + dir * dist;
                    }
                }
            }
        }

        public Vector3[] GetSegmentPoints(int i) {
            return new Vector3[] {
            NormalazeToTransformPoint(i * 3),
            NormalazeToTransformPoint(i * 3 + 1),
            NormalazeToTransformPoint(i * 3 + 2),
            NormalazeToTransformPoint(LoopPointIndex(i * 3 + 3))
        };
        }

        public void CenterCurve() {
            var count = 0;
            var posSumm = Vector3.zero;
            for (int i = 0; i < points.Count; i++) {
                if (i % 3 == 0) {
                    count++;
                    posSumm += points[i];
                }
            }

            var diff = posSumm / count;
            for (int i = 0; i < points.Count; i++) {
                points[i] -= diff;
            }
        }

        public void RemoveSegment(int i) {
            if (i % 3 != 0) {
                return;
            }

            var localIsClose = isClosed;
            if (isClosed) {
                SetIsClosed(false);
            }

            var isFirst = i == 0;
            var isLast = i == points.Count - 1;

            if (isFirst) {
                points.RemoveRange(0, 3);
                anchoreTypes.RemoveAt(0);
            }

            if (isLast) {
                points.RemoveRange(i - 2, 3);
                anchoreTypes.RemoveAt(anchoreTypes.Count - 1);
            }

            if (!isFirst && !isLast) {
                points.RemoveRange(i - 1, 3);
                anchoreTypes.RemoveAt(i / 3);
            }


            if (isClosed != localIsClose) {
                SetIsClosed(localIsClose);
            }

            normals.RemoveAt(i % 3);
            lengths.RemoveAt(i % 3);
        }

        public void ToggleIsClose() {
            isClosed = !isClosed;
            if (isClosed) {
                points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
                points.Add(points[0] * 2 - points[1]);
            } else {
                points.RemoveRange(points.Count - 2, 2);
            }
            UpdatePath();
        }

        public void CalcBound() {
            var localArrayBounds = new List<BizierBound>();
            for (int i = 0; i < SegmentCount; i++) {
                var segmentPoints = GetSegmentPoints(i);
                localArrayBounds.Add(BizierUtility.GetBizierBound(
                    segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3]));
            }

            bounds = localArrayBounds;
            bound = BizierUtility.GetBound(bounds);
        }

        public bool IsColide(int boundIndex, Vector3 point, CollisionErrorType collisionErrorType, float colisionErrorFactor = 1f) {
            return BizierUtility.GetIsColide(bounds[boundIndex], point, collisionErrorType, colisionErrorFactor);
        }

        public void CalculateCurveLength() {
            lengths.Clear();
            for (int i = 0; i < SegmentCount; i++) {
                var segmentPoint = GetSegmentPoints(i);
                var length = BizierUtility.GetBizierSegmentArchLength(segmentPoint[0], segmentPoint[1],
                    segmentPoint[2], segmentPoint[3], aproximationSegmentCount);
                lengths.Add(length);
            }
        }

        public int GetCurvePointSegmentIndex(float t, out float segmentT) {
            var curveLength = lengths.Sum();
            var length = Mathf.Lerp(0, curveLength, t);
            var index = 0;
            var segmentStartLength = 0f;
            var count = lengths.Count + (isClosed ? 1 : +0);
            for (int i = 0; i < count; i++) {
                var currLength = lengths[LoopSegmentIndex(i)];
                if (length >= segmentStartLength
                    && length <= (segmentStartLength + currLength) + 0.01f) {
                    segmentT = (length - segmentStartLength) / currLength;
                    return i;
                }
                segmentStartLength += currLength;
            }

            segmentT = 0;
            return index;
        }

        public void UpdatePath() {
            CalcBound();
            CalculateCurveLength();
            UpdateNormals();

        }

        private void UpdateNormals() {
            if (SegmentCount != normals.Count) {
                var localNormals = new List<float>();
                var normalCount = normals.Count;
                for (int i = 0; i < SegmentCount; i++) {
                    if (i < normalCount) {
                        localNormals.Add(normals[i]);
                    } else {
                        localNormals.Add(0);
                    }
                }
                normals = localNormals;
            }
        }


        private void SetIsClosed(bool isClosed) {
            this.isClosed = isClosed;
            if (this.isClosed) {
                points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
                points.Add(points[0] * 2 - points[1]);
            } else {
                points.RemoveRange(points.Count - 2, 2);
            }
        }

        private int LoopPointIndex(int index) {
            return (index + points.Count) % points.Count;
        }

        private int LoopSegmentIndex(int index) {
            return (index + SegmentCount) % SegmentCount;
        }

        private int LoopAnchoreIndex(int index) {
            return (index + AnchoreCount) % AnchoreCount;
        }

        private Vector3 NormalazeToTransformPoint(int i) {
            var rotation = transform.rotation;
            var m = Matrix4x4.TRS(transform.position, rotation, transform.lossyScale);
            return m.MultiplyPoint3x4(points[i]);
        }

        private Vector3 TransformToNormalizePoint(Vector3 point) {
            var rotation = transform.rotation;
            var m = Matrix4x4.TRS(transform.position, rotation, transform.lossyScale).inverse;
            return m.MultiplyPoint3x4(point);
        }
    }
}