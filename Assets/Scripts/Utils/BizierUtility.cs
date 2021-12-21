using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Bizier.Enums;
using Bizier.Structures;

namespace Bizier.Uttils {
    public class BizierUtility :MonoBehaviour {
        public static Vector3 GetBuizierPoint(Vector3[] points, float factor) {
            return GetBuizierPoint(points[0], points[1], points[2], points[3], factor);
        }

        public static Vector3 GetBuizierPoint(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor) {
            var t = Mathf.Clamp01(factor);
            var rt = 1f - t;
            return rt * rt * rt * p0 +
                    3f * rt * rt * t * p1 +
                    3f * rt * t * t * p2 +
                    t * t * t * p3;
        }


        public static Vector3 GetBuizierFirstDerivative(Vector3[] points, float factor) {
            return GetBuizierFirstDerivative(points[0], points[1], points[2], points[3], factor);
        }

        public static Vector3 GetBuizierFirstDerivative(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor) {
            var t = Mathf.Clamp01(factor);
            var rt = 1f - t;
            return
                (3f * rt * rt * (p1 - p0) +
                6f * rt * t * (p2 - p1) +
                3f * t * t * (p3 - p2)).normalized;
        }


        public static Vector3 GetBuizierSecondDerivative(Vector3[] points, float factor) {
            return GetBuizierSecondDerivative(points[0], points[1], points[2], points[3], factor);
        }

        public static Vector3 GetBuizierSecondDerivative(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor) {
            var t = Mathf.Clamp01(factor);
            var rt = 1f - t;
            return
                (3f * rt * rt * (p1 - p0) +
                6f * rt * t * (p2 - p1) +
                3f * t * t * (p3 - p2)).normalized;
        }


        public static float GetBuizierLength(Vector3[] points, int count) {
            return GetBuizierLength(points[0], points[1], points[2], points[3], count);
        }

        public static float GetBuizierLength(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int count) {
            var archLength = 0f;
            for (int i = 0; i < count; i++) {
                var tStart = (1f / count) * i;
                var tEnd = (1f / count) * (i + 1);
                var firstPoint = GetBuizierPoint(p0, p1, p2, p3, tStart);
                var secondPoint = GetBuizierPoint(p0, p1, p2, p3, tEnd);
                archLength += Vector3.Distance(firstPoint, secondPoint);
            }
            return archLength;
        }


        public static BizierBound GetBizierBound(Vector3[] points) {
            return GetBizierBound(points[0], points[1], points[2], points[3]);
        }

        public static BizierBound GetBizierBound(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var a = -3 * p0 + 9 * p1 - 9 * p2 + 3 * p3;
            var b = 6 * p0 - 12 * p1 + 6 * p2;
            var c = -3 * p0 + 3 * p1;

            var xResult = EquesionSolver.TrySolveQuadraticEquesion(a.x, b.x, c.x, out var xRoots);
            var yResult = EquesionSolver.TrySolveQuadraticEquesion(a.y, b.y, c.y, out var yRoots);
            var zResult = EquesionSolver.TrySolveQuadraticEquesion(a.z, b.z, c.z, out var zRoots);

            var args = new List<float>();
            if (xResult == WorkFlowResult.Success) {
                args.Add(xRoots.Item1);
                args.Add(xRoots.Item2);
            }

            if (yResult == WorkFlowResult.Success) {
                args.Add(yRoots.Item1);
                args.Add(yRoots.Item2);
            }

            if (zResult == WorkFlowResult.Success) {
                args.Add(zRoots.Item1);
                args.Add(zRoots.Item2);
            }

            var filterArgs = args.Where((el => el >= 0 && el <= 1)).ToList();

            var maxBound = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var minBound = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            maxBound = new Vector3(Mathf.Max(maxBound.x, p0.x),
                Mathf.Max(maxBound.y, p0.y), Mathf.Max(maxBound.z, p0.z));
            minBound = new Vector3(Mathf.Min(minBound.x, p0.x),
                Mathf.Min(minBound.y, p0.y), Mathf.Min(minBound.z, p0.z));

            maxBound = new Vector3(Mathf.Max(maxBound.x, p3.x),
                Mathf.Max(maxBound.y, p3.y), Mathf.Max(maxBound.z, p3.z));
            minBound = new Vector3(Mathf.Min(minBound.x, p3.x),
                Mathf.Min(minBound.y, p3.y), Mathf.Min(minBound.z, p3.z));

            foreach (var t in args) {
                var point = GetBuizierPoint(p0, p1, p2, p3, t);
                maxBound = new Vector3(Mathf.Max(maxBound.x, point.x),
                    Mathf.Max(maxBound.y, point.y), Mathf.Max(maxBound.z, point.z));
                minBound = new Vector3(Mathf.Min(minBound.x, point.x),
                    Mathf.Min(minBound.y, point.y), Mathf.Min(minBound.z, point.z));
            }

            return new BizierBound((maxBound + minBound) / 2, maxBound - minBound);
        }


        public static bool GetIsPointForvard(Vector3[] points, float factor, Vector3 point) {
            return GetIsPointForvard(points[0], points[1], points[2], points[3], factor, point);
        }

        public static bool GetIsPointForvard(
            Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor, Vector3 point) {
            var factorPoint = GetBuizierPoint(p0, p1, p2, p3, factor);
            var directionPoint = GetBuizierFirstDerivative(p0, p1, p2, p3, factor);
            var dist = GetDistanceToNormal(directionPoint, factorPoint, point);
            return dist >= 0;
        }



        public static float GetBizierSegmentArchLength(Vector3[] points, int segmentCount) {
            return GetBizierSegmentArchLength(points[0], points[1], points[2], points[3], segmentCount);
        }

        public static float GetBizierSegmentArchLength(
            Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segmentCount) {
            var archLength = 0f;
            var factor = 1f / segmentCount;
            for (int i = 0; i < segmentCount; i++) {
                var tStart = factor * i;
                var tEnd = factor * (i + 1);
                var firstPoint = GetBuizierPoint(p0, p1, p2, p3, tStart);
                var secondPoint = GetBuizierPoint(p0, p1, p2, p3, tEnd);
                archLength += Vector3.Distance(firstPoint, secondPoint);
            }
            return archLength;
        }


        public static float GetDistanceToNormal(Vector3 planeNormal, Vector3 planePos, Vector3 pointPos) {
            var c = Mathf.Sqrt(Mathf.Pow(planeNormal.x, 2) + Mathf.Pow(planeNormal.y, 2) + Mathf.Pow(planeNormal.z, 2));
            var b = planeNormal.x * planePos.x + planeNormal.y * planePos.y + planeNormal.z * planePos.z;
            var a = planeNormal.x * pointPos.x + planeNormal.y * pointPos.y + planeNormal.z * pointPos.z;
            var dist = ((a - b) / c);
            return dist;
        }

        public static bool GetIsColide(
            BizierBound bound, Vector3 point, CollisionErrorType collisionErrorType, float colisionErrorFactor = 1f) {
            var hs = bound.Size / 2;
            switch (collisionErrorType) {
                case CollisionErrorType.Multiply:
                hs *= colisionErrorFactor;
                break;
                case CollisionErrorType.Additional:
                hs += Vector3.one * colisionErrorFactor;
                break;
                default:
                hs *= colisionErrorFactor;
                break;
            }

            var c = bound.Center;
            return (point.x > c.x - hs.x && point.x < c.x + hs.x)
                && (point.y > c.y - hs.y && point.y < c.y + hs.y)
                && (point.z > c.z - hs.z && point.z < c.z + hs.z);
        }

        public static BizierBound GetBound(IEnumerable<BizierBound> bounds) {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var bound in bounds) {
                var hs = bound.Size / 2;
                var bmin = bound.Center - hs;
                var bmax = bound.Center + hs;
                min = new Vector3(
                    Mathf.Min(min.x, bmin.x), Mathf.Min(min.y, bmin.y), Mathf.Min(min.z, bmin.z));
                max = new Vector3(
                    Mathf.Max(max.x, bmax.x), Mathf.Max(max.y, bmax.y), Mathf.Max(max.z, bmax.z));
            }

            return new BizierBound((max + min) / 2, max - min);
        }

        public static float GetAndgle(Vector3 origin, Vector3 up, Vector3 right,
            Vector3 pos, float radius, bool clocWise = true) {
            var yDist = GetDistanceToNormal(up, origin, pos) / radius;
            var xDist = GetDistanceToNormal(right, origin, pos) / radius;

            var baseAngle = 0f;
            var angle = Mathf.Acos(yDist) * Mathf.Rad2Deg;
            if (!float.IsNaN(angle)) {
                if (xDist >= 0) {
                    baseAngle = angle;
                } else {
                    baseAngle = 360 - angle;
                }
            }

            return baseAngle;
        }

        public static Vector3 GetNormalHandlePosition(
        Vector3 pos, Vector3 upDir, Vector3 rightDir, float radius, float angle) {
            var radAngle = Mathf.Deg2Rad * angle;
            var newPos = pos + (radius * rightDir * Mathf.Sin(radAngle))
                + (radius * upDir * Mathf.Cos(radAngle));
            return newPos;
        }

        public static Vector3 GetSnapPosition(
            Vector3 normalOrigin, Vector3 normalDir, Vector3 pos, float radius) {
            var dist = GetDistanceToNormal(normalDir, normalOrigin, pos);
            var newPos = pos + normalDir * dist;
            var direction = (newPos - normalOrigin).normalized;
            return normalOrigin + direction * radius;
        }

        public static float GetAngleDiff(float starAngle, float endAngle) {
            var diff = (endAngle - starAngle) % 360;
            if (Mathf.Abs(diff) > 180) {
                var isClockWise = diff > 0;
                diff = ((endAngle + 180 * (isClockWise ? -1 : 1))
                    - (starAngle + 180 * (isClockWise ? 1 : -1))) % 360;
            }

            return diff;
        }
    }
}